using Blasphemous.ModdingAPI.Persistence;
using System;
using System.Collections.Generic;

namespace PrieWarp
{
    [Serializable]
    public class PrieWarpPersistentData : SaveData
    {
        public PrieWarpPersistentData() : base(PrieWarp.PERSISTENT_ID) { }

        public HashSet<string> unlockedPrieDieus = new();
        public bool unlockAllPrieDieus = false;
    }
}
