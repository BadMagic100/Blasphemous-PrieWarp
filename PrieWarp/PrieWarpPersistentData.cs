﻿using ModdingAPI;
using System;
using System.Collections.Generic;

namespace PrieWarp
{
    [Serializable]
    public class PrieWarpPersistentData : ModPersistentData
    {
        public PrieWarpPersistentData() : base(PrieWarp.PERSISTENT_ID) { }

        public HashSet<string> unlockedPrieDieus = new();
        public bool unlockAllPrieDieus = false;
    }
}
