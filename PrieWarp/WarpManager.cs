using ModdingAPI;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PrieWarp
{
    public class WarpManager
    {
        private readonly Dictionary<string, WarpPoint> warps;

        private WarpManager(Dictionary<string, WarpPoint> warps)
        {
            this.warps = warps;
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
