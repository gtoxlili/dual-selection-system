using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Bll
{
    public class Aes
    {
        public static string GetPlaintext(string ciphertext)
        {
            byte[] aesKey = Encoding.UTF8.GetBytes("一念通天神魔无惧");
            return Tools.Tools.AesDecrypt(ciphertext, aesKey);
        }

        public static string GetCiphertext(string plaintext)
        {
            byte[] aesKey = Encoding.UTF8.GetBytes("一念通天神魔无惧");
            return Tools.Tools.AesEncrypt(plaintext, aesKey);
        }
    }

    public class Tool
    {
        public static void Close(Control con)
        {
            List<Control> ctrls = new List<Control>();

            // _tea_unSelectList.ForEach(i => unSelectListTemp.Add(i));

            for (int i = 0; i < con.Controls.Count; i++) ctrls.Add(con.Controls[i]);

            con.Controls.Clear();

            foreach (Control c in ctrls)
                c.Dispose();
        }
    }
}