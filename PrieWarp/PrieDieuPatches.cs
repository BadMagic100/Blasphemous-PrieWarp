using Framework.Managers;
using HarmonyLib;

namespace PrieWarp
{
    [HarmonyPatch(typeof(SpawnManager))]
    [HarmonyPatch("ActivePrieDieu", MethodType.Setter)]
    internal static class SpawnManager_SetActivePrieDieu
    {
        static void Postfix()
        {
            Main.PrieWarp.WarpManager?.UnlockPrieDieuInCurrentScene();
        }
    }
}
