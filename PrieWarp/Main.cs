using BepInEx;

namespace PrieWarp
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.damocles.blasphemous.modding-api", "1.3.4")]
    public class Main : BaseUnityPlugin
    {
        public static PrieWarp PrieWarp;

        private void Start()
        {
            PrieWarp = new(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION);
        }
    }
}
