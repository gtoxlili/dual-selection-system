using System;
using System.Windows.Forms;

namespace 师生双选系统
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new Login().Show();
            Application.Run();
        }
    }
}
