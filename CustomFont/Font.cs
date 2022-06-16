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
        public static PrivateFontCollection rPFC = new PrivateFontCollection();
        public static PrivateFontCollection bPFC = new PrivateFontCollection();

        static Font()
        {
            string path = Application.StartupPath + "//UserInfo.xml";
            if (!File.Exists(path))
                SetFont("更纱黑体");
            else
            {
                XmlDocument _userInfo = new XmlDocument();
                _userInfo.Load(path);
                XmlElement PreferFont = (XmlElement)_userInfo.DocumentElement?.GetElementsByTagName("PreferFont")[0];
                SetFont(PreferFont.InnerText);
            }
        }

        public static void SetFont(string fontName)
        {
            //更纱黑体
            //霞鹜文楷
            //寒蝉手拙

            byte[] rfontData;
            byte[] bfontData;

            if (fontName == "更纱黑体")
            {
                rfontData = Resource.更纱黑体_regular;
                bfontData = Resource.更纱黑体_bold;
            }
            else if (fontName == "东竹石体")
            {
                rfontData = Resource.东竹石体;
                bfontData = Resource.东竹石体;
            }else if (fontName == "霞鹜文楷")
            {
                rfontData = Resource.霞鹜文楷_regular;
                bfontData = Resource.霞鹜文楷_Bold;
            }
            else
            {
                rfontData = Resource.悠哉字体_Regular;
                bfontData = Resource.悠哉字体_Bold;
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

                        rPFC.AddMemoryFont((IntPtr)rFontData, rfontData.Length);
                        bPFC.AddMemoryFont((IntPtr)bFontData, bfontData.Length);
                    }
                }
            }

        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr
            AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
    }
}