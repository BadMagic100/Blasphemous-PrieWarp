using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Penitent;
using HarmonyLib;
using ModdingAPI;
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

        private readonly Dictionary<string, WarpPoint> warpsByHotkey;
        private readonly Dictionary<string, WarpPoint> warpsById;
        private readonly Dictionary<string, WarpPoint> warpsByScene;

        private WarpManager(Dictionary<string, WarpPoint> warps)
        {
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
            Main.PrieWarp.Log($"Requested warp to {hotkey}");
            if (hotkey == "SS")
            {
                Main.PrieWarp.Log("Warpning to starting location");
                SpawnManager.OnPlayerSpawn += OnRespawnCompleted;
                Core.SpawnManager.ResetPersistence();
                Core.Events.SetFlag("CHERUB_RESPAWN", true);
                Core.SpawnManager.Respawn();
            }
            else if (hotkey == "LL")
            {
                Main.PrieWarp.Log("Warping to last save point");
                SpawnManager.OnPlayerSpawn += OnRespawnCompleted;
                Core.Events.SetFlag("CHERUB_RESPAWN", true);
                Core.SpawnManager.Respawn();
            }
            else if (warpsByHotkey.TryGetValue(hotkey, out WarpPoint warp))
            {
                // todo - before load, close whatever menu we are in (once we figure out what menu we're doing this from)
                Main.PrieWarp.Log($"Found warp for hotkey: {warp.id} in {warp.scene}");
                if (!CanWarp(warp))
                {
                    Main.PrieWarp.Log("Could not warp - destination is locked");
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
                Main.PrieWarp.LogWarning($"Failed to find warp for hotkey");
                return false;
            }
            return true;
        }

        internal void UnlockPrieDieuInCurrentScene()
        {
            string id = warpsByScene[Core.LevelManager.currentLevel.LevelName].id;
            Main.PrieWarp.Log($"Unlocking Prie Dieu {id} (visited)");
            Main.PrieWarp.LocalSaveData.unlockedPrieDieus.Add(id);
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

        public static bool TryLoad(FileUtil fileUtil, [NotNullWhen(true)] out WarpManager? warpManager)
        {
            if (fileUtil.loadDataText("prieDieus.json", out string jsonPDs))
            {
                List<WarpPoint> pds = fileUtil.jsonObject<List<WarpPoint>>(jsonPDs);
                Dictionary<string, WarpPoint> warps = new();
                foreach (WarpPoint wp in pds)
                {
                    // todo - allow configuring hotkeys, may require game restart if needed
                    Main.PrieWarp.Log($"Adding warp point: {wp.defaultHotkey} -> {wp.id}");
                    warps.Add(wp.defaultHotkey, wp);
                }
                warpManager = new WarpManager(warps);
                return true;
            }
            else
            {
                Main.PrieWarp.LogError("Failed to load warp points");
                warpManager = null;
                return false;
            }
        }
    }
}
