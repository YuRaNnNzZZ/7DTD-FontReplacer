using HarmonyLib;

namespace FontReplacer.Patches
{
    [HarmonyPatch(typeof(XUi), "GetUIFontByName")]
    public class ReferenceFont
    {
        private static void Postfix(NGUIFont __result, string _name)
        {
            if (_name == "ReferenceFont" && FontConfig.MainFont.font != null && __result != null && __result.dynamicFont != FontConfig.MainFont.font)
            {
                __result.dynamicFont = FontConfig.MainFont.font;
            }
        }
    }
}
