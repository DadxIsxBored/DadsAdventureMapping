# DadsAdventureMapping

DadsAdventureMapping expands the map area revealed while the local player moves. In Dads BepInEx Mod Manager, set **Reveal Size Multiplier** with the 2x–5x slider.

The mod adds a native map pin only after the player comes within 25 game units of a matching placed location, resource deposit, or boss Vegvisir. It reads Valheim's placed-location records and nearby networked objects, and matches their actual prefab names.

Each distinct prefab family has its own mod manager switch. Numbered variants share a switch: `Dolmen01`–`Dolmen03` are controlled by **Dolmens**, while `WoodHouse1`–`WoodHouse13` are controlled by **Wood Houses**. Dolmens are separate from Graves. Each switch describes the exact prefab names it covers.

Tower, resource deposit, grave, chamber, crypt, cave, and mine families are enabled by default. Dolmens and most other families are off by default. A nearby existing pin prevents a duplicate. Turning a switch off stops future pins; it does not remove saved pins.

The location-name catalog was checked against the installed Valheim 1.0.16 `_ZoneSystem` and `_LocationList` assets. It classifies 162 placed-location prefab names; the remaining 15 entries are development or test locations. Resource and boss Vegvisir names were checked against the game's `manifest_extended` asset listing. The 162 location names and 22 object prefab names form 100 separately configurable families after numbered variants are grouped.

Install `DadsAdventureMapping.dll` in `BepInEx/plugins/DadsAdventureMapping/`. Requires BepInExPack for Valheim. Settings are saved in `BepInEx/config/com.dadisbored.dadsadventuremapping.cfg` and can be edited in Dads BepInEx Mod Manager.
