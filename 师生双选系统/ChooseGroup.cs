using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using 师生双选系统.Properties;
using static System.Configuration.ConfigurationManager;
using Message = UI.Message;

namespace 师生双选系统
{
    public sealed partial class ChooseGroup : Form
    {
        private const int PageSize = 8;
        private readonly Action<List<group_info>> _changeFatherInfo;
        private readonly Action _changeFatherTab;
        private readonly Read<tea_choose> _chooseFunc = new Read<tea_choose>();
        private readonly Read<group_info> _groupFunc = new Read<group_info>();
        private readonly List<group_info> _groupSelectedList;

        private readonly Update _up = new Update();
        private List<group_info> _groupUnSelectList;
        private int _pageIndex = 1;
        private int _unSelectCount;

        public ChooseGroup(Action changeFatherTab)
        {
            _changeFatherTab = changeFatherTab;
            _unSelectCount = (int)(long)_groupFunc.GetOnlyContent("", "count(*)");
            _groupUnSelectList = _groupFunc.GetGroupInfo($@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");
            _groupSelectedList = new List<group_info>();
            InitializeComponent();
        }

        public ChooseGroup(Action<List<group_info>> changeFatherInfo)
        {
            //SELECT g_id,group_concat(s_name)  FROM group_info inner join stu_info using(g_id) GROUP BY g_id
            _changeFatherInfo = changeFatherInfo;
            _unSelectCount = (int)(long)_groupFunc.GetOnlyContent("", "COUNT(*)");
            _groupUnSelectList = _groupFunc.GetGroupInfo($@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");
            tea_choose groupArr = _chooseFunc.GetEntityValue("tid=" + tea_base_info.t_id);
            if (groupArr == null)
            {
                _groupSelectedList = new List<group_info>();
            }
            else
            {
                _groupSelectedList =
                    _groupFunc.GetGroupInfo(
                        $@"g_id in ({groupArr.c1},{groupArr.c2},{groupArr.c3},{groupArr.c4},{groupArr.c5})");

                List<group_info> unSelectListTemp = new List<group_info>(_groupUnSelectList);

                foreach (group_info t1 in unSelectListTemp.Where(
                             t1 => _groupSelectedList.Exists(t => t.g_id == t1.g_id)))
                    _groupUnSelectList.Remove(t1);
            }


            InitializeComponent();
        }

        private void ChooseGroup_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(AppSettings["Grade"].Split(','));
            comboBox2.Items.AddRange(AppSettings["Speciality"].Split(','));
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            panel1.Controls.Add(CreateiteratorsBox(true));
            panel2.Controls.Add(CreateiteratorsBox(false));
            panel3.Controls.Add(CreatePagination());
        }

        private Panel CreateCard(group_info value, int X, int Y, bool status)
        {
            Panel card = new Panel
            {
                BackgroundImage = Resources.阴影卡片,
                BackgroundImageLayout = ImageLayout.Stretch,
                Location = new Point(X, Y),
                Size = new Size(240, 320)
            };

            Label btn = new Label
            {
                AutoSize = true,
                Cursor = Cursors.Hand,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 10F, FontStyle.Bold, GraphicsUnit.Point, 134),
                ForeColor = ReturnRandomColor(value.g_no),
                Location = new Point(200, 24),
                Text = status ? "▶" : "◀"
            };
            btn.Click += (sender, e) =>
            {
                if (status)
                {
                    if (_groupSelectedList.Count == 5)
                    {
                        Message.ShowWarn(@"最多只能选择5个小组！", this);
                        return;
                    }

                    _groupUnSelectList.Remove(value);
                    _groupSelectedList.Add(value);
                }
                else
                {
                    _groupUnSelectList.Add(value);
                    _groupSelectedList.Remove(value);
                }

                _groupUnSelectList.Sort((x, y) => x.g_no.CompareTo(y.g_no));

                Tool.Close(panel1);
                Tool.Close(panel2);

                panel1.Controls.Add(CreateiteratorsBox(true));
                panel2.Controls.Add(CreateiteratorsBox(false));
            };


            card.Controls.Add(btn);

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


        private Panel CreatePagination()
        {
            Panel p = new Panel
            {
                Location = new Point(0, 0),
                AutoSize = true
            };

            int pageNum = _unSelectCount / PageSize + (_unSelectCount % PageSize == 0 ? 0 : 1);

            if (pageNum == 0)
                pageNum = 1;

            void Ml(object sender, EventArgs e)
            {
                if (((Label)sender).Text == _pageIndex.ToString()) return;
                ((Label)sender).BackColor = Color.White;
            }

            void Me(object sender, EventArgs e)
            {
                if (((Label)sender).Text == _pageIndex.ToString()) return;
                ((Label)sender).BackColor = Color.FromArgb(245, 245, 245);
            }

            void Clickfunc(object sender, EventArgs e)
            {
                Label l = (Label)sender;
                switch (l.Text)
                {
                    case "<":
                        _pageIndex--;
                        break;
                    case ">":
                        _pageIndex++;
                        break;
                    default:
                        {
                            if (_pageIndex == Convert.ToInt32(l.Text)) return;
                            _pageIndex = Convert.ToInt32(l.Text);
                            break;
                        }
                }

                _groupUnSelectList = _groupFunc.GetGroupInfo(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                    textBox1.Text, $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                List<group_info> unSelectListTemp = new List<group_info>(_groupUnSelectList);

                foreach (group_info t1 in unSelectListTemp.Where(
                             t1 => _groupSelectedList.Exists(t => t.g_id == t1.g_id)))
                    _groupUnSelectList.Remove(t1);

                Tool.Close(panel1);
                Tool.Close(panel3);

                panel1.Controls.Add(CreateiteratorsBox(true));
                panel3.Controls.Add(CreatePagination());
            }

            Label l1 = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(30, 28),
                Text = @"<",
                ForeColor = _pageIndex == 1 ? Color.FromArgb(189, 189, 189) : Color.Black,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = _pageIndex == 1 ? Cursors.Default : Cursors.Hand
            };

            if (_pageIndex != 1)
            {
                l1.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                l1.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
                l1.Click += new EventHandler((Action<object, EventArgs>)Clickfunc);
            }

            Label l2 = new Label
            {
                Location = new Point(pageNum >= 7 ? 320 : (pageNum + 1) * 40, 0),
                Size = new Size(30, 28),
                Text = @">",
                ForeColor = _pageIndex == pageNum ? Color.FromArgb(189, 189, 189) : Color.Black,
                Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = _pageIndex == pageNum ? Cursors.Default : Cursors.Hand
            };
            if (_pageIndex != pageNum)
            {
                l2.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                l2.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
                l2.Click += new EventHandler((Action<object, EventArgs>)Clickfunc);
            }

            // 分页数小时
            if (pageNum <= 7)
            {
                for (int i = 1; i <= pageNum; i++)
                {
                    Label l = new Label
                    {
                        Location = new Point(40 * i, 0),
                        Size = new Size(30, 28),
                        Text = i.ToString(),
                        BackColor = i.Equals(_pageIndex) ? Color.FromArgb(225, 225, 225) : Color.White,
                        Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Cursor = Cursors.Hand
                    };
                    l.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                    l.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
                    l.Click += new EventHandler((Action<object, EventArgs>)Clickfunc);
                    p.Controls.Add(l);
                }
            }
            else
            {
                if (_pageIndex <= 4)
                {
                    for (int i = 2; i < 6; i++)
                    {
                        Label l = new Label
                        {
                            Location = new Point(40 * i, 0),
                            Size = new Size(30, 28),
                            Text = i.ToString(),
                            BackColor = i.Equals(_pageIndex) ? Color.FromArgb(225, 225, 225) : Color.White,
                            Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Cursor = Cursors.Hand
                        };
                        l.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                        l.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
                        l.Click += new EventHandler((Action<object, EventArgs>)Clickfunc);
                        p.Controls.Add(l);
                    }

                    Label l3 = new Label
                    {
                        Location = new Point(40 * 6, 0),
                        Size = new Size(30, 28),
                        Text = @"...",
                        Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                        ForeColor = Color.FromArgb(189, 189, 189),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Cursor = Cursors.Default
                    };
                    p.Controls.Add(l3);
                }
                else if (_pageIndex > pageNum - 4)
                {
                    Label l3 = new Label
                    {
                        Location = new Point(40 * 2, 0),
                        Size = new Size(30, 28),
                        Text = @"...",
                        Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                        ForeColor = Color.FromArgb(189, 189, 189),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Cursor = Cursors.Default
                    };
                    p.Controls.Add(l3);

                    for (int i = 1; i < 5; i++)
                    {
                        Label l = new Label
                        {
                            Location = new Point(40 * (i + 2), 0),
                            Size = new Size(30, 28),
                            Text = (pageNum - 5 + i).ToString(),
                            BackColor = (pageNum - 5 + i).Equals(_pageIndex)
                                ? Color.FromArgb(225, 225, 225)
                                : Color.White,
                            Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Cursor = Cursors.Hand
                        };
                        l.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                        l.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
                        l.Click += new EventHandler((Action<object, EventArgs>)Clickfunc);
                        p.Controls.Add(l);
                    }
                }
                else
                {
                    Label l3 = new Label
                    {
                        Location = new Point(40 * 2, 0),
                        Size = new Size(30, 28),
                        Text = @"...",
                        Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                        ForeColor = Color.FromArgb(189, 189, 189),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Cursor = Cursors.Default
                    };
                    p.Controls.Add(l3);
                    Label l6 = new Label
                    {
                        Location = new Point(40 * 6, 0),
                        Size = new Size(30, 28),
                        Text = @"...",
                        Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                        ForeColor = Color.FromArgb(189, 189, 189),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Cursor = Cursors.Default
                    };
                    p.Controls.Add(l6);

                    for (int i = 3; i < 6; i++)
                    {
                        Label l = new Label
                        {
                            Location = new Point(40 * i, 0),
                            Size = new Size(30, 28),
                            Text = (_pageIndex + i - 4).ToString(),
                            BackColor = (_pageIndex + i - 4).Equals(_pageIndex)
                                ? Color.FromArgb(225, 225, 225)
                                : Color.White,
                            Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Cursor = Cursors.Hand
                        };
                        l.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                        l.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
                        l.Click += new EventHandler((Action<object, EventArgs>)Clickfunc);
                        p.Controls.Add(l);
                    }
                }


                Label l4 = new Label
                {
                    Location = new Point(40 * 7, 0),
                    Size = new Size(30, 28),
                    Text = pageNum.ToString(),
                    BackColor = _pageIndex == pageNum ? Color.FromArgb(225, 225, 225) : Color.White,
                    Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };
                l4.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                l4.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
                l4.Click += new EventHandler((Action<object, EventArgs>)Clickfunc);
                p.Controls.Add(l4);


                Label l5 = new Label
                {
                    Location = new Point(40 * 1, 0),
                    Size = new Size(30, 28),
                    Text = @"1",
                    BackColor = _pageIndex == 1 ? Color.FromArgb(225, 225, 225) : Color.White,
                    Font = new Font(CustomFont.Font.bPFC.Families[0], 10f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand
                };
                l5.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                l5.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
                l5.Click += new EventHandler((Action<object, EventArgs>)Clickfunc);
                p.Controls.Add(l5);
            }

            p.Controls.Add(l1);
            p.Controls.Add(l2);

            p.Left = (panel3.Width - l2.Right) / 2;

            return p;
        }

        private Panel CreateiteratorsBox(bool status)
        {
            Panel p = new Panel
            {
                Location = new Point(0, 0),
                AutoSize = true
            };

            List<group_info> db = status ? _groupUnSelectList : _groupSelectedList;

            int i = 0;
            while (true)
            {
                List<group_info> fz = db.Skip(i * 2).Take(2).ToList();
                if (fz.Count != 0)
                    for (int z = 0; z < fz.Count; z++)
                        p.Controls.Add(CreateCard(db[i * 2 + z], 35 + z * 275, 30 + 350 * i, status));
                else
                    break;
                i++;
            }

            return p;
        }


        private static Color ReturnRandomColor(int i)
        {
            Color[] co =
            {
                Color.FromArgb(254, 82, 86),
                Color.FromArgb(254, 167, 49),
                Color.FromArgb(248, 216, 102),
                Color.FromArgb(153, 240, 243),
                Color.FromArgb(0, 165, 231),
                Color.FromArgb(87, 35, 7)
            };
            return co[i % co.Length];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _pageIndex = 1;

            _unSelectCount = _groupFunc
                .GetGroupInfo(comboBox1.SelectedIndex, comboBox2.SelectedIndex, textBox1.Text, "").Count;

            _groupUnSelectList = _groupFunc.GetGroupInfo(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                textBox1.Text,
                $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

            List<group_info> unSelectListTemp = new List<group_info>(_groupUnSelectList);

            foreach (group_info t1 in unSelectListTemp.Where(t1 => _groupSelectedList.Exists(t => t.t_id == t1.t_id)))
                _groupUnSelectList.Remove(t1);

            Tool.Close(panel1);
            panel1.Controls.Add(CreateiteratorsBox(true));

            Tool.Close(panel3);
            panel3.Controls.Add(CreatePagination());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_groupSelectedList.Count == 0)
            {
                Message.ShowWarn(@"已选择的小组为空！", this);
                return;
            }

            if (MessageBox.Show(@"确定要提交吗？", @"提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) !=
                DialogResult.OK) return;
            //insert or replace into tea_choose(tid,c1,c2,c3,c4,c5) values (201900102,201900102,201900102,201900102,201900102,201900104);
            int[] gidArr = new int[5];
            for (int i = 0; i < _groupSelectedList.Count; i++) gidArr[i] = _groupSelectedList[i].g_id;
            if (_up.UpdateChooseGroupInfo(gidArr, tea_base_info.t_id.ToString()))
            {
                MessageBox.Show(@"提交成功！", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (_changeFatherTab != null)
                    _changeFatherTab();
                else
                    _changeFatherInfo(_groupSelectedList);

                Close();
            }
            else
            {
                Message.ShowError(@"提交失败！", this);
            }
        }
    }
}