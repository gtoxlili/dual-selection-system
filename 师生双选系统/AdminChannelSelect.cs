using System;
using System.Windows.Forms;

namespace 师生双选系统
{
    public partial class AdminChannelSelect : Form
    {
        private readonly int _infoTabindex;

        public AdminChannelSelect(int tab)
        {
            _infoTabindex = tab;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new AdminChangeInfo(_infoTabindex).Show();
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new AdminChannel(_infoTabindex).Show();
            Close();
        }
    }
}