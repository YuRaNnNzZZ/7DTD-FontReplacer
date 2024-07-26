using System.IO;
using System.Reflection;
using System.Xml;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

public class FontReplacer : IModApi
{
    public static string modFullPath;

    public static Font mainFont = null;
    public static Font consoleFont = null;

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
                    if (mainFont == null)
                    {
                        mainFont = TryParseAndLoadFont(xnode);
                    }

                    continue;
                }

                if (xnode.Name == "ConsoleFont")
                {
                    if (consoleFont == null)
                    {
                        consoleFont = TryParseAndLoadFont(xnode);
                    }

                    continue;
                }
            }
        }
    }

    private Font TryParseAndLoadFont(XmlNode xnode)
    {
        XmlNode bundlePathNode = xnode.Attributes.GetNamedItem("BundlePath");
        string fontBundlePath = bundlePathNode?.Value?.ToString();

        XmlNode nameNode = xnode.Attributes.GetNamedItem("Name");
        string fontName = nameNode?.Value?.ToString();

        string fullBundlePath = modFullPath + "/" + fontBundlePath;

        if (!File.Exists(fullBundlePath))
        {
            Log.Error("[Font Replacement][" + xnode.Name + "] File '" + fontBundlePath + "' not found in mod folder!");

            return null;
        }

        Log.Out("[Font Replacement][" + xnode.Name + "] Loading replacement font '" + fontName + "' from '" + fullBundlePath + "'");
        return DataLoader.LoadAsset<Font>("#" + fullBundlePath + "?" + fontName);
    }

    [HarmonyPatch(typeof(XUi), nameof(XUi.GetUIFontByName))]
    public class ReplaceReferenceFont
    {
        public static NGUIFont Postfix(NGUIFont __result, string _name)
        {
            if (mainFont != null && __result != null && _name == "ReferenceFont" && __result.dynamicFont != mainFont)
            {
                __result.dynamicFont = mainFont;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(GUIWindowConsole), "AllocText")]
    public class ReplaceConsoleFont
    {
        public static Text Postfix(Text __result)
        {
            if (consoleFont != null && __result != null && __result.font != consoleFont)
            {
                __result.font = consoleFont;
            }

            return __result;
        }
    }
}
