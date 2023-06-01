using Framework.Managers;
using Framework.Util;
using HarmonyLib;
using ModdingAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        private readonly Dictionary<string, WarpPoint> warps;

        private WarpManager(Dictionary<string, WarpPoint> warps)
        {
            this.warps = warps;
        }

        public void AttemptWarp(string hotkey)
        {
            Main.PrieWarp.Log($"Requested warp to {hotkey}");
            if (warps.TryGetValue(hotkey, out WarpPoint warp))
            {
                // todo - before load, close whatever menu we are in (once we figure out what menu we're doing this from)
                Main.PrieWarp.Log($"Found warp for hotkey: {warp.label} in {warp.scene}");
                SpawnManager.OnTeleportPrieDieu += OnWarpCompleted;
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
            }
        }

        private bool CanWarp(WarpPoint warp)
        {
            return Main.PrieWarp.LocalSaveData.unlockAllPrieDieus || 
        }

        private void OnWarpCompleted(string spawnId)
        {
            SpawnManager.OnTeleportPrieDieu -= OnWarpCompleted;
            PrieDieu pd = UnityEngine.Object.FindObjectOfType<PrieDieu>();
            Singleton<Core>.Instance.StartCoroutine(WaitUntilReadyThenUse(pd));
        }

        private IEnumerator WaitUntilReadyThenUse(PrieDieu pd)
        {
            yield return new WaitUntil(() => !Core.Input.InputBlocked);
            pd.Use();
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
                    Main.PrieWarp.Log($"Adding warp point: {wp.defaultHotkey} -> {wp.label}");
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
