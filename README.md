# DadsAdventureMapping

DadsAdventureMapping expands the map area revealed while the local player moves. In Dads BepInEx Mod Manager, set **Reveal Size Multiplier** with the 2x–5x slider.

The mod adds a native map pin only after the player comes within 25 game units of a matching placed location, resource deposit, or boss Vegvisir. It reads Valheim's placed-location records and nearby networked objects, and matches their actual prefab names.

The mod manager has one switch for each POI type. Copper, Tin, Silver, Iron, and other resources use their resource names. All towers share **Tower**, all houses share **House**, all boss Vegvisirs share **Vegvisir**, and graves plus burial chambers share **Tomb**. Swamp crypts use **Crypt**, infested mine entrances use **Mine**, and caves use **Cave**. Dolmens have their own **Dolmen** switch.

Hildir's quest locations share **Hildir Locations**. The vendor switches are **Hildir**, **Haldor**, and **Bog Witch**. Other repeated types, including camps, ruins, runestones, wells, and shipwrecks, each share a switch. Every switch describes the exact prefab names it covers.

Tower, resource, Tomb, Crypt, Cave, and Mine are enabled by default. Dolmen and most other types are off by default. A nearby existing pin prevents a duplicate. Turning a switch off stops future pins; it does not remove saved pins.

The location-name catalog was checked against the installed Valheim 1.0.16 `_ZoneSystem` and `_LocationList` assets. It classifies 162 placed-location prefab names; the remaining 15 entries are development or test locations. Resource, vendor, and boss Vegvisir names were checked against the game's `manifest_extended` asset listing. The 162 location names and 26 object prefab names are controlled by 47 switches.

Install `DadsAdventureMapping.dll` in `BepInEx/plugins/DadsAdventureMapping/`. Requires BepInExPack for Valheim. Settings are saved in `BepInEx/config/com.dadisbored.dadsadventuremapping.cfg` and can be edited in Dads BepInEx Mod Manager.
