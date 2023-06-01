using Framework.Managers;
using HarmonyLib;
using Tools.Level.Interactables;

namespace PrieWarp
{
    [HarmonyPatch(typeof(SpawnManager))]
    [HarmonyPatch("ActivePrieDieu", MethodType.Setter)]
    internal static class SpawnManager_SetActivePrieDieu
    {
        static void Postfix(PrieDieu __0)
        {
            Main.PrieWarp.Log($"Level: {Core.LevelManager.currentLevel.LevelName} ID: {__0.GetPersistenID()}");
        }
    }
}
