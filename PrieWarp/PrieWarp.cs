using ModdingAPI;

namespace PrieWarp
{
    public class PrieWarp : PersistentMod
    {
        public const string PERSISTENT_ID = "ID_PRIEWARP";

        public WarpManager? WarpManager { get; private set; }
        public HotkeyWatcher? HotkeyWatcher { get; private set; }

        public PrieWarpPersistentData LocalSaveData { get; private set; } = new();

        public override string PersistentID => PERSISTENT_ID;

        public PrieWarp(string modId, string modName, string modVersion) : base(modId, modName, modVersion) { }

        protected override void Initialize()
        {
            if (!WarpManager.TryLoad(FileUtil, out WarpManager? warpManager))
            {
                LogError("Failed PrieWarp setup - could not prepare warp data.");
                return;
            }
            WarpManager = warpManager;
            HotkeyWatcher = new HotkeyWatcher();
            RegisterCommand(new PrieWarpCommand());
        }

        protected override void Update()
        {
            HotkeyWatcher?.Update();
        }

        public override ModPersistentData SaveGame() => LocalSaveData;

        public override void LoadGame(ModPersistentData data) => LocalSaveData = (PrieWarpPersistentData)data;

        public override void NewGame(bool NGPlus)
        {
            LocalSaveData = new PrieWarpPersistentData();
        }

        public override void ResetGame()
        {
            LocalSaveData = new PrieWarpPersistentData();
        }
    }
}
