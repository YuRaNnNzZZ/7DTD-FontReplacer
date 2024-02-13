using System.IO;
using System.Xml;
using UnityEngine;

namespace FontReplacer
{
    public struct FontData
    {
        public Font font { get; }
        public int size { get; }

        public FontData(string bundlePath, string assetName, int size)
        {
            font = null;
            this.size = size;

            bundlePath = Path.GetFullPath(bundlePath);
            if (File.Exists(bundlePath))
            {
                Log.Out("[Font Replacement] Loading font '" + assetName + "' from '" + bundlePath + "'");
                font = DataLoader.LoadAsset<Font>("#" + bundlePath + "?" + assetName);
            }
            else
            {
                Log.Error("[Font Replacement] File '" + bundlePath + "' not found in mod folder!");
            }
        }
    }

    public class FontConfig
    {
        public static FontData MainFont;
        public static FontData ConsoleFont;

        public static void Load(Mod mod)
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
                        string fontBundlePath = bundlePathNode?.Value?.ToString();

                        XmlNode nameNode = xnode.Attributes.GetNamedItem("Name");
                        string fontName = nameNode?.Value?.ToString();

                        MainFont = new FontData(mod.Path + "/" + fontBundlePath, fontName, 0);
                    }

                    if (xnode.Name == "ConsoleFont")
                    {
                        XmlNode bundlePathNode = xnode.Attributes.GetNamedItem("BundlePath");
                        string fontBundlePath = bundlePathNode?.Value?.ToString();

                        XmlNode nameNode = xnode.Attributes.GetNamedItem("Name");
                        string fontName = nameNode?.Value?.ToString();

                        XmlNode sizeNode = xnode.Attributes.GetNamedItem("Size");
                        string fontSizeStr = sizeNode?.Value?.ToString();

                        int.TryParse(fontSizeStr, out int fontSize);
                        ConsoleFont = new FontData(mod.Path + "/" + fontBundlePath, fontName, fontSize);
                    }
                }
            }
        }

    }
}
