using System;
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
    public const string Version = "1.0.1";
    internal const float MarkerRange = 25f;

    internal static ConfigEntry<float> RevealMultiplier = null!;
    internal static ConfigEntry<bool> Towers = null!;
    internal static ConfigEntry<bool> Houses = null!;
    internal static ConfigEntry<bool> ResourceVeins = null!;
    internal static ConfigEntry<bool> Tombs = null!;
    internal static ConfigEntry<bool> Chambers = null!;
    internal static ConfigEntry<bool> Crypts = null!;
    internal static ConfigEntry<bool> Mines = null!;

    private Harmony? _harmony;
    private float _nextScan;

    private void Awake()
    {
        RevealMultiplier = Config.Bind("1 - Map Reveal", "Reveal Size Multiplier", 2f,
            new ConfigDescription("Map area revealed as the player moves, from 2x to 5x the normal radius.",
                new AcceptableValueRange<float>(2f, 5f)));
        Towers = Config.Bind("2 - Auto Markers", "Towers", true, "Mark towers after coming within 25 meters.");
        Houses = Config.Bind("2 - Auto Markers", "Houses", true, "Mark houses after coming within 25 meters.");
        ResourceVeins = Config.Bind("2 - Auto Markers", "Resource Veins", true, "Mark ore deposits after coming within 25 meters.");
        Tombs = Config.Bind("2 - Auto Markers", "Tombs", true, "Mark tombs and graves after coming within 25 meters.");
        Chambers = Config.Bind("2 - Auto Markers", "Chambers", true, "Mark burial chambers and caves after coming within 25 meters.");
        Crypts = Config.Bind("2 - Auto Markers", "Crypts", true, "Mark crypts after coming within 25 meters.");
        Mines = Config.Bind("2 - Auto Markers", "Mines", true, "Mark mines after coming within 25 meters.");

        _harmony = new Harmony(Guid);
        _harmony.PatchAll(typeof(DadsAdventureMappingPlugin).Assembly);
    }

    private void Update()
    {
        if (Time.time < _nextScan) return;
        _nextScan = Time.time + 1f;

        Player player = Player.m_localPlayer;
        Minimap map = Minimap.instance;
        if (player == null || map == null || ZoneSystem.instance == null) return;

        Vector3 position = player.transform.position;
        foreach (Location location in FindObjectsByType<Location>(FindObjectsSortMode.None))
        {
            if (location == null) continue;
            GameObject root = location.gameObject;
            TryMark(map, position, root.transform.position, Utils.GetPrefabName(root), false);
        }

        if (!ResourceVeins.Value) return;
        foreach (MineRock rock in FindObjectsByType<MineRock>(FindObjectsSortMode.None))
        {
            if (rock != null) TryMark(map, position, rock.transform.position, Utils.GetPrefabName(rock.gameObject), true);
        }
        foreach (MineRock5 rock in FindObjectsByType<MineRock5>(FindObjectsSortMode.None))
        {
            if (rock != null) TryMark(map, position, rock.transform.position, Utils.GetPrefabName(rock.gameObject), true);
        }
    }

    private static void TryMark(Minimap map, Vector3 player, Vector3 target, string prefab, bool resource)
    {
        if ((player - target).sqrMagnitude > MarkerRange * MarkerRange) return;
        if (!TryClassify(prefab, resource, out string label, out Minimap.PinType icon)) return;

        // Existing native pins survive reloads; checking them also prevents marking a user-pinned site twice.
        foreach (Minimap.PinData pin in map.m_pins)
        {
            float px = pin.m_pos.x - target.x;
            float pz = pin.m_pos.z - target.z;
            if (px * px + pz * pz < 16f) return;
        }
        map.AddPin(target, icon, label, true, false, Player.m_localPlayer.GetPlayerID());
    }

    private static bool TryClassify(string prefab, bool resource, out string label, out Minimap.PinType icon)
    {
        label = string.Empty;
        icon = Minimap.PinType.Icon3;
        if (string.IsNullOrEmpty(prefab)) return false;
        string name = prefab.ToLowerInvariant();

        if (resource)
        {
            if (!ResourceVeins.Value ||
                !(Has(name, "ore", "copper", "tin", "silver", "mudpile", "scrap", "flametal"))) return false;
            label = "Resource vein";
            icon = Minimap.PinType.Icon2;
            return true;
        }

        if (Has(name, "crypt")) { if (!Crypts.Value) return false; label = "Crypt"; }
        else if (Has(name, "mine", "mineshaft")) { if (!Mines.Value) return false; label = "Mine"; }
        else if (Has(name, "burial", "chamber", "cave")) { if (!Chambers.Value) return false; label = "Chamber"; }
        else if (Has(name, "tomb", "grave", "dolmen")) { if (!Tombs.Value) return false; label = "Tomb"; }
        else if (Has(name, "tower", "watchpost")) { if (!Towers.Value) return false; label = "Tower"; }
        else if (Has(name, "house", "hut", "cabin", "farm", "longhouse")) { if (!Houses.Value) return false; label = "House"; }
        else return false;

        icon = Minimap.PinType.Icon3;
        return true;
    }

    private static bool Has(string name, params string[] fragments)
    {
        foreach (string fragment in fragments)
            if (name.Contains(fragment)) return true;
        return false;
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
