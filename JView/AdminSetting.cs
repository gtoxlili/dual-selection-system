using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Bll;
using JView.Properties;
using Model;
using static System.Configuration.ConfigurationManager;
using Message = UI.Message;

namespace JView
{
    public partial class AdminSetting : Form
    {
        private readonly Read<admin_config> _func = new Read<admin_config>();
        private readonly Update _up;
        private string _infoSummary;

        public AdminSetting()
        {
            _up = new Update();
            InitializeComponent();
        }

        private void AdminSetting_Load(object sender, EventArgs e)
        {
            comboBox7.Items.AddRange(AppSettings["Grade"].Split(','));
            comboBox5.Items.AddRange(AppSettings["Speciality"].Split(','));
            comboBox7.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;

            comboBox9.Items.AddRange(AppSettings["Grade"].Split(','));
            comboBox8.Items.AddRange(AppSettings["Speciality"].Split(','));
            comboBox9.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;

            textBox2.Text = admin_config.Instance.admUser;
            textBox1.Text = admin_config.Instance.admPwd;

            comboBox1.SelectedIndex = admin_config.Instance.stuGroupNum - 1;
            comboBox2.SelectedIndex = admin_config.Instance.stuChooseNum - 1;

            dateTimePicker1.Value = Convert.ToDateTime(admin_config.Instance.stuCloseTime);

            comboBox3.SelectedIndex = admin_config.Instance.teaHaveNum - 1;
            comboBox4.SelectedIndex = admin_config.Instance.teaChooseNum - 1;

            dateTimePicker2.Value = Convert.ToDateTime(admin_config.Instance.teaCloseTime);

            textBox4.Text = admin_config.Instance.defaultPwd;
            comboBox6.SelectedItem = GetFontFamilies();

            _infoSummary =
                _func.GetMd5(
                    $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                    0);
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


        private void label3_Click(object sender, EventArgs e)
        {
            if (!textBox2.ReadOnly)
            {
                string nowSummary =
                    _func.GetMd5(
                        $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                        0);

                if (nowSummary == _infoSummary)
                {
                    Message.ShowWarn("您没有做任何修改!");
                    return;
                }

                textBox2.ReadOnly = true;
                if (_up.UpdateSettingInfo(textBox2.Text, "admUser"))
                {
                    _infoSummary = nowSummary;
                    admin_config.Instance.admUser = textBox2.Text;
                    Message.ShowSuccess(@"修改成功！");
                }
                else
                {
                    Message.ShowError(@"修改失败！");
                }

                return;
            }

            textBox2.ReadOnly = false;
        }

        private void textBox2_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (textBox2.ReadOnly)
            {
                label19.BackColor = Color.White;
                label3.Image = Resources.edit;
                textBox3.Focus();
            }

            else
            {
                label19.BackColor = Color.Black;
                label3.Image = Resources.确定;
                textBox2.Focus();
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (!textBox1.ReadOnly)
            {
                string nowSummary =
                    _func.GetMd5(
                        $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                        0);

                if (nowSummary == _infoSummary)
                {
                    Message.ShowWarn("您没有做任何修改!");
                    return;
                }

                textBox1.ReadOnly = true;
                if (_up.UpdateSettingInfo(textBox1.Text, "admPwd"))
                {
                    _infoSummary = nowSummary;
                    admin_config.Instance.admPwd = textBox1.Text;
                    Message.ShowSuccess(@"修改成功！");
                }
                else
                {
                    Message.ShowError(@"修改失败！");
                }

                return;
            }

            textBox1.ReadOnly = false;
        }

        private void textBox1_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (textBox1.ReadOnly)
            {
                label4.BackColor = Color.White;
                label5.Image = Resources.edit;
                textBox3.Focus();
            }

            else
            {
                label4.BackColor = Color.Black;
                label5.Image = Resources.确定;
                textBox1.Focus();
            }
        }

        private void label20_Click(object sender, EventArgs e)
        {
            if (!textBox4.ReadOnly)
            {
                string nowSummary =
                    _func.GetMd5(
                        $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                        0);

                if (nowSummary == _infoSummary)
                {
                    Message.ShowWarn("您没有做任何修改!");
                    return;
                }

                textBox4.ReadOnly = true;
                if (_up.UpdateSettingInfo(textBox4.Text, "defaultPwd"))
                {
                    _infoSummary = nowSummary;
                    admin_config.Instance.defaultPwd = textBox4.Text;
                    Message.ShowSuccess(@"修改成功！");
                }
                else
                {
                    Message.ShowError(@"修改失败！");
                }

                return;
            }

            textBox4.ReadOnly = false;
        }

        private void textBox4_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (textBox4.ReadOnly)
            {
                label21.BackColor = Color.White;
                label20.Image = Resources.edit;
                textBox3.Focus();
            }

            else
            {
                label21.BackColor = Color.Black;
                label20.Image = Resources.确定;
                textBox4.Focus();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox1.Focused)
                return;
            string nowSummary =
                _func.GetMd5(
                    $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                    0);
            if (nowSummary == _infoSummary)
            {
                Message.ShowWarn("您没有做任何修改!");
                return;
            }

            if (_up.UpdateSettingInfo((comboBox1.SelectedIndex + 1).ToString(), "stuGroupNum"))
            {
                _infoSummary = nowSummary;
                admin_config.Instance.stuGroupNum = comboBox1.SelectedIndex + 1;
                Message.ShowSuccess(@"修改成功！");
            }
            else
            {
                Message.ShowError(@"修改失败！");
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox2.Focused)
                return;
            string nowSummary =
                _func.GetMd5(
                    $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                    0);
            if (nowSummary == _infoSummary)
            {
                Message.ShowWarn("您没有做任何修改!");
                return;
            }

            if (_up.UpdateSettingInfo((comboBox2.SelectedIndex + 1).ToString(), "stuChooseNum"))
            {
                _infoSummary = nowSummary;
                admin_config.Instance.stuChooseNum = comboBox2.SelectedIndex + 1;
                Message.ShowSuccess(@"修改成功！");
            }
            else
            {
                Message.ShowError(@"修改失败！");
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox4.Focused)
                return;
            string nowSummary =
                _func.GetMd5(
                    $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                    0);
            if (nowSummary == _infoSummary)
            {
                Message.ShowWarn("您没有做任何修改!");
                return;
            }

            if (_up.UpdateSettingInfo((comboBox4.SelectedIndex + 1).ToString(), "teaChooseNum"))
            {
                _infoSummary = nowSummary;
                admin_config.Instance.teaChooseNum = comboBox4.SelectedIndex + 1;
                Message.ShowSuccess(@"修改成功！");
            }
            else
            {
                Message.ShowError(@"修改失败！");
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox3.Focused)
                return;
            string nowSummary =
                _func.GetMd5(
                    $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                    0);
            if (nowSummary == _infoSummary)
            {
                Message.ShowWarn("您没有做任何修改!");
                return;
            }

            if (_up.UpdateSettingInfo((comboBox3.SelectedIndex + 1).ToString(), "teaHaveNum"))
            {
                _infoSummary = nowSummary;
                admin_config.Instance.teaHaveNum = comboBox3.SelectedIndex + 1;
                Message.ShowSuccess(@"修改成功！");
            }
            else
            {
                Message.ShowError(@"修改失败！");
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (!dateTimePicker1.Focused)
                return;
            string nowSummary =
                _func.GetMd5(
                    $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                    0);
            if (nowSummary == _infoSummary)
            {
                Message.ShowWarn("您没有做任何修改!");
                return;
            }

            if (_up.UpdateSettingInfo(dateTimePicker1.Value.ToString("yyyy-MM-dd"), "stuCloseTime"))
            {
                _infoSummary = nowSummary;
                admin_config.Instance.stuCloseTime = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                Message.ShowSuccess(@"修改成功！");
            }
            else
            {
                Message.ShowError(@"修改失败！");
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (!dateTimePicker2.Focused)
                return;
            string nowSummary =
                _func.GetMd5(
                    $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                    0);
            if (nowSummary == _infoSummary)
            {
                Message.ShowWarn("您没有做任何修改!");
                return;
            }

            if (_up.UpdateSettingInfo(dateTimePicker2.Value.ToString("yyyy-MM-dd"), "teaCloseTime"))
            {
                _infoSummary = nowSummary;
                admin_config.Instance.teaCloseTime = dateTimePicker2.Value.ToString("yyyy-MM-dd");
                Message.ShowSuccess(@"修改成功！");
            }
            else
            {
                Message.ShowError(@"修改失败！");
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBox6.Focused)
                return;
            string nowSummary =
                _func.GetMd5(
                    $@"{textBox2.Text}{textBox1.Text}{comboBox1.SelectedIndex}{comboBox2.SelectedIndex}{dateTimePicker1.Value}{comboBox3.SelectedIndex}{comboBox4.SelectedIndex}{dateTimePicker2.Value}{textBox4.Text}{comboBox6.SelectedIndex}",
                    0);
            if (nowSummary == _infoSummary)
            {
                Message.ShowWarn("您没有做任何修改!");
                return;
            }

            SetFontFamilies(comboBox6.SelectedItem.ToString());
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