using Gameplay.UI.Others.MenuLogic;
using HarmonyLib;

namespace PrieWarp
{
    [HarmonyPatch(typeof(NewInventoryWidget), "Show")]
    internal static class NewInventoryWidget_Show
    {
        static void Postfix(NewInventoryWidget __instance)
        {
            Main.PrieWarp.HotkeyWatcher?.SetWatchingState(__instance.currentlyActive);
        }
    }
}
