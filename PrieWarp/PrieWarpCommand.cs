using Framework.Managers;
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
                ["unlock"] = Unlock,
                ["warp"] = Warp
            };
        }

        private void Help(string[] args)
        {
            Write("Available commands:");
            Write("unlock <id>|ALL - unlocks a specific Prie Dieu, or toggles the ability to teleport to any Prie Dieu");
            Write("warp <hotkey> - warps to a Prie Dieu by its hotkey");
        }

        private void Unlock(string[] args)
        {
            if (!ValidateParameterList(args, 1)
                || !ValidateWarpManager(out WarpManager? warpManager)
                || !ValidateInSave())
            {
                return;
            }

            PrieWarpPersistentData data = Main.PrieWarp.LocalSaveData;
            string id = args[0];
            if (id == "all")
            {
                data.unlockAllPrieDieus = !data.unlockAllPrieDieus;
                Write($"{ToggledVerb(data.unlockAllPrieDieus)} all Prie Dieus");
            }
            else if (!warpManager.WarpExists(id))
            {
                Write($"No warp with the id {id} exists");
            }
            else
            {
                data.unlockedPrieDieus.Add(id);
                Write($"Unlocked Prie Dieu with id {id}");
            }
        }

        private void Warp(string[] args)
        {
            if (!ValidateParameterList(args, 1) 
                || !ValidateWarpManager(out WarpManager? warpManager)
                || !ValidateInSave())
            {
                return;
            }

            warpManager.AttemptWarp(args[0].ToUpper());
        }

        private string ToggledVerb(bool newState) => newState ? "Unlocked" : "Locked";

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

        private bool ValidateInSave()
        {
            if (Core.LevelManager.currentLevel.LevelName == "MainMenu")
            {
                Write("PrieWarp commands can only be used from inside a save file.");
                return false;
            }
            return true;
        }
    }
}
