using Bll;
using Model;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using JView.Properties;
using Message = UI.Message;

namespace JView
{
    public partial class StuPersonInfo : Form
    {
        private readonly Read<object> _rd = new Read<object>();
        private readonly Update _up = new Update();
        private readonly string _placeholder;
        public StuPersonInfo()
        {
            InitializeComponent();
            comboBox6.SelectedItem = GetFontFamilies();
            _placeholder = comboBox6.SelectedItem.ToString().Equals("更纱黑体") ? @"    " : @"       ";
        }

        private void StuPersonInfo_Load(object sender, EventArgs e)
        {
            label5.Text = stu_info.Instance.s_id + _placeholder;
            comboBox1.SelectedIndex = stu_info.Instance.role;

            if (stu_info.Instance.role != 0)
                label21.Visible = false;

            if (!stu_info.Instance.g_id.Equals(0))
            {
                label16.Visible = true;
                label16.Text = new[] { "未选", "队长", "队员" }[stu_info.Instance.role] + _placeholder;
                comboBox1.Visible = false;
            }

            label7.Text = stu_info.Instance.s_name + _placeholder;
            label9.Text = ConfigurationManager.AppSettings["Speciality"].Split(',')[stu_info.Instance.major_id + 1] + _placeholder;
            label11.Text = stu_info.Instance.sex + _placeholder;
            label13.Text = stu_info.Instance.grade + _placeholder;
            label22.Text = (stu_info.Instance.g_id.Equals(0) ? "null" : stu_info.Instance.g_id.ToString()) + _placeholder;
            textBox2.Text = @"●●●●●●";
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (!textBox2.ReadOnly)
            {
                DialogResult drc = MessageBox.Show(@"确定修改吗？ 新密码为：" + textBox2.Text,
                    @"提示",
                    MessageBoxButtons.YesNo);
                if (drc != DialogResult.Yes) return;
                stu_info.Instance.pwd = _rd.GetMd5(textBox2.Text);
                textBox2.ReadOnly = true;
                if (_up.UpdateStuInfo(stu_info.Instance.pwd, "pwd"))
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox1.Focused)
                return;
            
            ComboBox send = (ComboBox)sender;

            label21.Visible = send.SelectedIndex == 0;
            stu_info.Instance.role = send.SelectedIndex;
            if (_up.UpdateStuInfo(send.SelectedIndex.ToString(), "role"))
                Message.ShowSuccess(@"修改成功！");
            else
                Message.ShowError(@"修改失败！");
            
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

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox6.Focused)
                return;

            SetFontFamilies(comboBox6.SelectedItem.ToString());
        }

        private static string GetFontFamilies()
        {
            string path = Application.StartupPath + "//UserInfo.xml";
            if (!File.Exists(path))
                return "更纱黑体";

            XmlDocument userInfo = new XmlDocument();
            userInfo.Load(path);

            XmlElement preferFont = (XmlElement)userInfo.DocumentElement?.GetElementsByTagName("PreferFont")[0];
            return preferFont?.InnerText;
        }

        private static void SetFontFamilies(string fontName)
        {
            string path = Application.StartupPath + "//UserInfo.xml";

            XmlDocument userInfo = new XmlDocument();

            userInfo.Load(path);
            XmlElement preferFont = (XmlElement)userInfo.DocumentElement?.GetElementsByTagName("PreferFont")[0];
            if (preferFont != null) preferFont.InnerText = fontName;
            userInfo.Save(path);

            Application.Restart();
            Process.GetCurrentProcess()?.Kill();

        }
    }
}