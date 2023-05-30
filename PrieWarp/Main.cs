using BepInEx;
using System;

namespace PrieWarp
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.damocles.blasphemous.modding-api", "1.3.4")]
    public class Main : BaseUnityPlugin
    {
        private static PrieWarp? prieWarp;
        public static PrieWarp PrieWarp
        {
            get => prieWarp ?? throw new NullReferenceException("Early access to PrieWarp instance");
        }

        private void Start()
        {
            prieWarp = new(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION);
        }
    }
}
