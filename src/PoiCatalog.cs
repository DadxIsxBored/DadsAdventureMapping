using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DadsAdventureMapping;

internal enum PoiCategory
{
    Towers, Houses, ResourceVeins, Tombs, Dolmens, Crypts, Caves, Mines,
    HildirLocations,
    Villages, TarPits, BossAltars, BossVegvisirs, Camps, Ruins, Fortresses,
    Farms, Merchants, Runestones, Wells, Shipwrecks, StoneCircles, Landmarks,
    Nests, Excavations, Spawners
}

// Names below come from the Valheim 1.0.16 _ZoneSystem and _LocationList
// prefabs in valheim_Data/StreamingAssets/SoftRef/Bundles, plus object prefab
// paths in manifest_extended. Only actual prefab names are accepted.
internal sealed class PoiDefinition
{
    internal readonly string Key;
    internal readonly string SettingName;
    internal readonly string PinLabel;
    internal readonly PoiCategory Category;
    internal readonly bool DefaultEnabled;
    internal readonly List<string> PrefabNames = new();

    internal PoiDefinition(string key, PoiCategory category)
    {
        Key = key;
        Category = category;
        SettingName = PoiCatalog.DisplayName(key);
        PinLabel = PoiCatalog.PinLabel(key);
        DefaultEnabled = category == PoiCategory.Towers || category == PoiCategory.ResourceVeins ||
            category == PoiCategory.Tombs || category == PoiCategory.Crypts ||
            category == PoiCategory.Caves ||
            category == PoiCategory.Mines;
    }
}

internal static class PoiCatalog
{
    internal static readonly int LocationProxyHash = "LocationProxy".GetStableHashCode();
    private static readonly Dictionary<string, PoiDefinition> Locations = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<int, PoiDefinition> Objects = new();
    private static readonly Dictionary<string, PoiDefinition> Families = new(StringComparer.OrdinalIgnoreCase);
    internal static readonly List<PoiDefinition> Definitions = new();

    static PoiCatalog()
    {
        AddLocations(PoiCategory.Towers, "StoneTower1 StoneTower2 StoneTower3 StoneTower4 StoneTowerRuins03 StoneTowerRuins04 StoneTowerRuins05 StoneTowerRuins07 StoneTowerRuins08 StoneTowerRuins09 StoneTowerRuins10 Mistlands_GuardTower1_new Mistlands_GuardTower1_ruined_new Mistlands_GuardTower1_ruined_new2 Mistlands_GuardTower2_new Mistlands_GuardTower3_new Mistlands_GuardTower3_ruined_new Mistlands_Lighthouse1_new CharredTowerRuins1 CharredTowerRuins1_dvergr CharredTowerRuins2 CharredTowerRuins3");
        AddLocations(PoiCategory.Houses, "WoodHouse1 WoodHouse2 WoodHouse3 WoodHouse4 WoodHouse5 WoodHouse6 WoodHouse7 WoodHouse8 WoodHouse9 WoodHouse10 WoodHouse11 WoodHouse12 WoodHouse13 StoneHouse1 StoneHouse2 StoneHouse3 StoneHouse4 StoneHouse5 StoneHouse1_heath StoneHouse2_heath StoneHouse5_heath SwampHut1 SwampHut2 SwampHut3 SwampHut4 SwampHut5 AbandonedLogCabin02 AbandonedLogCabin03 AbandonedLogCabin04");
        AddLocations(PoiCategory.Tombs, "Grave1 MountainGrave01");
        AddLocations(PoiCategory.Dolmens, "Dolmen01 Dolmen02 Dolmen03");
        // Crypt2/3/4 are Black Forest burial chambers and share the Tomb switch.
        AddLocations(PoiCategory.Tombs, "Crypt2 Crypt3 Crypt4");
        AddLocations(PoiCategory.Crypts, "SunkenCrypt1 SunkenCrypt2 SunkenCrypt3 SunkenCrypt4");
        AddLocations(PoiCategory.Caves, "TrollCave TrollCave02 MountainCave01 MountainCave02 MorgenHole1 MorgenHole2 MorgenHole3");
        AddLocations(PoiCategory.HildirLocations, "Hildir_cave Hildir_crypt Hildir_plainsfortress");
        AddLocations(PoiCategory.Mines, "Mistlands_DvergrTownEntrance1 Mistlands_DvergrTownEntrance2");
        AddLocations(PoiCategory.Villages, "WoodVillage1");
        AddLocations(PoiCategory.TarPits, "TarPit1 TarPit2 TarPit3");
        AddLocations(PoiCategory.BossAltars, "Eikthyrnir GDKing Bonemass Dragonqueen GoblinKing FaderLocation Mistlands_DvergrBossEntrance1");
        AddLocations(PoiCategory.Camps, "Greydwarf_camp1 Greydwarf_camp2 Greydwarf_camp3 GoblinCamp1 GoblinCamp2");
        AddLocations(PoiCategory.Ruins, "SwampRuin1 SwampRuin2 Ruin1 Ruin2 Ruin3 CharredRuins1 CharredRuins2 CharredRuins3 CharredRuins4 AshlandRuins");
        AddLocations(PoiCategory.Fortresses, "Castle Fort1 CharredFortress FortressRuins");
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
        AddObjects(PoiCategory.Merchants, "Haldor Hildir BogWitch BogWitch_Hut");
    }

    private static void AddLocations(PoiCategory category, string names)
    {
        foreach (string name in names.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        {
            PoiDefinition definition = GetDefinition(name, category);
            Locations.Add(name, definition);
            definition.PrefabNames.Add(name);
        }
    }

    private static void AddObjects(PoiCategory category, string names)
    {
        foreach (string name in names.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        {
            PoiDefinition definition = GetDefinition(name, category);
            Objects.Add(name.GetStableHashCode(), definition);
            definition.PrefabNames.Add(name);
        }
    }

    private static PoiDefinition GetDefinition(string name, PoiCategory category)
    {
        string key = FamilyKey(name, category);
        if (!Families.TryGetValue(key, out PoiDefinition definition))
        {
            definition = new PoiDefinition(key, category);
            Families.Add(key, definition);
            Definitions.Add(definition);
        }
        else if (definition.Category != category)
            throw new InvalidOperationException("POI family has conflicting categories: " + key);
        return definition;
    }

    private static string FamilyKey(string name, PoiCategory category)
    {
        switch (category)
        {
            case PoiCategory.Towers: return "Tower";
            case PoiCategory.Houses: return "House";
            case PoiCategory.Tombs: return "Tomb";
            case PoiCategory.Crypts: return "Crypt";
            case PoiCategory.Caves: return "Cave";
            case PoiCategory.Mines: return "Mine";
            case PoiCategory.HildirLocations: return "Hildir Locations";
            case PoiCategory.BossVegvisirs: return "Vegvisir";
            case PoiCategory.Villages: return "Village";
            case PoiCategory.TarPits: return "Tar Pit";
            case PoiCategory.BossAltars: return "Boss Site";
            case PoiCategory.Camps: return "Camp";
            case PoiCategory.Ruins: return "Ruin";
            case PoiCategory.Fortresses: return "Fortress";
            case PoiCategory.Farms: return "Farm";
            case PoiCategory.Runestones: return "Runestone";
            case PoiCategory.Wells: return "Well";
            case PoiCategory.Shipwrecks: return "Shipwreck";
            case PoiCategory.StoneCircles: return "Stone Circle";
            case PoiCategory.Nests: return "Nest";
            case PoiCategory.Excavations: return "Excavation";
            case PoiCategory.Spawners: return "Spawner";
            case PoiCategory.Merchants:
                if (name == "Vendor_BlackForest" || name == "Haldor") return "Haldor";
                if (name == "Hildir_camp" || name == "Hildir") return "Hildir";
                return "Bog Witch";
            case PoiCategory.ResourceVeins:
                if (name == "rock4_copper" || name == "MineRock_Copper") return "Copper";
                if (name == "MineRock_Tin") return "Tin";
                if (name == "MineRock_Iron" || name == "mudpile" || name == "mudpile2") return "Iron";
                if (name == "silvervein") return "Silver";
                if (name == "FlametalRockstand") return "Flametal";
                return name.Replace("MineRock_", string.Empty);
        }
        // Other numbered variants of the same prefab stem share a setting.
        return Regex.Replace(name, "[0-9]+", string.Empty).TrimEnd('_');
    }

    internal static bool TryGetLocation(string name, out PoiDefinition definition) =>
        Locations.TryGetValue(name ?? string.Empty, out definition);

    internal static bool TryGetObject(int prefabHash, out PoiDefinition definition) =>
        Objects.TryGetValue(prefabHash, out definition);

    private static string Readable(string key) =>
        Regex.Replace(key.Replace('_', ' '), "([a-z])([A-Z])", "$1 $2");

    internal static string DisplayName(string key)
    {
        switch (key)
        {
            case "GDKing": return "Elder Altar";
            case "Eikthyrnir": return "Eikthyr Altar";
            case "Dragonqueen": return "Moder Altar";
            case "GoblinKing": return "Yagluth Altar";
            case "Mistlands_DvergrBossEntrance": return "Queen Entrance";
            default: return Readable(key);
        }
    }

    internal static string PinLabel(string key) => DisplayName(key);
}
