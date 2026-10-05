# DadsAdventureMapping

DadsAdventureMapping expands the map area revealed while the local player moves. In Dads BepInEx Mod Manager, set **Reveal Size Multiplier** with the 2x–5x slider.

The mod adds a native map pin only after the player comes within 25 game units of a matching placed location, resource deposit, or boss Vegvisir. It reads Valheim's placed-location records and nearby networked objects, and matches their actual prefab names.

Towers, resource veins, tombs, burial chambers, crypts, caves and caverns, and mines are enabled by default. Houses, villages, tar pits, boss sites, boss Vegvisirs, camps, ruins, fortresses, farms, merchants, runestones, wells, shipwrecks, stone circles, landmarks, nests, excavation sites, and spawners are off by default. Every category has its own switch in the mod manager. A nearby existing pin prevents a duplicate. Turning a category off stops future pins; it does not remove saved pins.

The location-name catalog was checked against the installed Valheim 1.0.16 `_ZoneSystem` and `_LocationList` assets. It classifies 162 placed-location prefab names; the remaining 15 entries are development or test locations. Resource and boss Vegvisir names were checked against the game's `manifest_extended` asset listing.

Install `DadsAdventureMapping.dll` in `BepInEx/plugins/DadsAdventureMapping/`. Requires BepInExPack for Valheim. Settings are saved in `BepInEx/config/com.dadisbored.dadsadventuremapping.cfg` and can be edited in Dads BepInEx Mod Manager.
