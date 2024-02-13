using HarmonyLib;
using UnityEngine;

namespace FontReplacer.Patches
{
    [HarmonyPatch(typeof(GUIWindowConsole), "OnGUI")]
    public class ConsoleFont
    {
        private static void Prefix(GUIStyle ___labelStyle)
        {
            if (___labelStyle != null && FontConfig.ConsoleFont.font != null && ___labelStyle.font != FontConfig.ConsoleFont.font)
            {
                ___labelStyle.font = FontConfig.ConsoleFont.font;

                if (FontConfig.ConsoleFont.size > 0)
                {
                    ___labelStyle.fontSize = FontConfig.ConsoleFont.size;
                }
            }
        }
    }
}
