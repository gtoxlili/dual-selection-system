using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using 师生双选系统.Properties;
using static System.Configuration.ConfigurationManager;

namespace 师生双选系统
{
    public partial class TeaArmyInfo : Form
    {
        private readonly Read<tea_choose> _chooseFunc = new Read<tea_choose>();
        private readonly Read<group_info> _func = new Read<group_info>();
        private bool _carouselst = true;
        private List<group_info> _groupArr;
        private Panel _nowCard;
        private int _pageIndex;

        public TeaArmyInfo()
        {
            tea_choose grouparr = _chooseFunc.GetEntityValue("tid=" + tea_info.Instance.t_id);
            _groupArr = _func.GetGroupInfo(
                $@"g_id in ({grouparr.c1},{grouparr.c2},{grouparr.c3},{grouparr.c4},{grouparr.c5})");

            InitializeComponent();

            label10.MouseEnter += (s, _) => ((Label)s).BackColor = Color.FromArgb(245, 245, 245);
            label10.MouseLeave += (s, _) => ((Label)s).BackColor = Color.White;

            label4.MouseEnter += (s, _) => ((Label)s).BackColor = Color.FromArgb(245, 245, 245);
            label4.MouseLeave += (s, _) => ((Label)s).BackColor = Color.White;
        }

        private void TeaArmyInfo_Load(object sender, EventArgs e)
        {
            _nowCard = CreateCard(_groupArr[_pageIndex]);
            panel1.Controls.Add(_nowCard);
            label5.Text = $@"{_pageIndex + 1}/{_groupArr.Count}";
        }

        private static Panel CreateCard(group_info value, int x = 41, int y = 0)
        {
            Panel card = new Panel
            {
                BackgroundImage = Resources.阴影卡片,
                BackgroundImageLayout = ImageLayout.Stretch,
                Location = new Point(x, y),
                Size = new Size(240, 320)
            };

            Panel conPanel = new Panel
            {
                AutoScroll = true,
                Location = new Point(80, 205),
                Size = new Size(140, 90)
            };
            Label 项目说明 = new Label
            {
                AutoSize = true,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(0, 0),
                MaximumSize = new Size(140, 0),
                MinimumSize = new Size(140, 0),
                Text = value.project_info,
                TextAlign = ContentAlignment.MiddleLeft
            };
            项目说明.SizeChanged += (sender, e) =>
            {
                Label l = (Label)sender;
                if (l.Height >= 90)
                {
                    l.MaximumSize = new Size(123, 0);
                    l.MinimumSize = new Size(123, 0);
                }
                else
                {
                    l.MaximumSize = new Size(140, 0);
                    l.MinimumSize = new Size(140, 0);
                }
            };
            conPanel.Controls.Add(项目说明);
            Label l1 = new Label
            {
                AutoSize = true,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(20, 205),
                Text = @"项目说明",
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(conPanel);
            card.Controls.Add(l1);

            Label 项目名称 = new Label
            {
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(80, 177),
                Size = new Size(140, 15),
                Text = value.project_name,
                TextAlign = ContentAlignment.MiddleRight
            };
            Label l2 = new Label
            {
                AutoSize = true,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(20, 177),
                Text = @"项目名称",
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(项目名称);
            card.Controls.Add(l2);


            Label 组号 = new Label
            {
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(80, 149),
                Size = new Size(140, 15),
                Text = value.g_id.ToString(),
                TextAlign = ContentAlignment.MiddleRight
            };
            Label l3 = new Label
            {
                AutoSize = true,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(20, 149),
                Text = @"组号",
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(组号);
            card.Controls.Add(l3);


            Label 组员 = new Label
            {
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(60, 121),
                Size = new Size(160, 15),
                Text = value.nameArr,
                TextAlign = ContentAlignment.MiddleRight
            };
            Label l4 = new Label
            {
                AutoSize = true,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(20, 121),
                Text = @"组员",
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(组员);
            card.Controls.Add(l4);

            Label 专业 = new Label
            {
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(80, 93),
                Size = new Size(140, 15),
                Text = AppSettings["Speciality"].Split(',')[value.major_id + 1],
                TextAlign = ContentAlignment.MiddleRight
            };
            Label l5 = new Label
            {
                AutoSize = true,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(20, 93),
                Text = @"专业",
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(专业);
            card.Controls.Add(l5);

            Label 年级 = new Label
            {
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(80, 65),
                Size = new Size(140, 15),
                Text = value.grade.ToString(),
                TextAlign = ContentAlignment.MiddleRight
            };
            Label l6 = new Label
            {
                AutoSize = true,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = Color.FromArgb(144, 147, 153),
                Location = new Point(20, 65),
                Text = @"年级",
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(年级);
            card.Controls.Add(l6);

            Label divider = new Label
            {
                BackColor = Color.Silver,
                Location = new Point(20, 53),
                Size = new Size(200, 1)
            };
            card.Controls.Add(divider);

            Label title = new Label
            {
                AutoSize = true,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 12.25F, FontStyle.Bold),
                Location = new Point(20, 24),
                Text = value.g_name,
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(title);

            return card;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            new ChooseGroup(memberList =>
            {
                _pageIndex = 0;
                panel1.Controls.Remove(_nowCard);
                _nowCard.Dispose();
                _groupArr = memberList;
                _nowCard = CreateCard(_groupArr[_pageIndex]);
                panel1.Controls.Add(_nowCard);
                label5.Text = $@"{_pageIndex + 1}/{_groupArr.Count}";
            }).Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (!_carouselst)
                return;
            _carouselst = false;
            //上一页
            if (_pageIndex == 0)
                _pageIndex = _groupArr.Count - 1;
            else
                _pageIndex--;

            Panel nextCard = CreateCard(_groupArr[_pageIndex], 322);
            panel1.Controls.Add(nextCard);
            label5.Text = $@"{_pageIndex + 1}/{_groupArr.Count}";

            Timer t = new Timer
            {
                Enabled = true,
                Interval = 10
            };

            t.Tick += (_, __) =>
            {
                if (nextCard.Left >= 41)
                {
                    nextCard.Left -= 15;
                    _nowCard.Left -= 15;
                }
                else
                {
                    panel1.Controls.Remove(_nowCard);
                    _nowCard.Dispose();
                    _nowCard = nextCard;
                    _carouselst = true;
                    t.Dispose();
                }
            };
        }

        private void label10_Click(object sender, EventArgs e)
        {
            if (!_carouselst)
                return;
            _carouselst = false;
            //下一页
            if (_pageIndex == _groupArr.Count - 1)
                _pageIndex = 0;
            else
                _pageIndex++;

            Panel nextCard = CreateCard(_groupArr[_pageIndex], -240);
            panel1.Controls.Add(nextCard);
            label5.Text = $@"{_pageIndex + 1}/{_groupArr.Count}";

            Timer t = new Timer
            {
                Enabled = true,
                Interval = 10
            };

            t.Tick += (_, __) =>
            {
                if (nextCard.Left <= 41)
                {
                    nextCard.Left += 15;
                    _nowCard.Left += 15;
                }
                else
                {
                    panel1.Controls.Remove(_nowCard);
                    _nowCard.Dispose();
                    _nowCard = nextCard;
                    _carouselst = true;
                    t.Dispose();
                }
            };
        }
    }
}