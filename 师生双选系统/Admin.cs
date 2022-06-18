using Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using Message = UI.Message;

namespace 师生双选系统
{
    public partial class Admin : Form
    {
        private Form _f;
        private Login _lg;
        private Button _nowRouter;

        public Admin()
        {
            InitializeComponent();
        }

        private void ChangeTab(object sender)
        {
            _nowRouter.ForeColor = Color.Black;
            _nowRouter.BackColor = Color.FromArgb(240, 240, 240);

            _nowRouter = (Button)sender;
            _nowRouter.ForeColor = Color.FromArgb(240, 240, 240);
            _nowRouter.BackColor = Color.Black;
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            Message.ShowInfo($@"管理员 {admin_config.Instance.admID}, 欢迎回来!", this);

            _nowRouter = button2;
            RouterChange(new AdmUserInfo());

            label18.Text = @"工号：" + admin_config.Instance.admID;
        }


        private void Admin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_lg == null || _lg.IsDisposed)
                Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeTab(sender);
            RouterChange(new AdmUserInfo());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChangeTab(sender);
            RouterChange(new AdmAutoDispense());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChangeTab(sender);
            RouterChange(new AdminSetting());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Message.ShowInfo("暂未开放, 敬请期待", this);
        }

        private void RouterChange(Form routingPage)
        {
            if (_f != null)
            {
                Router.Controls.Clear();
                _f.Close();
            }

            _f = routingPage;
            _f.TopLevel = false;
            _f.Dock = DockStyle.Fill;
            Router.Controls.Add(_f);
            _f.Show();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(@"确定要返回登录页面吗？",
                @"返回登录", MessageBoxButtons.OKCancel);

            if (dr != DialogResult.OK) return;
            _lg = new Login(goBack:true);
            _lg.Show();
            Close();
        }
    }
}