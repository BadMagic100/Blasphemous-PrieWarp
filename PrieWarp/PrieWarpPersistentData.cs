using Blasphemous.ModdingAPI.Persistence;
using System;
using System.Collections.Generic;

namespace PrieWarp
{
    [Serializable]
    public class PrieWarpPersistentData : SlotSaveData
    {
        public PrieWarpPersistentData() : base() { }

        public HashSet<string> unlockedPrieDieus = new();
        public bool unlockAllPrieDieus = false;
    }
}
