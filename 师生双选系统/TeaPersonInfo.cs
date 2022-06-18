using BLL;
using Model;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using 师生双选系统.Properties;
using Message = UI.Message;

namespace 师生双选系统
{
    public partial class TeaPersonInfo : Form
    {
        private readonly Read<object> _rd = new Read<object>();
        private readonly Update _up = new Update();
        private readonly string placeholder;
        public TeaPersonInfo()
        {
            InitializeComponent();
            comboBox6.SelectedItem = GetFontFamilies();
            placeholder = comboBox6.SelectedItem.ToString().Equals("更纱黑体") ? @"    " : @"       ";

            textBox3.MouseEnter += delegate { textBox3.BackColor = panel1.BackColor = Color.FromArgb(240, 240, 240); };
            panel1.MouseEnter += delegate { textBox3.BackColor = panel1.BackColor = Color.FromArgb(240, 240, 240); };
            panel1.MouseLeave += delegate
            {
                if (textBox3.Focused)
                    return;
                textBox3.BackColor = panel1.BackColor = Color.White;
            };
            textBox3.MouseLeave += delegate
            {
                if (textBox3.Focused)
                    return;
                textBox3.BackColor = panel1.BackColor = Color.White;
            };
        }


        private void TeaPersonInfo_Load(object sender, EventArgs e)
        {
            label5.Text = tea_info.Instance.t_id + placeholder;
            label7.Text = tea_info.Instance.t_name + placeholder;
            label9.Text = ConfigurationManager.AppSettings["Speciality"].Split(',')[stu_info.Instance.major_id + 1] + placeholder;
            label11.Text = tea_info.Instance.sex + placeholder;
            label13.Text = tea_info.Instance.grade + placeholder;
            textBox2.Text = @"●●●●●●";
            textBox3.Text = tea_info.Instance.direction;
            if (!textBox3.Text.Equals(""))
                label21.Visible = false;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (!textBox2.ReadOnly)
            {
                if (textBox2.Text.Trim() == "")
                {
                    Message.ShowWarn(@"新密码不得为空！");
                    textBox2.Focus();
                    return;
                }

                DialogResult drc = MessageBox.Show(@"确定修改吗？ 新密码为：" + textBox2.Text,
                    @"提示",
                    MessageBoxButtons.YesNo);
                if (drc != DialogResult.Yes) return;
                tea_info.Instance.pwd = _rd.GetMd5(textBox2.Text, tea_info.Instance.t_no);
                textBox2.ReadOnly = true;
                if (_up.UpdateTeaInfo(tea_info.Instance.pwd, "pwd"))
                    Message.ShowSuccess(@"修改成功！");
                else
                    Message.ShowError(@"修改失败！");
                return;
            }

            DialogResult dr = MessageBox.Show(@"确定要修改密码吗？",
                @"提示",
                MessageBoxButtons.OKCancel);

            if (dr == DialogResult.OK)
                textBox2.ReadOnly = false;
        }


        private void textBox2_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (textBox2.ReadOnly)
            {
                label19.BackColor = Color.White;
                label3.Image = Resources.edit;
                textBox2.Text = @"●●●●●●"; //清空密码
                textBox1.Focus();
            }

            else
            {
                label19.BackColor = Color.Black;
                label3.Image = Resources.确定;
                textBox2.Text = "";
                textBox2.Focus();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int multilineTextHeight = (textBox3.GetLineFromCharIndex(textBox3.Text.Length) + 1) * textBox3.Font.Height;
            textBox3.ScrollBars = textBox3.Height < multilineTextHeight ? ScrollBars.Vertical : ScrollBars.None;
        }

        private void textBox3_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (textBox3.ReadOnly)
            {
                label4.BackColor = Color.Silver;
                label18.BackColor = Color.Silver;
                label17.Image = Resources.edit;
                textBox3.BackColor = panel1.BackColor = Color.White;
                textBox1.Focus();
            }

            else
            {
                label4.BackColor = Color.Black;
                label18.BackColor = Color.Black;
                label17.Image = Resources.确定;
                textBox3.BackColor = panel1.BackColor = Color.FromArgb(240, 240, 240);
                textBox3.Focus();
            }
        }

        private void label17_Click(object sender, EventArgs e)
        {
            if (!textBox3.ReadOnly)
            {
                if (textBox3.Text.Trim() == "")
                {
                    Message.ShowWarn(@"研究方向为空！");
                    textBox3.Focus();
                    return;
                }

                textBox3.ReadOnly = true;
                tea_info.Instance.direction = textBox3.Text;
                if (_up.UpdateTeaInfo(tea_info.Instance.direction, "direction"))
                    Message.ShowSuccess(@"修改成功！");
                else
                    Message.ShowError(@"修改失败！");

                return;
            }

            label21.Visible = false;
            textBox3.ReadOnly = false;
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox6.Focused)
                return;

            SetFontFamilies(comboBox6.SelectedItem.ToString());
        }

        private string GetFontFamilies()
        {
            string path = Application.StartupPath + "//UserInfo.xml";
            if (!File.Exists(path))
                return "更纱黑体";

            XmlDocument _userInfo = new XmlDocument();
            _userInfo.Load(path);

            XmlElement PreferFont = (XmlElement)_userInfo.DocumentElement?.GetElementsByTagName("PreferFont")[0];
            return PreferFont.InnerText;
        }

        private void SetFontFamilies(string fontName)
        {
            string path = Application.StartupPath + "//UserInfo.xml";

            XmlDocument _userInfo = new XmlDocument();

            _userInfo.Load(path);
            XmlElement PreferFont = (XmlElement)_userInfo.DocumentElement?.GetElementsByTagName("PreferFont")[0];
            PreferFont.InnerText = fontName;
            _userInfo.Save(path);

            Application.Restart();
            Process.GetCurrentProcess()?.Kill();

        }
    }
}