using Bll;
using Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using Message = UI.Message;

namespace JView
{
    public partial class Teacher : Form
    {
        private readonly Read<tea_choose> _checkChoose = new Read<tea_choose>();

        private Form _f;
        private Login _lg;
        private Button _nowRouter;

        public Teacher()
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

        private void Teacher_Load(object sender, EventArgs e)
        {
            Message.ShowInfo($@"{tea_info.Instance.t_name}老师, 欢迎回来!", this);

            _nowRouter = button2;
            RouterChange(new TeaPersonInfo());

            label18.Text = tea_info.Instance.t_name;
        }


        private void Teacher_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_lg == null || _lg.IsDisposed)
                Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChangeTab(sender);
            RouterChange(new TeaPersonInfo());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CheckChoose<TeaArmyInfo>(sender))
                return;

            ChangeTab(sender);
            RouterChange(new TeaArmyInfo());
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

        private bool CheckChoose<T>(object sender) where T : Form, new()
        {
            bool haveChoose = (int)(long)_checkChoose.GetOnlyContent("tid=" + tea_info.Instance.t_id, "count(*)") != 0;
            if (haveChoose) return true;

            DialogResult dr = MessageBox.Show(@"您还尚未选择任何队伍，是否立即选择？", @"Not Group", MessageBoxButtons.OKCancel);

            if (dr != DialogResult.OK) return false;
            ChooseGroup f = new ChooseGroup(() =>
            {
                ChangeTab((Button)sender);
                RouterChange(new T());
            });
            f.Show();

            return false;
        }
    }
}