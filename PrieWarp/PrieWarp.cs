using ModdingAPI;

namespace PrieWarp
{
    public class PrieWarp : Mod
    {
        public PrieWarp(string modId, string modName, string modVersion) : base(modId, modName, modVersion) { }

        protected override void Initialize()
        {
            if (!WarpManager.TryLoad(FileUtil, out WarpManager? warpManager))
            {
                LogError("Failed PrieWarp setup - could not prepare warp data.");
            }
        }
    }
}
