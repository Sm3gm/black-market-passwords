using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using UnityEngine;

namespace SoDBlackMarketPasswords;

[BepInPlugin("ta.sod.blackmarketpasswords", "Black Market Passwords", Plugin.VER)]
public class Plugin : BasePlugin
{
    internal const string VER = "0.1.1";

    internal static BepInEx.Logging.ManualLogSource L;
    internal static ConfigEntry<KeyCode> PasswordKey;
    internal static ConfigEntry<KeyCode> PasswordModifier;
    internal static ConfigEntry<KeyCode> StampKey;
    internal static ConfigEntry<KeyCode> StampModifier;
    internal static ConfigEntry<string> Separator;

    // BepInEx trims config values, so spaces are added here rather than stored.
    internal static string Sep()
    {
        var s = (Separator.Value ?? "").Trim();
        if (s.Length == 0) return " ";
        if (s == "," || s == ";" || s == ":" || s == ".") return s + " ";
        return " " + s + " ";
    }
    internal static ConfigEntry<bool> IncludeDay;
    internal static ConfigEntry<bool> IncludeName;

    public override void Load()
    {
        L = Log;

        PasswordKey = Config.Bind("General", "PasswordKey", KeyCode.F8,
            "Press this with a board note open and focused to append the current "
          + "black market and weapons dealer passwords. Default is F8.");

        PasswordModifier = Config.Bind("General", "PasswordModifier", KeyCode.None,
            "Modifier held with PasswordKey. Default is None, which makes the "
          + "binding a single key. Left and right Shift, Control and Alt count "
          + "as the same modifier.");

        StampKey = Config.Bind("General", "StampKey", KeyCode.F8,
            "Press this with a board note open and focused to stamp the time and "
          + "place on it. Default is F8.");

        StampModifier = Config.Bind("General", "StampModifier", KeyCode.LeftShift,
            "Modifier held with StampKey. Default is LeftShift, so the stamp is "
          + "Shift plus F8. Set to None to make the binding a single key. Left "
          + "and right Shift, Control and Alt count as the same modifier.");

        Separator = Config.Bind("General", "Separator", ",",
            "Text placed between the parts of each line, for example between the label and the shop name. Spaces are added automatically: one after , ; : or . and one on each side of anything else.");

        IncludeDay = Config.Bind("General", "IncludeDay", true,
            "Prefix the time and place stamp with the day number.");

        IncludeName = Config.Bind("General", "IncludeName", true,
            "Append the name of the citizen you are interacting with, if any. "
          + "Records what you currently know, so an unidentified citizen "
          + "stamps as 'Unknown Citizen'.");

        AddComponent<StampBehaviour>();

        L.LogInfo("Black Market Passwords " + VER + " loaded. Passwords: "
            + Describe(PasswordModifier.Value, PasswordKey.Value)
            + ". Time and place stamp: "
            + Describe(StampModifier.Value, StampKey.Value) + ".");
    }

    static string Describe(KeyCode modifier, KeyCode key)
    {
        if (modifier == KeyCode.None) return key.ToString();
        return modifier.ToString() + " plus " + key.ToString();
    }
}

public class StampBehaviour : MonoBehaviour
{
    public StampBehaviour(System.IntPtr ptr) : base(ptr) { }

    void Update()
    {
        bool wantPasswords = Matches(Plugin.PasswordKey.Value, Plugin.PasswordModifier.Value);
        bool wantStamp = Matches(Plugin.StampKey.Value, Plugin.StampModifier.Value);

        // One press cannot do both. The binding that carries a modifier wins,
        // so a plain key and a modified key can share the same KeyCode.
        if (wantPasswords && wantStamp)
        {
            if (Plugin.PasswordModifier.Value == KeyCode.None
             && Plugin.StampModifier.Value != KeyCode.None) wantPasswords = false;
            else wantStamp = false;
        }

        if (!wantPasswords && !wantStamp) return;

        StickyNoteController note = FindFocusedNote();
        if (note == null)
        {
            Plugin.L.LogInfo("no focused note, nothing done");
            return;
        }

        string text;
        try { text = wantPasswords ? BuildPasswords() : BuildStamp(); }
        catch (System.Exception e)
        {
            Plugin.L.LogError("build failed: " + e.Message);
            return;
        }

        if (string.IsNullOrEmpty(text)) return;

        try
        {
            string existing = note.input.text;
            if (string.IsNullOrEmpty(existing)) note.input.text = text;
            else if (existing.EndsWith("\n")) note.input.text = existing + text;
            else note.input.text = existing + "\n" + text;
            Plugin.L.LogInfo("wrote: " + text);
        }
        catch (System.Exception e)
        {
            Plugin.L.LogError("write failed: " + e.Message);
        }
    }

    static bool Matches(KeyCode key, KeyCode modifier)
    {
        if (!Input.GetKeyDown(key)) return false;
        if (modifier == KeyCode.None) return true;
        if (Input.GetKey(modifier)) return true;
        KeyCode twin = Twin(modifier);
        return twin != KeyCode.None && Input.GetKey(twin);
    }

    static KeyCode Twin(KeyCode k)
    {
        switch (k)
        {
            case KeyCode.LeftShift: return KeyCode.RightShift;
            case KeyCode.RightShift: return KeyCode.LeftShift;
            case KeyCode.LeftControl: return KeyCode.RightControl;
            case KeyCode.RightControl: return KeyCode.LeftControl;
            case KeyCode.LeftAlt: return KeyCode.RightAlt;
            case KeyCode.RightAlt: return KeyCode.LeftAlt;
            default: return KeyCode.None;
        }
    }

    // Hold the controller, not the input field: the TMPro assembly is not
    // in refs, so TMP_InputField cannot be named here.
    static StickyNoteController FindFocusedNote()
    {
        try
        {
            var notes = Resources.FindObjectsOfTypeAll<StickyNoteController>();
            if (notes == null) return null;
            foreach (var n in notes)
            {
                if (n == null || n.input == null) continue;
                if (!n.gameObject.activeInHierarchy) continue;
                if (!n.input.isFocused) continue;
                return n;
            }
        }
        catch (System.Exception e)
        {
            Plugin.L.LogError("note lookup failed: " + e.Message);
        }
        return null;
    }

    static string BuildPasswords()
    {
        var city = CityData.Instance;
        if (city == null || city.addressDirectory == null)
        {
            Plugin.L.LogInfo("passwords: no city loaded, nothing done");
            return null;
        }

        string sep = Plugin.Sep();
        var blackMarket = new List<string>();
        var weapons = new List<string>();
        var other = new List<string>();

        foreach (var a in city.addressDirectory)
        {
            if (a == null || a.addressPreset == null) continue;
            if (!a.addressPreset.needsPassword) continue;

            string password;
            try { password = a.GetPassword(); }
            catch { continue; }
            if (string.IsNullOrEmpty(password)) continue;

            string preset = a.addressPreset.name;
            string label;
            if (preset == "BlackmarketTrader") label = "Black market";
            else if (preset == "WeaponsDealer") label = "Weapons";
            else label = preset;

            string line = label + sep + a.name + ": " + password;
            if (preset == "BlackmarketTrader") blackMarket.Add(line);
            else if (preset == "WeaponsDealer") weapons.Add(line);
            else other.Add(line);
        }

        if (blackMarket.Count == 0 && weapons.Count == 0 && other.Count == 0)
        {
            Plugin.L.LogInfo("passwords: none found, nothing done");
            return null;
        }

        var sb = new StringBuilder();
        foreach (var line in blackMarket) Append(sb, line);
        foreach (var line in weapons) Append(sb, line);
        foreach (var line in other) Append(sb, line);
        return sb.ToString();
    }

    static void Append(StringBuilder sb, string line)
    {
        if (sb.Length > 0) sb.Append('\n');
        sb.Append(line);
    }

    static string BuildStamp()
    {
        var sb = new StringBuilder();
        var s = SessionData.Instance;
        string sep = Plugin.Sep();

        if (Plugin.IncludeDay.Value && s != null)
        {
            try { sb.Append("Day ").Append(s.dayInt + 1).Append(", "); }
            catch { }
        }

        if (s != null)
        {
            try
            {
                float clock = s.decimalClock;
                int hh = (int)clock;
                int mm = (int)((clock - hh) * 60f);
                if (hh < 10) sb.Append('0');
                sb.Append(hh).Append(':');
                if (mm < 10) sb.Append('0');
                sb.Append(mm);
            }
            catch { sb.Append("??:??"); }
        }

        var p = Player.Instance;

        try
        {
            if (p != null && p.currentGameLocation != null)
                sb.Append(sep).Append(p.currentGameLocation.name);
        }
        catch { }

        if (Plugin.IncludeName.Value)
        {
            try
            {
                if (p != null)
                {
                    var iw = p.interactingWith;
                    if (iw != null)
                    {
                        string n = iw.name;
                        if (!string.IsNullOrEmpty(n)) sb.Append(sep).Append(n);
                    }
                }
            }
            catch { }
        }

        return sb.ToString();
    }
}
