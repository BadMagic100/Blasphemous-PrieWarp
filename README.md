# Blasphemous-PrieWarp
A Blasphemous mod which allows easily warping between unlocked Prie Dieus with mnemonic hotkeys.

## Hotkeys

When in the inventory, entering alphabetical keys will count towards hotkey entry. The 2 most recently pressed keys will be treated as your hotkey,
and if the hotkey is valid, you will be warped to the corresponding Prie Dieu or hardsave. The default hotkeys are mnemonic, and use the following rules of thumb

1. If the Prie Dieu is near a boss, the hotkey indicates the boss (e.g. EX -> Exposito).
2. If there is only one Prie Dieu in the area (after the above rule is applied), the hotkey indicates the area name.
3. If there are multiple Prie Dieus in the area, the one nearest the entrance takes the hotkey indicating the area name.
4. All Prie Dieus in the area will share the same first key, unless taken from a boss name.

The default hotkeys are listed here:
* SS -> **S**tart
* LL -> **L**ast hardsave
* BS -> **B**rotherhood of the **S**ilent Sorrow (Main)
* BU -> **B**rotherhood of the Silent Sorrow (**U**pper)
* HL -> **H**oly **L**ine
* AA -> **A**lbero
* BR -> **Br**idge of Three Calvalries
* MD -> **M**ercy **D**reams (Entrance)
* TP -> **T**en **P**iedad
* DC -> **D**esecrated **C**istern
* MC -> **M**ountain (by **C**istern)
* MB -> **M**ountain (by **B**rotherhood)
* JO -> **Jo**ndo
* EW -> **E**choes of Salt (**W**est)
* EE -> **E**choes of Salt (**E**ast)
* OT -> Where **O**live **T**rees Wither
* GP -> **G**raveyard of the **P**eaks
* CE -> **C**onvent of our Lady of the Charred Visage (**E**ntrance)
* CV -> Our Lady of the **C**harred **V**isage
* PS -> **P**atio of the **S**ilent Steps
* MM -> **M**other of **M**others (Knot of Three Words)
* MQ -> **M**el**q**uiades
* LN -> **L**ibrary of **N**egated Words (Entrance)
* LP -> **L**ibrary of Negated Words (**P**atio Shortcut)
* SC -> **S**leeping **C**anvases (Entrance)
* EX -> **Ex**posito
* RT -> Archcathedral **R**oof**t**ops (Entrance)
* RE -> Archcathedral **R**ooftops (Below **E**levator)
* RU -> Archcathedral **R**ooftops (**U**pper)
* DH -> **D**eambulatory of His **H**oliness
* MH -> **M**ourning and **H**avoc (By Elevator)
* SI -> **Si**erpes
* HD -> **H**all of **D**awning

The hotkeys can be changed in your config folder. After starting the game for the first time with PrieWarp installed, the config file will be generated.
Changes to the config will require a restart of the game to take effect.

## Commands

* `prie unlock` - unlocks a Prie Dieu with the given ID. IDs are listed in a resources file. You can also use `prie unlock all` to toggle the ability to warp
  to any Prie Dieu, regardless of if you've visited it.
* `prie warp` - manually input a hotkey outside of the inventory.

## Known Issues

* If you warp out of a boss fight, you may carry the healthbar with you.
