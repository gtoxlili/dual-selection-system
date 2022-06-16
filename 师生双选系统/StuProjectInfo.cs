using BLL;
using Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using Message = UI.Message;

namespace 师生双选系统
{
    public partial class StuProjectInfo : Form
    {
        private readonly Read<group_info> _func = new Read<group_info>();
        private readonly Update _up = new Update();
        private string _infoSummary;

        public StuProjectInfo()
        {
            InitializeComponent();
            AddUIbeauifiy();
        }

        private void StuProjectInfo_Load(object sender, EventArgs e)
        {
            group_info g = _func.GetEntityValue("g_id=" + stu_base_info.g_id);

            textBox1.Text = g.project_name;
            textBox2.Text = g.project_info;

            if (!textBox1.Text.Equals("") && !textBox2.Text.Equals(""))
                _infoSummary = _func.GetMd5(textBox1.Text + textBox2.Text, 0);
        }

        public void AddUIbeauifiy()
        {
            panel2.MouseEnter += delegate { textBox1.BackColor = panel2.BackColor = Color.FromArgb(240, 240, 240); };
            textBox1.MouseEnter += delegate { textBox1.BackColor = panel2.BackColor = Color.FromArgb(240, 240, 240); };
            panel2.MouseLeave += delegate
            {
                if (textBox1.Focused)
                    return;
                textBox1.BackColor = panel2.BackColor = Color.White;
            };
            textBox1.MouseLeave += delegate
            {
                if (textBox1.Focused)
                    return;
                textBox1.BackColor = panel2.BackColor = Color.White;
            };

            textBox1.GotFocus += delegate { label17.BackColor = Color.Black; };
            textBox1.LostFocus += delegate
            {
                label17.BackColor = Color.Silver;
                textBox1.BackColor = panel2.BackColor = Color.White;
            };

            panel1.MouseEnter += delegate { textBox2.BackColor = panel1.BackColor = Color.FromArgb(240, 240, 240); };
            textBox2.MouseEnter += delegate { textBox2.BackColor = panel1.BackColor = Color.FromArgb(240, 240, 240); };
            panel1.MouseLeave += delegate
            {
                if (textBox2.Focused)
                    return;
                textBox2.BackColor = panel1.BackColor = Color.White;
            };
            textBox2.MouseLeave += delegate
            {
                if (textBox2.Focused)
                    return;
                textBox2.BackColor = panel1.BackColor = Color.White;
            };

            textBox2.GotFocus += delegate { label4.BackColor = Color.Black; };
            textBox2.LostFocus += delegate
            {
                label4.BackColor = Color.Silver;
                textBox2.BackColor = panel1.BackColor = Color.White;
            };
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int multilineTextHeight = (textBox2.GetLineFromCharIndex(textBox2.Text.Length) + 1) * textBox2.Font.Height;

            textBox2.ScrollBars = textBox2.Height < multilineTextHeight ? ScrollBars.Vertical : ScrollBars.None;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || textBox2.Text.Equals(""))
            {
                Message.ShowWarn("请完整填写信息!");
                return;
            }

            string nowSummary = _func.GetMd5(textBox1.Text + textBox2.Text, 0);

            if (nowSummary == _infoSummary)
            {
                Message.ShowWarn("您没有做任何修改!");
                return;
            }

            if (MessageBox.Show(@"确定要提交吗？", @"提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) !=
                DialogResult.OK) return;


            if (_up.UpdateGroupInfo(stu_base_info.g_id.ToString(), textBox1.Text, textBox2.Text))
            {
                _infoSummary = nowSummary;
                Message.ShowSuccess(@"提交成功！");
            }
            else
            {
                Message.ShowError(@"提交失败！");
            }
        }
    }
}