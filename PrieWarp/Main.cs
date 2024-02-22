using BepInEx;
using System;

namespace PrieWarp
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("Blasphemous.ModdingAPI", "2.1.0")]
    [BepInDependency("Blasphemous.CheatConsole", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        private static PrieWarp? prieWarp;
        public static PrieWarp PrieWarp
        {
            get => prieWarp ?? throw new NullReferenceException("Early access to PrieWarp instance");
        }

        private void Start()
        {
            prieWarp = new(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, "BadMagic", PluginInfo.PLUGIN_VERSION);
        }
    }
}
