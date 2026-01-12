using Blasphemous.CheatConsole;
using Blasphemous.ModdingAPI;
using Blasphemous.ModdingAPI.Persistence;

namespace PrieWarp
{
    public class PrieWarp : BlasMod, ISlotPersistentMod<PrieWarpPersistentData>
    {
        public const string PERSISTENT_ID = "ID_PRIEWARP";

        public WarpManager? WarpManager { get; private set; }
        public HotkeyWatcher? HotkeyWatcher { get; private set; }

        public PrieWarpPersistentData LocalSaveData { get; private set; } = new();

        public string PersistentID => PERSISTENT_ID;

        public PrieWarp(string modId, string modName, string author, string modVersion) : base(modId, modName, author, modVersion) { }

        protected override void OnInitialize()
        {
            if (!WarpManager.TryLoad(ConfigHandler, FileHandler, out WarpManager? warpManager))
            {
                ModLog.Error("Failed PrieWarp setup - could not prepare warp data.");
                return;
            }
            WarpManager = warpManager;
            HotkeyWatcher = new HotkeyWatcher();
        }

        protected override void OnRegisterServices(ModServiceProvider provider)
        {
            provider.RegisterCommand(new PrieWarpCommand());
        }

        protected override void OnUpdate()
        {
            HotkeyWatcher?.Update();
        }

        public PrieWarpPersistentData SaveSlot() => LocalSaveData;

        public void LoadSlot(PrieWarpPersistentData data) => LocalSaveData = data;

        protected override void OnNewGame()
        {
            LocalSaveData = new PrieWarpPersistentData();
        }

        public void ResetSlot()
        {
            LocalSaveData = new PrieWarpPersistentData();
        }
    }
}
