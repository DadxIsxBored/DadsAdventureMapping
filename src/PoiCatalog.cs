using System;
using System.Collections.Generic;

namespace DadsAdventureMapping;

internal enum PoiCategory
{
    Towers, Houses, ResourceVeins, Tombs, Chambers, Crypts, Caves, Mines,
    Villages, TarPits, BossAltars, BossVegvisirs, Camps, Ruins, Fortresses,
    Farms, Merchants, Runestones, Wells, Shipwrecks, StoneCircles, Landmarks,
    Nests, Excavations, Spawners
}

// Names below come from the Valheim 1.0.16 _ZoneSystem and _LocationList
// prefabs in valheim_Data/StreamingAssets/SoftRef/Bundles, plus object prefab
// paths in manifest_extended. Only actual prefab names are accepted.
internal static class PoiCatalog
{
    internal static readonly int LocationProxyHash = "LocationProxy".GetStableHashCode();
    private static readonly Dictionary<string, PoiCategory> Locations = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<int, PoiCategory> Objects = new();

    static PoiCatalog()
    {
        AddLocations(PoiCategory.Towers, "StoneTower1 StoneTower2 StoneTower3 StoneTower4 StoneTowerRuins03 StoneTowerRuins04 StoneTowerRuins05 StoneTowerRuins07 StoneTowerRuins08 StoneTowerRuins09 StoneTowerRuins10 Mistlands_GuardTower1_new Mistlands_GuardTower1_ruined_new Mistlands_GuardTower1_ruined_new2 Mistlands_GuardTower2_new Mistlands_GuardTower3_new Mistlands_GuardTower3_ruined_new Mistlands_Lighthouse1_new CharredTowerRuins1 CharredTowerRuins1_dvergr CharredTowerRuins2 CharredTowerRuins3");
        AddLocations(PoiCategory.Houses, "WoodHouse1 WoodHouse2 WoodHouse3 WoodHouse4 WoodHouse5 WoodHouse6 WoodHouse7 WoodHouse8 WoodHouse9 WoodHouse10 WoodHouse11 WoodHouse12 WoodHouse13 StoneHouse1 StoneHouse2 StoneHouse3 StoneHouse4 StoneHouse5 StoneHouse1_heath StoneHouse2_heath StoneHouse5_heath SwampHut1 SwampHut2 SwampHut3 SwampHut4 SwampHut5 AbandonedLogCabin02 AbandonedLogCabin03 AbandonedLogCabin04");
        AddLocations(PoiCategory.Tombs, "Grave1 MountainGrave01 Dolmen01 Dolmen02 Dolmen03");
        // Crypt2/3/4 are the Black Forest burial chamber locations; SunkenCrypt is a swamp crypt.
        AddLocations(PoiCategory.Chambers, "Crypt2 Crypt3 Crypt4");
        AddLocations(PoiCategory.Crypts, "SunkenCrypt1 SunkenCrypt2 SunkenCrypt3 SunkenCrypt4 Hildir_crypt");
        AddLocations(PoiCategory.Caves, "TrollCave TrollCave02 MountainCave01 MountainCave02 Hildir_cave MorgenHole1 MorgenHole2 MorgenHole3");
        AddLocations(PoiCategory.Mines, "Mistlands_DvergrTownEntrance1 Mistlands_DvergrTownEntrance2");
        AddLocations(PoiCategory.Villages, "WoodVillage1");
        AddLocations(PoiCategory.TarPits, "TarPit1 TarPit2 TarPit3");
        AddLocations(PoiCategory.BossAltars, "Eikthyrnir GDKing Bonemass Dragonqueen GoblinKing FaderLocation Mistlands_DvergrBossEntrance1");
        AddLocations(PoiCategory.Camps, "Greydwarf_camp1 Greydwarf_camp2 Greydwarf_camp3 GoblinCamp1 GoblinCamp2");
        AddLocations(PoiCategory.Ruins, "SwampRuin1 SwampRuin2 Ruin1 Ruin2 Ruin3 CharredRuins1 CharredRuins2 CharredRuins3 CharredRuins4 AshlandRuins");
        AddLocations(PoiCategory.Fortresses, "Castle Fort1 CharredFortress FortressRuins Hildir_plainsfortress");
        AddLocations(PoiCategory.Farms, "WoodFarm1");
        AddLocations(PoiCategory.Merchants, "Vendor_BlackForest Hildir_camp");
        AddLocations(PoiCategory.Runestones, "Runestone_Greydwarfs Runestone_Draugr Runestone_Meadows Runestone_Boars Runestone_Swamps Runestone_Mountains Runestone_BlackForest Runestone_Plains Runestone_Mistlands Runestone_Ashlands DrakeLorestone");
        AddLocations(PoiCategory.Wells, "SwampWell1 MountainWell1");
        AddLocations(PoiCategory.Shipwrecks, "ShipWreck01 ShipWreck02 ShipWreck03 ShipWreck04");
        AddLocations(PoiCategory.StoneCircles, "StoneCircle StoneHenge1 StoneHenge2 StoneHenge3 StoneHenge4 StoneHenge5 StoneHenge6 ShipSetting01");
        AddLocations(PoiCategory.Landmarks, "StartTemple Waymarker01 Waymarker02 PlaceofMystery1 PlaceofMystery2 PlaceofMystery3 Mistlands_Harbour1 Mistlands_Viaduct1 Mistlands_Viaduct2 Mistlands_RockSpire1 Mistlands_Giant1 Mistlands_Giant2 Mistlands_RoadPost1 Mistlands_StatueGroup1 Mistlands_Statue1 Mistlands_Statue2 Mistlands_Swords1 Mistlands_Swords2 Mistlands_Swords3 LeviathanLava SulfurArch");
        AddLocations(PoiCategory.Nests, "DrakeNest01 VoltureNest");
        AddLocations(PoiCategory.Excavations, "Mistlands_Excavation1 Mistlands_Excavation2 Mistlands_Excavation3");
        AddLocations(PoiCategory.Spawners, "FireHole InfestedTree01 CharredStone_Spawner");

        AddObjects(PoiCategory.ResourceVeins, "MineRock_Copper MineRock_Tin MineRock_Iron MineRock_Obsidian MineRock_Meteorite MineRock_Stone rock4_copper silvervein mudpile mudpile2 FlametalRockstand");
        AddObjects(PoiCategory.BossVegvisirs, "Vegvisir_Bonemass Vegvisir_DNBoss Vegvisir_DragonQueen Vegvisir_Eikthyr Vegvisir_Fader Vegvisir_GDKing Vegvisir_GoblinKing Vegvisir_SeekerQueen Vegvisir_placeofmystery Vegvisir_placeofmystery_2 Vegvisir_placeofmystery_3");
    }

    private static void AddLocations(PoiCategory category, string names)
    {
        foreach (string name in names.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            Locations.Add(name, category);
    }

    private static void AddObjects(PoiCategory category, string names)
    {
        foreach (string name in names.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            Objects.Add(name.GetStableHashCode(), category);
    }

    internal static bool TryGetLocation(string name, out PoiCategory category) =>
        Locations.TryGetValue(name ?? string.Empty, out category);

    internal static bool TryGetObject(int prefabHash, out PoiCategory category) =>
        Objects.TryGetValue(prefabHash, out category);

    internal static string Label(PoiCategory category)
    {
        switch (category)
        {
            case PoiCategory.ResourceVeins: return "Resource vein";
            case PoiCategory.Caves: return "Cave";
            case PoiCategory.TarPits: return "Tar pit";
            case PoiCategory.BossAltars: return "Boss site";
            case PoiCategory.BossVegvisirs: return "Boss Vegvisir";
            case PoiCategory.StoneCircles: return "Stone circle";
            case PoiCategory.Excavations: return "Excavation site";
            default:
                string name = category.ToString();
                return name.EndsWith("s", StringComparison.Ordinal) ? name.Substring(0, name.Length - 1) : name;
        }
    }
}
