using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using 师生双选系统.Properties;
using Message = UI.Message;

namespace 师生双选系统
{
    public partial class StuArmyInfo : Form
    {
        private readonly Read<group_info> _gbll = new Read<group_info>();
        private readonly Read<stu_info> _sbll = new Read<stu_info>();
        private readonly Update _ubll = new Update();
        private stu_info _headerInfo;

        private Panel _gradeInfo;

        public StuArmyInfo()
        {
            InitializeComponent();
        }

        private void StuArmyInfo_Load(object sender, EventArgs e)
        {
            if (stu_info.Instance.role.Equals(2)) button2.Visible = false;

            _gradeInfo = new Panel
            {
                Location = new Point(105, 238),
                AutoSize = true
            };


            textBox2.Text = _gbll.GetOnlyContent("g_id=" + stu_info.Instance.g_id, "g_name").ToString();

            List<stu_info> memberList = _sbll.GetDbContent("g_id=" + stu_info.Instance.g_id + " ORDER BY role");

            _headerInfo = memberList[0];
            for (int i = 0; i < memberList.Count; i++) AddList(i, memberList[i], _gradeInfo);

            Controls.Add(_gradeInfo);
        }


        private static void AddList(int index, stu_info content, Control gradeInfo)
        {
            Panel p = new Panel();
            Label divider = new Label();

            Label i1 = new Label();
            Label i2 = new Label();
            Label i3 = new Label();
            Label i4 = new Label();
            Label i5 = new Label();

            void Ml(object sender, EventArgs e)
            {
                p.BackColor = Color.White;
            }

            void Me(object sender, EventArgs e)
            {
                p.BackColor = Color.FromArgb(245, 247, 250);
            }

            divider.BackColor = Color.Gainsboro;
            //divider.Location = new Point(105, 238);
            divider.Location = new Point(0, 0);
            divider.Size = new Size(390, 1);

            p.Location = new Point(0, index * 36);
            p.Size = new Size(390, 36);
            p.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            p.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i1.AutoSize = true;
            i1.BackColor = Color.Transparent;
            i1.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i1.ForeColor = Color.FromArgb(96, 98, 102);
            i1.Location = new Point(0, 10);
            i1.Text = content.role == 1 ? "队长" : "队员";
            i1.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i1.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i2.AutoSize = true;
            i2.BackColor = Color.Transparent;
            i2.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i2.ForeColor = Color.FromArgb(96, 98, 102);
            i2.Location = new Point(49, 10);
            i2.Text = content.s_name;
            i1.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i1.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i3.AutoSize = true;
            i3.BackColor = Color.Transparent;
            i3.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i3.ForeColor = Color.FromArgb(96, 98, 102);
            i3.Location = new Point(111, 10);
            i3.Text = content.s_id.ToString();
            i3.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i3.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i4.AutoSize = true;
            i4.BackColor = Color.Transparent;
            i4.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i4.ForeColor = Color.FromArgb(96, 98, 102);
            i4.Location = new Point(193, 10);
            i4.Text = content.grade.ToString();
            i4.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i4.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i5.AutoSize = true;
            i5.BackColor = Color.Transparent;
            i5.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i5.ForeColor = Color.FromArgb(96, 98, 102);
            i5.Location = new Point(246, 10);
            i5.Text = ConfigurationManager.AppSettings["Speciality"].Split(',')[content.major_id + 1];
            i5.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i5.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            p.Controls.Add(i1);
            p.Controls.Add(i2);
            p.Controls.Add(i3);
            p.Controls.Add(i4);
            p.Controls.Add(i5);
            p.Controls.Add(divider);

            gradeInfo.Controls.Add(p);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (!textBox2.ReadOnly)
            {
                textBox2.ReadOnly = true;
                if (_ubll.UpdateGroupdata(textBox2.Text, "g_name"))
                    Message.ShowSuccess(@"修改成功！");
                else
                    Message.ShowError(@"修改失败！");

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
                textBox1.Focus();
            }

            else
            {
                label19.BackColor = Color.Black;
                label3.Image = Resources.确定;
                textBox2.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new CreateGroup(memberList =>
            {
                Tool.Close(_gradeInfo);
                AddList(0, _headerInfo, _gradeInfo);
                for (int i = 0; i < memberList.Count; i++) AddList(i + 1, memberList[i], _gradeInfo);
            }).Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new CreateGroup().Show();
        }
    }
}