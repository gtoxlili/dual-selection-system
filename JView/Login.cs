using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Bll;
using Model;
using static System.Configuration.ConfigurationManager;
using Message = UI.Message;

namespace JView
{
    public partial class Login : Form
    {
        private readonly bool _goBack;
        private readonly XmlDocument _userInfo = new XmlDocument();

        private bool _isLogin;

        public Login()
        {
            InitializeComponent();
        }

        public Login(bool goBack)
        {
            InitializeComponent();
            _goBack = goBack;
        }

        private void CreateNewInfoFile(string path)
        {
            XmlElement root = _userInfo.CreateElement("Info");
            root.SetAttribute("identity", "");
            root.AppendChild(_userInfo.CreateElement("User"));
            root.AppendChild(_userInfo.CreateElement("Password"));
            root.AppendChild(_userInfo.CreateElement("autoLogin"));
            root.AppendChild(_userInfo.CreateElement("rememberPW"));
            root.AppendChild(_userInfo.CreateElement("PreferFont"));

            XmlElement preferFont = (XmlElement)root?.GetElementsByTagName("PreferFont")[0];
            preferFont.InnerText = "更纱黑体";
            
            _userInfo.AppendChild(root);
            _userInfo.Save(path);
        }

        private void SaveInfoFile(string path)
        {
            XmlElement root = _userInfo.DocumentElement;
            root?.SetAttribute("identity", Aes.GetCiphertext(comboBox1.Text));
            XmlElement user = (XmlElement)root?.GetElementsByTagName("User")[0];
            XmlElement password = (XmlElement)root?.GetElementsByTagName("Password")[0];
            XmlElement autoLogin = (XmlElement)root?.GetElementsByTagName("autoLogin")[0];
            XmlElement rememberPw = (XmlElement)root?.GetElementsByTagName("rememberPW")[0];
            user.InnerText = Aes.GetCiphertext(textBox1.Text);
            rememberPw.InnerText = checkBox2.Checked ? "1" : "0";
            autoLogin.InnerText = checkBox1.Checked ? "1" : "0";
            if (checkBox2.Checked)
                password.InnerText = Aes.GetCiphertext(textBox2.Text);

            _userInfo.Save(path);
        }

        private void Login_Load(object sender, EventArgs e)
        {
            textBox2.GotFocus += delegate { label3.BackColor = Color.Black; };
            textBox2.LostFocus += delegate { label3.BackColor = Color.Silver; };
            textBox1.GotFocus += delegate { label2.BackColor = Color.Black; };
            textBox1.LostFocus += delegate { label2.BackColor = Color.Silver; };
            comboBox1.DropDown += delegate { label6.BackColor = Color.Black; };
            comboBox1.DropDownClosed += delegate { label6.BackColor = Color.Silver; };
            comboBox1.Items.AddRange(AppSettings["Identity"].Split(','));

            string path = Application.StartupPath + "//UserInfo.xml";
            if (!File.Exists(path))
                CreateNewInfoFile(path);
            _userInfo.Load(path);
            if (!_userInfo.DocumentElement.GetAttribute("identity").Equals(""))
            {
                string user = Aes.GetPlaintext(_userInfo.DocumentElement.SelectNodes("User")?[0].InnerText);
                string password = Aes.GetPlaintext(_userInfo.DocumentElement.SelectNodes("Password")?[0].InnerText);
                string identity = Aes.GetPlaintext(_userInfo.DocumentElement.GetAttribute("identity"));
                string rememberPw = _userInfo.DocumentElement.SelectNodes("rememberPW")?[0].InnerText;
                string autoLogin = _userInfo.DocumentElement.SelectNodes("autoLogin")?[0].InnerText;
                textBox1.Text = user;
                comboBox1.Text = identity;
                checkBox1.Checked = autoLogin.Equals("1");
                checkBox2.Checked = rememberPw.Equals("1");
                if (checkBox2.Checked)
                    textBox2.Text = password;
            }
            else
            {
                comboBox1.Text = @"学生";
            }

            if (!checkBox1.Checked || _goBack) return;
            button2_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") || textBox2.Text.Equals(""))
            {
                Message.ShowWarn(@"请输入用户名和密码！", minWidth: 42);
                return;
            }

            switch (comboBox1.Text)
            {
                case "学生":
                    Stu_Login();
                    break;
                case "教师":
                    Tea_Login();
                    break;
                case "管理员":
                    Adm_Login();
                    break;
            }
        }

        private void Stu_Login()
        {
            if (textBox1.Text.Length != 9)
            {
                Message.ShowError(@"学号长度不符合！", minWidth: 42);
                return;
            }

            Read<stu_info> func = new Read<stu_info>();
            stu_info result = func.CheckAcc(textBox1.Text, textBox2.Text);

            if (result == null)
            {
                Message.ShowError(@"账号或密码错误！", minWidth: 42);
                return;
            }

            SaveInfoFile(Application.StartupPath + "//UserInfo.xml");
            func.EntryInfo(result);
            _isLogin = true;
            Student stu = new Student();
            stu.Show();
            Close();
        }

        private void Adm_Login()
        {
            Read<admin_config> func = new Read<admin_config>();
            admin_config result = func.CheckAdmAcc(textBox1.Text, textBox2.Text);

            if (result == null)
            {
                Message.ShowError(@"账号或密码错误！", minWidth: 42);
                return;
            }

            SaveInfoFile(Application.StartupPath + "//UserInfo.xml");
            func.EntryInfo(result);
            _isLogin = true;
            Admin adm = new Admin();
            adm.Show();
            Close();
        }

        private void Tea_Login()
        {
            if (textBox1.Text.Length != 9)
            {
                Message.ShowError(@"工号长度不符合！", minWidth: 42);
                return;
            }

            Read<tea_info> func = new Read<tea_info>();
            tea_info result = func.CheckAcc(textBox1.Text, textBox2.Text);

            if (result == null)
            {
                Message.ShowError(@"账号或密码错误！", minWidth: 42);
                return;
            }

            SaveInfoFile(Application.StartupPath + "//UserInfo.xml");
            func.EntryInfo(result);
            _isLogin = true;
            Teacher tea = new Teacher();
            tea.Show();
            Close();
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_isLogin) Application.Exit();
        }
    }
}