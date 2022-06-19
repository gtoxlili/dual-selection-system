using System;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using CustomFont.Properties;

namespace CustomFont
{
    public static class Font
    {
        public static PrivateFontCollection RPfc = new PrivateFontCollection();
        public static PrivateFontCollection BPfc = new PrivateFontCollection();

        static Font()
        {
            string path = Application.StartupPath + "//UserInfo.xml";
            if (!File.Exists(path))
            {
                SetFont("更纱黑体");
            }
            else
            {
                XmlDocument userInfo = new XmlDocument();
                userInfo.Load(path);
                XmlElement preferFont = (XmlElement)userInfo.DocumentElement?.GetElementsByTagName("PreferFont")[0];
                if (preferFont != null) SetFont(preferFont.InnerText);
            }
        }

        public static void SetFont(string fontName)
        {
            //更纱黑体
            //霞鹜文楷
            //寒蝉手拙

            byte[] rfontData;
            byte[] bfontData;

            switch (fontName)
            {
                case "更纱黑体":
                    rfontData = Resource.更纱黑体_regular;
                    bfontData = Resource.更纱黑体_bold;
                    break;
                case "东竹石体":
                    rfontData = Resource.东竹石体;
                    bfontData = Resource.东竹石体;
                    break;
                case "霞鹜文楷":
                    rfontData = Resource.霞鹜文楷_regular;
                    bfontData = Resource.霞鹜文楷_Bold;
                    break;
                default:
                    rfontData = Resource.悠哉字体_Regular;
                    bfontData = Resource.悠哉字体_Bold;
                    break;
            }

            unsafe
            {
                fixed (byte* rFontData = rfontData)
                {
                    fixed (byte* bFontData = bfontData)
                    {
                        uint rFonts = 0;
                        AddFontMemResourceEx((IntPtr)rFontData, (uint)rfontData.Length, IntPtr.Zero, ref rFonts);
                        uint bFonts = 0;
                        AddFontMemResourceEx((IntPtr)bFontData, (uint)bfontData.Length, IntPtr.Zero, ref bFonts);

                        RPfc.AddMemoryFont((IntPtr)rFontData, rfontData.Length);
                        BPfc.AddMemoryFont((IntPtr)bFontData, bfontData.Length);
                    }
                }
            }
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr
            AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
    }
}