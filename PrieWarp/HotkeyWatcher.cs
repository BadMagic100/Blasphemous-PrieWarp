using Gameplay.UI;
using Gameplay.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrieWarp
{
    public class HotkeyWatcher
    {
        private bool watching = false;
        private List<string> currentInput = new();

        private static readonly IEnumerable<KeyCode> alphaKeys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .Select(c => Enum.Parse(typeof(KeyCode), c.ToString()))
            .OfType<KeyCode>()
            .ToList();

        public void SetWatchingState(bool state)
        {
            watching = state;
            if (!state)
            {
                currentInput.Clear();
            }
            Main.PrieWarp.Log($"Hotkey watching active: {state}");
        }

        public void Update()
        {
            if (!watching || ConsoleWidget.Instance.IsEnabled())
            {
                return;
            }

            foreach (KeyCode key in alphaKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    if (currentInput.Count == 2)
                    {
                        currentInput.RemoveAt(0);
                    }
                    currentInput.Add(key.ToString());
                }
            }

            if (currentInput.Count == 2)
            {
                string input = currentInput[0] + currentInput[1];
                if (Main.PrieWarp.WarpManager!.AttemptWarp(input))
                {
                    currentInput.Clear();
                    UIController.instance.HideInventory();
                }
            }
        }
    }
}
