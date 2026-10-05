using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace DadsAdventureMapping;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("valheim.exe")]
public sealed class DadsAdventureMappingPlugin : BaseUnityPlugin
{
    public const string Guid = "com.dadisbored.dadsadventuremapping";
    public const string Name = "DadsAdventureMapping";
    public const string Version = "1.0.2";
    internal const float MarkerRange = 25f;

    internal static ConfigEntry<float> RevealMultiplier = null!;
    private readonly Dictionary<PoiCategory, ConfigEntry<bool>> _markers = new();

    private Harmony? _harmony;
    private float _nextScan;

    private void Awake()
    {
        RevealMultiplier = Config.Bind("1 - Map Reveal", "Reveal Size Multiplier", 2f,
            new ConfigDescription("Map area revealed as the player moves, from 2x to 5x the normal radius.",
                new AcceptableValueRange<float>(2f, 5f)));
        Bind(PoiCategory.Towers, "Towers", true);
        Bind(PoiCategory.Houses, "Houses", false);
        Bind(PoiCategory.ResourceVeins, "Resource Veins", true);
        Bind(PoiCategory.Tombs, "Tombs", true);
        Bind(PoiCategory.Chambers, "Chambers", true);
        Bind(PoiCategory.Crypts, "Crypts", true);
        Bind(PoiCategory.Caves, "Caves and Caverns", true);
        Bind(PoiCategory.Mines, "Mines", true);
        Bind(PoiCategory.Villages, "Villages", false);
        Bind(PoiCategory.TarPits, "Tar Pits", false);
        Bind(PoiCategory.BossAltars, "Boss Sites", false);
        Bind(PoiCategory.BossVegvisirs, "Boss Vegvisirs", false);
        Bind(PoiCategory.Camps, "Camps", false);
        Bind(PoiCategory.Ruins, "Ruins", false);
        Bind(PoiCategory.Fortresses, "Fortresses", false);
        Bind(PoiCategory.Farms, "Farms", false);
        Bind(PoiCategory.Merchants, "Merchants", false);
        Bind(PoiCategory.Runestones, "Runestones", false);
        Bind(PoiCategory.Wells, "Wells", false);
        Bind(PoiCategory.Shipwrecks, "Shipwrecks", false);
        Bind(PoiCategory.StoneCircles, "Stone Circles", false);
        Bind(PoiCategory.Landmarks, "Landmarks", false);
        Bind(PoiCategory.Nests, "Nests", false);
        Bind(PoiCategory.Excavations, "Excavation Sites", false);
        Bind(PoiCategory.Spawners, "Spawners", false);

        _harmony = new Harmony(Guid);
        _harmony.PatchAll(typeof(DadsAdventureMappingPlugin).Assembly);
    }

    private void Bind(PoiCategory category, string name, bool enabled)
    {
        _markers.Add(category, Config.Bind("2 - Auto Markers", name, enabled,
            "Place a map pin for this category after coming within 25 meters of a matching prefab."));
    }

    private void Update()
    {
        if (Time.time < _nextScan) return;
        _nextScan = Time.time + 2f;

        Player player = Player.m_localPlayer;
        Minimap map = Minimap.instance;
        ZoneSystem zones = ZoneSystem.instance;
        ZDOMan zdoMan = ZDOMan.instance;
        if (player == null || map == null || zones == null || zdoMan == null) return;

        Vector3 position = player.transform.position;
        // On a client GetLocationList contains the streamed nearby locations. On a local
        // server it can contain more; TryMark still applies the 25-meter limit.
        foreach (ZoneSystem.LocationInstance location in zones.GetLocationList())
        {
            if (location.m_location != null &&
                PoiCatalog.TryGetLocation(location.m_location.m_prefabName, out PoiCategory category))
                TryMark(map, position, location.m_position, category);
        }

        // LocationProxy ZDOs cover locations whose Location component is inactive or absent.
        // Resource deposits and Vegvisirs are networked objects rather than zone locations.
        foreach (ZDO zdo in zdoMan.m_objectsByID.Values)
        {
            if (zdo == null || !zdo.IsValid()) continue;
            Vector3 target = zdo.GetPosition();
            if ((position - target).sqrMagnitude > MarkerRange * MarkerRange) continue;
            int prefab = zdo.GetPrefab();
            if (prefab == PoiCatalog.LocationProxyHash)
            {
                int locationHash = zdo.GetInt(ZDOVars.s_location);
                if (zones.m_locationsByHash.TryGetValue(locationHash, out ZoneSystem.ZoneLocation location) &&
                    PoiCatalog.TryGetLocation(location.m_prefabName, out PoiCategory category))
                    TryMark(map, position, target, category);
            }
            else if (PoiCatalog.TryGetObject(prefab, out PoiCategory category))
                TryMark(map, position, target, category);
        }

        // Some placed prefabs can be active in the scene before their ZDO is available.
        foreach (MineRock rock in FindObjectsByType<MineRock>(FindObjectsSortMode.None))
            TryMarkComponent(map, position, rock);
        foreach (MineRock5 rock in FindObjectsByType<MineRock5>(FindObjectsSortMode.None))
            TryMarkComponent(map, position, rock);
        foreach (Vegvisir vegvisir in FindObjectsByType<Vegvisir>(FindObjectsSortMode.None))
            TryMarkComponent(map, position, vegvisir);
    }

    private void TryMarkComponent(Minimap map, Vector3 player, Component component)
    {
        if (component == null) return;
        Vector3 target = component.transform.position;
        if ((player - target).sqrMagnitude > MarkerRange * MarkerRange) return;
        string name = Utils.GetPrefabName(component.gameObject);
        if (PoiCatalog.TryGetObject(name.GetStableHashCode(), out PoiCategory category))
            TryMark(map, player, target, category);
    }

    private void TryMark(Minimap map, Vector3 player, Vector3 target, PoiCategory category)
    {
        if ((player - target).sqrMagnitude > MarkerRange * MarkerRange) return;
        if (!_markers[category].Value) return;

        // Existing native pins survive reloads; checking them also prevents marking a user-pinned site twice.
        foreach (Minimap.PinData pin in map.m_pins)
        {
            float px = pin.m_pos.x - target.x;
            float pz = pin.m_pos.z - target.z;
            if (px * px + pz * pz < 16f) return;
        }
        Minimap.PinType icon = category == PoiCategory.ResourceVeins ? Minimap.PinType.Icon2 : Minimap.PinType.Icon3;
        map.AddPin(target, icon, PoiCatalog.Label(category), true, false, Player.m_localPlayer.GetPlayerID());
    }

    private void OnDestroy() => _harmony?.UnpatchSelf();
}

[HarmonyPatch(typeof(Minimap), "Explore", new[] { typeof(Vector3), typeof(float) })]
internal static class MovingExplorePatch
{
    private static void Prefix(Vector3 __0, ref float __1)
    {
        Player player = Player.m_localPlayer;
        if (player == null) return;
        Vector3 current = player.transform.position;
        float dx = current.x - __0.x;
        float dz = current.z - __0.z;
        if (dx * dx + dz * dz <= 1f)
            __1 *= Mathf.Clamp(DadsAdventureMappingPlugin.RevealMultiplier.Value, 2f, 5f);
    }
}
