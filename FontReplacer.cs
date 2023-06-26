using System.IO;
using System.Reflection;
using System.Xml;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

public class FontReplacer : IModApi
{
    public static string modFullPath;

    public static bool loaded = false;
    public static string fontBundlePath;
    public static string fontName;

    public static bool loaded2 = false;
    public static string consoleFontBundlePath;
    public static string consoleFontName;

    public void InitMod(Mod mod)
    {
        var harmony = new Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        modFullPath = mod.Path;
        LoadFontConfig(mod);
    }

    public void LoadFontConfig(Mod mod)
    {
        string configFileName = mod.Path + "/FontInfo.xml";
        if (!File.Exists(configFileName))
        {
            Log.Error("[Font Replacement] No FontInfo.xml config found in mod folder!");

            return;
        }

        XmlDocument xDoc = new XmlDocument();
        xDoc.Load(configFileName);

        XmlElement xRoot = xDoc.DocumentElement;
        if (xRoot != null)
        {
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Name == "Font")
                {
                    XmlNode bundlePathNode = xnode.Attributes.GetNamedItem("BundlePath");
                    fontBundlePath = bundlePathNode?.Value?.ToString();

                    XmlNode nameNode = xnode.Attributes.GetNamedItem("Name");
                    fontName = nameNode?.Value?.ToString();
                }

                if (xnode.Name == "ConsoleFont")
                {
                    XmlNode bundlePathNode = xnode.Attributes.GetNamedItem("BundlePath");
                    consoleFontBundlePath = bundlePathNode?.Value?.ToString();

                    XmlNode nameNode = xnode.Attributes.GetNamedItem("Name");
                    consoleFontName = nameNode?.Value?.ToString();
                }
            }
        }

        loaded = fontBundlePath != null && fontName != null;
        loaded2 = consoleFontBundlePath != null && consoleFontName != null;
    }

    [HarmonyPatch(typeof(XUi), nameof(XUi.GetUIFontByName))]
    public class ReplaceReferenceFont
    {
        private static Font newFont;

        public static NGUIFont Postfix(NGUIFont __result, string _name, bool _showWarning)
        {
            if (loaded && _name == "ReferenceFont" && __result != null)
            {
                if (newFont == null)
                {
                    string fullBundlePath = modFullPath + "/" + fontBundlePath;

                    if (!File.Exists(fullBundlePath))
                    {
                        Log.Error("[Font Replacement] File '" + fontBundlePath + "' not found in mod folder!");

                        loaded = false;
                        return __result;
                    }

                    Log.Out("[Font Replacement] Loading replacement font '" + fontName + "' from '" + fullBundlePath + "'");
                    newFont = DataLoader.LoadAsset<Font>("#" + fullBundlePath + "?" + fontName);
                }

                if (__result.dynamicFont != newFont)
                {
                    __result.dynamicFont = newFont;
                }
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(GUIWindowConsole), "AllocText")]
    public class ReplaceConsoleFont
    {
        private static Font newFont;

        public static Text Postfix(Text __result)
        {
            if (!loaded2) return __result;

            if (newFont == null)
            {
                string fullBundlePath = modFullPath + "/" + consoleFontBundlePath;

                if (!File.Exists(fullBundlePath))
                {
                    Log.Error("[Font Replacement] File '" + consoleFontBundlePath + "' not found in mod folder!");
                    loaded2 = false;
                    return __result;
                }

                Log.Out("[Font Replacement] Loading console replacement font '" + consoleFontName + "' from '" + fullBundlePath + "'");
                newFont = DataLoader.LoadAsset<Font>("#" + fullBundlePath + "?" + consoleFontName);
            }

            if (__result != null && __result.font != newFont)
            {
                __result.font = newFont;
            }

            return __result;
        }
    }
}
