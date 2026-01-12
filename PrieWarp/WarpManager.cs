using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Config;
using Blasphemous.ModdingAPI.Files;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Penitent;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Tools.Level.Interactables;
using UnityEngine;

namespace PrieWarp
{
    public class WarpManager
    {
        // SpawnFromTeleportOnPrieDieu doesn't quite work, since we want to force a reload
        private static readonly MethodInfo spawnManagerSpawnPlayer = AccessTools.Method(
            typeof(SpawnManager), "SpawnPlayer", new Type[] {
                typeof(string),
                typeof(SpawnManager.PosibleSpawnPoints),
                typeof(string),
                typeof(bool),
                typeof(bool),
                typeof(Color?)
            });

        private readonly string warpStartHotkey;
        private readonly string warpLastHotkey;
        private readonly Dictionary<string, WarpPoint> warpsByHotkey;
        private readonly Dictionary<string, WarpPoint> warpsById;
        private readonly Dictionary<string, WarpPoint> warpsByScene;

        private WarpManager(string warpStartHotkey, string warpLastHotkey, Dictionary<string, WarpPoint> warps)
        {
            this.warpStartHotkey = warpStartHotkey;
            this.warpLastHotkey = warpLastHotkey;
            this.warpsByHotkey = warps;
            this.warpsById = warps.Values.ToDictionary(x => x.id);
            this.warpsByScene = warps.Values.ToDictionary(x => x.scene);
        }

        public bool WarpExists(string id) => warpsById.ContainsKey(id);

        public bool AttemptWarp(string hotkey)
        {
            // usage of cherub respawn is actually functional - it is apparently the only way to track the state
            // of on ongoing respawn reliably, otherwise we may restore resources before you're allowed to
            //
            // but also it's aesthetically pleasing
            ModLog.Info($"Requested warp to {hotkey}");
            if (hotkey == warpStartHotkey)
            {
                ModLog.Info("Warping to starting location");
                SpawnManager.OnPlayerSpawn += OnRespawnCompleted;
                Core.SpawnManager.ResetPersistence();
                Core.Events.SetFlag("CHERUB_RESPAWN", true);
                Core.SpawnManager.Respawn();
            }
            else if (hotkey == warpLastHotkey)
            {
                ModLog.Info("Warping to last save point");
                SpawnManager.OnPlayerSpawn += OnRespawnCompleted;
                Core.Events.SetFlag("CHERUB_RESPAWN", true);
                Core.SpawnManager.Respawn();
            }
            else if (warpsByHotkey.TryGetValue(hotkey, out WarpPoint warp))
            {
                ModLog.Info($"Found warp for hotkey: {warp.id} in {warp.scene}");
                if (!CanWarp(warp))
                {
                    ModLog.Info("Could not warp - destination is locked");
                    return false;
                }
                SpawnManager.OnTeleportPrieDieu += OnWarpToPrieDieuCompleted;
                Core.Events.SetFlag("CHERUB_RESPAWN", true);
                spawnManagerSpawnPlayer.Invoke(Core.SpawnManager, new object?[] {
                    warp.scene,
                    SpawnManager.PosibleSpawnPoints.TeleportPrieDieu,
                    string.Empty,
                    true,
                    true,
                    null
                });
            }
            else
            {
                ModLog.Warn($"Failed to find warp for hotkey");
                return false;
            }
            return true;
        }

        internal void UnlockPrieDieuInCurrentScene()
        {
            string id = warpsByScene[Core.LevelManager.currentLevel.LevelName].id;
            if (Main.PrieWarp.LocalSaveData.unlockedPrieDieus.Add(id))
            {
                ModLog.Info($"Unlocked Prie Dieu {id} (first visit)");
            }
        }

        private bool CanWarp(WarpPoint warp)
        {
            PrieWarpPersistentData data = Main.PrieWarp.LocalSaveData;
            return data.unlockAllPrieDieus || data.unlockedPrieDieus.Contains(warp.id);
        }

        private void OnRespawnCompleted(Penitent penitent)
        {
            SpawnManager.OnPlayerSpawn -= OnRespawnCompleted;
            Singleton<Core>.Instance.StartCoroutine(WaitUntilReadyThenSimulateUse(null));
        }

        private void OnWarpToPrieDieuCompleted(string spawnId)
        {
            SpawnManager.OnTeleportPrieDieu -= OnWarpToPrieDieuCompleted;
            PrieDieu pd = UnityEngine.Object.FindObjectOfType<PrieDieu>();
            Singleton<Core>.Instance.StartCoroutine(WaitUntilReadyThenSimulateUse(pd));
        }

        private IEnumerator WaitUntilReadyThenSimulateUse(PrieDieu? pd)
        {
            yield return new WaitUntil(() => !Core.Events.GetFlag("CHERUB_RESPAWN"));
            yield return new WaitUntil(() => !Core.Input.InputBlocked);
            if (pd != null)
            {
                // activate the pd (and consequently unlock it) in the event that we unlock-all'd our way here
                Core.SpawnManager.ActivePrieDieu = pd;
            }
            SimulateReusePrieDieu();
        }

        private void SimulateReusePrieDieu()
        {
            Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
            Core.Logic.Penitent.Stats.Flask.SetToCurrentMax();
            if (Core.Alms.GetPrieDieuLevel() > 1)
            {
                Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
            }
            Core.Persistence.SaveGame();
            Core.Logic.EnemySpawner.RespawnDeadEnemies();
            Core.Logic.BreakableManager.Reset();
        }

        public static bool TryLoad(ConfigHandler configHandler, FileHandler fileHandler, [NotNullWhen(true)] out WarpManager? warpManager)
        {
            HotkeyConfig config = configHandler.Load<HotkeyConfig>();
            if (fileHandler.LoadDataAsJson("prieDieus.json", out List<WarpPoint> pds))
            {
                Dictionary<string, WarpPoint> warps = new();
                foreach (WarpPoint wp in pds)
                {
                    if (!config.PrieDieuHotkeys.ContainsKey(wp.id))
                    {
                        config.PrieDieuHotkeys[wp.id] = wp.defaultHotkey;
                    }
                    string hotkey = config.PrieDieuHotkeys[wp.id];
                    ModLog.Info($"Adding warp point: {hotkey} -> {wp.id}");
                    warps.Add(hotkey, wp);
                }
                // persist defaults
                configHandler.Save(config);
                warpManager = new WarpManager(config.WarpToStartHotkey, config.WarpToLastHardsaveHotkey, warps);
                return true;
            }
            else
            {
                ModLog.Error("Failed to load warp points");
                warpManager = null;
                return false;
            }
        }
    }
}
