using Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using Message = UI.Message;

namespace 师生双选系统
{
    public partial class Student : Form
    {
        private Form _f;
        private Login _lg;
        private Button _nowRouter;

        public Student()
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

        private void Student_Load(object sender, EventArgs e)
        {
            Message.ShowInfo($@"{stu_base_info.s_name}同学, 欢迎回来!", this);

            _nowRouter = button2;
            RouterChange(new StuPersonInfo());

            label18.Text = stu_base_info.s_name;
        }


        private void Student_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_lg == null || _lg.IsDisposed)
                Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeTab(sender);
            RouterChange(new StuPersonInfo());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckGroup<StuArmyInfo>(sender))
                return;

            ChangeTab(sender);
            RouterChange(new StuArmyInfo());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!CheckGroup<StuProjectInfo>(sender))
                return;

            ChangeTab(sender);
            RouterChange(new StuProjectInfo());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(@"确定要返回登录页面吗？",
                @"返回登录", MessageBoxButtons.OKCancel);

            if (dr != DialogResult.OK) return;
            _lg = new Login(goBack: true);
            _lg.Show();
            Close();
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

        private bool CheckGroup<T>(object sender) where T : Form, new()
        {
            if (!stu_base_info.g_id.Equals(0)) return true;
            switch (stu_base_info.role)
            {
                case 1:
                    {
                        DialogResult dr = MessageBox.Show(@"您还尚未创建队伍，是否立即创建？", @"Not Group", MessageBoxButtons.OKCancel);

                        if (dr == DialogResult.OK)
                        {
                            CreateGroup f = new CreateGroup(() =>
                            {
                                ChangeTab((Button)sender);
                                RouterChange(new T());
                            });
                            f.Show();
                        }

                        break;
                    }
                case 2:
                    Message.ShowWarn(@"您未存在于任何队伍，请尽快联系队长创建！");
                    break;
                default:
                    Message.ShowWarn(@"请先选择您的身份！");
                    break;
            }

            return false;
        }
    }
}