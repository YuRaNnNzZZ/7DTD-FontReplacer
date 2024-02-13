using System.Reflection;
using HarmonyLib;

namespace FontReplacer
{
    public class FontReplacer : IModApi
    {
        public void InitMod(Mod mod)
        {
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            FontConfig.Load(mod);
        }
    }
}
