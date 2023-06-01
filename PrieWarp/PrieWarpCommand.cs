using ModdingAPI.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PrieWarp
{
    internal class PrieWarpCommand : ModCommand
    {
        protected override string CommandName => "prie";

        protected override bool AllowUppercase => false;

        protected override Dictionary<string, Action<string[]>> AddSubCommands()
        {
            return new Dictionary<string, Action<string[]>>()
            {
                ["help"] = Help,
                ["list"] = List,
                ["toggleunlock"] = ToggleUnlock,
                ["warp"] = Warp
            };
        }

        private void Help(string[] args)
        {

        }

        private void List(string[] args)
        {

        }

        private void ToggleUnlock(string[] args)
        {
            if (!ValidateWarpManager(out WarpManager? warpManager))
            {
                return;
            }

            if (args.Length == 1 && args[0] == "all")
            {
                Main.PrieWarp.LocalSaveData
                Write("Unlocked all prie dieus");
                return;
            }
        }

        private void Warp(string[] args)
        {
            if (!ValidateParameterList(args, 1) || !ValidateWarpManager(out WarpManager? warpManager))
            {
                return;
            }

            warpManager.AttemptWarp(args[0].ToUpper());
        }

        private bool ValidateWarpManager([NotNullWhen(true)] out WarpManager? warpManager)
        {
            if (Main.PrieWarp.WarpManager == null)
            {
                Write("Could not perform warp - no warp manager available.");
                warpManager = null;
                return false;
            }
            warpManager = Main.PrieWarp.WarpManager;
            return true;
        }
    }
}
