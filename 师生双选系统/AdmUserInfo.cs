using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using 师生双选系统.Properties;
using static System.Configuration.ConfigurationManager;
using Message = UI.Message;

namespace 师生双选系统
{
    public partial class AdmUserInfo : Form
    {
        private const int PageSize = 20;
        private readonly Read<autodispense_tmp> _autodispenseFunc = new Read<autodispense_tmp>();
        private readonly Read<group_info> _groupFunc = new Read<group_info>();
        private readonly Read<stu_info> _stuFunc = new Read<stu_info>();
        private readonly Read<tea_info> _teaFunc = new Read<tea_info>();
        private List<autodispense_tmp> _autodispenseList;
        private bool _carouselst = true;

        private int _count;
        private List<group_info> _groupArrayList;
        private int _infoTabindex;
        private int _pageIndex = 1;

        private List<stu_info> _stuArrayList;
        private List<tea_info> _teaArrayList;

        public AdmUserInfo()
        {
            _count = (int)(long)_stuFunc.GetOnlyContent("", "count(*)");
            _stuArrayList =
                _stuFunc.GetDbContent(
                    $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

            InitializeComponent();

            label3.MouseEnter += (s, _) => ((Label)s).BackColor = Color.FromArgb(245, 245, 245);
            label3.MouseLeave += (s, _) => ((Label)s).BackColor = Color.White;

            label4.MouseEnter += (s, _) => ((Label)s).BackColor = Color.FromArgb(245, 245, 245);
            label4.MouseLeave += (s, _) => ((Label)s).BackColor = Color.White;

            label5.MouseEnter += (s, _) => ((Label)s).BackColor = Color.FromArgb(245, 245, 245);
            label5.MouseLeave += (s, _) => ((Label)s).BackColor = Color.White;

            label10.MouseEnter += (s, _) => ((Label)s).BackColor = Color.FromArgb(245, 245, 245);
            label10.MouseLeave += (s, _) => ((Label)s).BackColor = Color.White;

            label3.Click += (_, __) => InfoTabindexChange(0);
            label4.Click += (_, __) => InfoTabindexChange(1);
            label5.Click += (_, __) => InfoTabindexChange(2);
            label10.Click += (_, __) => InfoTabindexChange(3);
        }

        private void InfoTabindexChange(int tabIndex)
        {
            if (!_carouselst || tabIndex == _infoTabindex)
                return;
            _carouselst = false;

            _pageIndex = 1;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            textBox1.Text = "";

            switch (_infoTabindex)
            {
                case 0:
                    label3.ForeColor = Color.FromArgb(138, 138, 138);
                    label3.Image = Resources.stu_unfocus;
                    break;
                case 1:
                    label4.ForeColor = Color.FromArgb(138, 138, 138);
                    label4.Image = Resources.tea_unfocus;
                    break;
                case 2:
                    label5.ForeColor = Color.FromArgb(138, 138, 138);
                    label5.Image = Resources.group_unfocus;
                    break;
                case 3:
                    label10.ForeColor = Color.FromArgb(138, 138, 138);
                    label10.Image = Resources.allot_unfocus;
                    break;
            }

            int nowTop = label17.Top;

            Timer t = new Timer
            {
                Enabled = true,
                Interval = 10
            };

            t.Tick += (_, __) =>
            {
                if (35 * (tabIndex - _infoTabindex) + nowTop != label17.Top)
                {
                    label17.Top += 5 * (tabIndex - _infoTabindex);
                }
                else
                {
                    _infoTabindex = tabIndex;
                    _carouselst = true;
                    t.Dispose();
                }
            };

            panel2.Visible = false;

            switch (tabIndex)
            {
                case 0:
                    label3.ForeColor = Color.FromArgb(19, 34, 122);
                    label3.Image = Resources.stu_focus;

                    _count = (int)(long)_stuFunc.GetOnlyContent("", "count(*)");
                    _stuArrayList =
                        _stuFunc.GetDbContent(
                            $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    Tool.Close(panel1);
                    Tool.Close(panel3);
                    label7.Text = @"学号";
                    label6.Text = @"姓名";
                    panel1.Controls.Add(CreateListBox(_stuArrayList, AddArrayList));
                    panel3.Controls.Add(CreatePagination());
                    break;
                case 1:
                    label4.ForeColor = Color.FromArgb(19, 34, 122);
                    label4.Image = Resources.tea_focus;

                    _count = (int)(long)_teaFunc.GetOnlyContent("", "count(*)");
                    _teaArrayList =
                        _teaFunc.GetDbContent(
                            $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    Tool.Close(panel1);
                    Tool.Close(panel3);
                    label7.Text = @"工号";
                    label6.Text = @"姓名";
                    label8.Text = @"年级";
                    label9.Text = @"专业";
                    panel1.Controls.Add(CreateListBox(_teaArrayList, AddArrayList));
                    panel3.Controls.Add(CreatePagination());
                    break;
                case 2:
                    label5.ForeColor = Color.FromArgb(19, 34, 122);
                    label5.Image = Resources.group_focus;

                    _count = (int)(long)_groupFunc.GetOnlyContent("", "count(*)");
                    _groupArrayList =
                        _groupFunc.GetGroupInfo(
                            $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    Tool.Close(panel1);
                    Tool.Close(panel3);
                    label7.Text = @"组号";
                    label6.Text = @"组名";
                    label8.Text = @"年级";
                    label9.Text = @"专业";
                    panel1.Controls.Add(CreateListBox(_groupArrayList, AddArrayList));
                    panel3.Controls.Add(CreatePagination());
                    break;
                case 3:
                    label10.ForeColor = Color.FromArgb(19, 34, 122);
                    label10.Image = Resources.allot_focus;

                    _count = (int)(long)_autodispenseFunc.GetOnlyContent("state = 0", "count(*)");

                    _autodispenseList =
                        _autodispenseFunc.GetDbContent(
                            $@"state = 0 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    Tool.Close(panel1);
                    Tool.Close(panel3);

                    label7.Text = @"组名";
                    label6.Text = @"教师名";
                    label8.Text = @"匹配分";
                    label9.Text = @"状态";
                    panel2.Visible = true;
                    panel1.Controls.Add(CreateListBox(_autodispenseList, AddArrayList));
                    panel3.Controls.Add(CreatePagination());
                    break;
            }
        }

        private void AdmUserInfo_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(AppSettings["Grade"].Split(','));
            comboBox2.Items.AddRange(AppSettings["Speciality"].Split(','));
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            panel1.Controls.Add(CreateListBox(_stuArrayList, AddArrayList));
            panel3.Controls.Add(CreatePagination());
        }


        private Panel CreatePagination()
        {
            Panel p = new Panel
            {
                Location = new Point(0, 0),
                AutoSize = true
            };

            int pageNum = _count / PageSize + (_count % PageSize == 0 ? 0 : 1);

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

                switch (_infoTabindex)
                {
                    case 0:
                        _stuArrayList = _stuFunc.GetDbContent(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                            textBox1.Text,
                            $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                        Tool.Close(panel1);
                        panel1.Controls.Add(CreateListBox(_stuArrayList, AddArrayList));
                        break;
                    case 1:
                        _teaArrayList = _teaFunc.GetDbContent(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                            textBox1.Text,
                            $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                        Tool.Close(panel1);
                        panel1.Controls.Add(CreateListBox(_teaArrayList, AddArrayList));
                        break;
                    case 2:
                        _groupArrayList = _groupFunc.GetGroupInfo(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                            textBox1.Text,
                            $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                        Tool.Close(panel1);
                        panel1.Controls.Add(CreateListBox(_groupArrayList, AddArrayList));
                        break;
                    case 3:
                        _autodispenseList =
                            _autodispenseFunc.GetDbContent(
                                $@"state = 0 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                        Tool.Close(panel1);


                        panel1.Controls.Add(CreateListBox(_autodispenseList, AddArrayList));
                        break;
                }

                Tool.Close(panel3);
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

        private static Panel CreateListBox<T>(IReadOnlyList<T> valuesArr, Func<int, T, Panel> func)
        {
            Panel p = new Panel
            {
                Location = new Point(0, 0),
                AutoSize = true
            };

            for (int i = 0; i < valuesArr.Count; i++) p.Controls.Add(func(i, valuesArr[i]));
            return p;
        }

        private Panel AddArrayList<T>(int index, T values)
        {
            string etr = "";
            if (typeof(T) == typeof(stu_info))
                etr = "s";
            else if (typeof(T) == typeof(tea_info))
                etr = "t";
            else if (typeof(T) == typeof(group_info))
                etr = "g";


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
            divider.Location = new Point(0, 0);
            divider.Size = new Size(390, 1);

            p.Location = new Point(0, 0 + index * 36);
            p.Size = new Size(390, 36);
            p.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            p.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i2.AutoSize = true;
            i2.BackColor = Color.Transparent;
            i2.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i2.ForeColor = Color.FromArgb(96, 98, 102);
            i2.Location = new Point(0, 10); //49
            i2.Text = typeof(T).GetProperty(etr + "_name")?.GetValue(values, null).ToString();
            i2.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i2.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i3.AutoSize = true;
            i3.BackColor = Color.Transparent;
            i3.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i3.ForeColor = Color.FromArgb(96, 98, 102);
            i3.Location = new Point(68, 10); //111
            i3.Text = typeof(T).GetProperty(etr + "_id")?.GetValue(values, null).ToString();
            i3.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i3.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i4.AutoSize = true;
            i4.BackColor = Color.Transparent;
            i4.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i4.ForeColor = Color.FromArgb(96, 98, 102);
            i4.Location = new Point(144, 10);
            i4.Text = typeof(T).GetProperty("grade")?.GetValue(values, null).ToString();
            i4.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i4.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i5.AutoSize = true;
            i5.BackColor = Color.Transparent;
            i5.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i5.ForeColor = Color.FromArgb(96, 98, 102);
            i5.Location = new Point(197, 10);
            i5.Text = AppSettings["Speciality"].Split(',')[
                (int)typeof(T).GetProperty("major_id")?.GetValue(values, null) + 1];
            i5.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i5.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i1.Cursor = Cursors.Hand;
            i1.BackColor = Color.Transparent;
            i1.Size = new Size(16, 16);
            i1.Location = new Point(350, 10);
            i1.Image = ReturnRandomColor((int)typeof(T).GetProperty(etr + "_no")?.GetValue(values, null));
            i1.ImageAlign = ContentAlignment.MiddleCenter;
            i1.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i1.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
            i1.Click += (sender, e) =>
            {
                if (typeof(T) == typeof(stu_info))
                    new AdminChangeInfo(((stu_info)(object)values).s_id, nowValues =>
                    {
                        if (nowValues == default)
                        {
                            button4_Click(default, default);
                            return;
                        }

                        i2.Text = nowValues.s_name;
                        i3.Text = nowValues.s_id.ToString();
                        i4.Text = nowValues.grade.ToString();
                        i5.Text = AppSettings["Speciality"].Split(',')[nowValues.major_id + 1];
                    }).Show();
                else if (typeof(T) == typeof(tea_info))
                    new AdminChangeInfo(((tea_info)(object)values).t_id, nowValues =>
                    {
                        if (nowValues == default)
                        {
                            button4_Click(default, default);
                            return;
                        }

                        i2.Text = nowValues.t_name;
                        i3.Text = nowValues.t_id.ToString();
                        i4.Text = nowValues.grade.ToString();
                        i5.Text = AppSettings["Speciality"].Split(',')[nowValues.major_id + 1];
                    }).Show();
                else
                    new AdminChangeInfo(((group_info)(object)values).g_id, nowValues =>
                    {
                        if (nowValues == default)
                        {
                            button4_Click(default, default);
                            return;
                        }

                        i2.Text = nowValues.g_name;
                        i3.Text = nowValues.g_id.ToString();
                        i4.Text = nowValues.grade.ToString();
                        i5.Text = AppSettings["Speciality"].Split(',')[nowValues.major_id + 1];
                    }).Show();
            };


            p.Controls.Add(i1);
            p.Controls.Add(i2);
            p.Controls.Add(i3);
            p.Controls.Add(i4);
            p.Controls.Add(i5);

            p.Controls.Add(divider);

            return p;
        }

        private Panel AddArrayList(int index, autodispense_tmp values)
        {
            Panel p = new Panel();
            Label divider = new Label();

            Label i1 = new Label();
            Label i2 = new Label();
            Label i3 = new Label();
            Label i4 = new Label();
            Label i5 = new Label();
            Label i6 = new Label();
            
            void Ml(object sender, EventArgs e)
            {
                p.BackColor = Color.White;
            }

            void Me(object sender, EventArgs e)
            {
                p.BackColor = Color.FromArgb(245, 247, 250);
            }

            divider.BackColor = Color.Gainsboro;
            divider.Location = new Point(0, 0);
            divider.Size = new Size(390, 1);

            p.Location = new Point(0, 0 + index * 36);
            p.Size = new Size(390, 36);
            p.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            p.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i2.AutoSize = true;
            i2.BackColor = Color.Transparent;
            i2.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i2.ForeColor = Color.FromArgb(96, 98, 102);
            i2.Location = new Point(0, 10); //49
            i2.Text = values.g_name;
            i2.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i2.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i3.AutoSize = true;
            i3.BackColor = Color.Transparent;
            i3.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i3.ForeColor = Color.FromArgb(96, 98, 102);
            i3.Location = new Point(68, 10); //111
            i3.Text = values.t_name;
            i3.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i3.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i4.AutoSize = true;
            i4.BackColor = Color.Transparent;
            i4.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i4.ForeColor = Color.FromArgb(96, 98, 102);
            i4.Location = new Point(144, 10);
            i4.Text = values.score.ToString(CultureInfo.InvariantCulture);
            i4.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i4.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i5.AutoSize = true;
            i5.BackColor = Color.Transparent;
            i5.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i5.ForeColor = Color.FromArgb(96, 98, 102);
            i5.Location = new Point(197, 10);
            i5.Text = values.state.ToString() == "0" ? "未审核" : "已审核";
            i5.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i5.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i1.Cursor = Cursors.Hand;
            i1.BackColor = Color.Transparent;
            i1.Size = new Size(16, 16);
            i1.Location = new Point(350, 10);
            i1.Image = ReturnRandomColor(Convert.ToInt32(values.score), Resources.确定);
            i1.ImageAlign = ContentAlignment.MiddleCenter;
            i1.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i1.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
            i1.Click += (sender, e) =>
            {
                if (new Update().DispenseOkAudit(values.g_id))
                    Message.ShowSuccess("变更成功");

                _autodispenseList =
                    _autodispenseFunc.GetDbContent(
                        $@"state = 0 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                Tool.Close(panel1);


                panel1.Controls.Add(CreateListBox(_autodispenseList, AddArrayList));
            };

            i6.Cursor = Cursors.Hand;
            i6.BackColor = Color.Transparent;
            i6.Size = new Size(16, 16);
            i6.Location = new Point(320, 10);
            i6.Image = ReturnRandomColor(Convert.ToInt32(values.score), Resources.关闭);
            i6.ImageAlign = ContentAlignment.MiddleCenter;
            i6.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i6.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
            i6.Click += (sender, e) =>
            {
                if (new Update().DispenseDelAudit(values.g_id))
                    Message.ShowSuccess("删除成功");
                
                _autodispenseList =
                    _autodispenseFunc.GetDbContent(
                        $@"state = 0 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                Tool.Close(panel1);
                panel1.Controls.Add(CreateListBox(_autodispenseList, AddArrayList));
                
            };

            p.Controls.Add(i1);
            p.Controls.Add(i2);
            p.Controls.Add(i3);
            p.Controls.Add(i4);
            p.Controls.Add(i5);
            p.Controls.Add(i6);
            
            p.Controls.Add(divider);

            return p;
        }


        private static Bitmap ReturnRandomColor(int i, Bitmap img = null)
        {
            int[,] co =
            {
                { 254, 82, 86 },
                { 254, 167, 49 },
                { 248, 216, 102 },
                { 153, 240, 243 },
                { 0, 165, 231 },
                { 87, 35, 7 }
            };
            if (img == null)
                img = Resources.编辑;
            int col = i % 6;

            for (int x = 0; x < img.Width; x++)
                for (int y = 0; y < img.Height; y++)
                {
                    int alp = img.GetPixel(x, y).A;
                    if (alp == 0)
                        continue;
                    img.SetPixel(x, y, Color.FromArgb(alp, co[col, 0], co[col, 1], co[col, 2]));
                }

            return img;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (sender != default)
                _pageIndex = 1;

            switch (_infoTabindex)
            {
                case 0:
                    _count = (int)(long)_stuFunc.GetConditionContent("", "count(*)", comboBox1.SelectedIndex,
                        comboBox2.SelectedIndex, textBox1.Text);
                    _stuArrayList = _stuFunc.GetDbContent(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                        textBox1.Text,
                        $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    Tool.Close(panel1);
                    panel1.Controls.Add(CreateListBox(_stuArrayList, AddArrayList));
                    break;
                case 1:
                    _count = (int)(long)_teaFunc.GetConditionContent("", "count(*)", comboBox1.SelectedIndex,
                        comboBox2.SelectedIndex, textBox1.Text);
                    _teaArrayList = _teaFunc.GetDbContent(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                        textBox1.Text,
                        $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    Tool.Close(panel1);
                    panel1.Controls.Add(CreateListBox(_teaArrayList, AddArrayList));
                    break;
                case 2:
                    _count = _groupFunc
                        .GetGroupInfo(comboBox1.SelectedIndex, comboBox2.SelectedIndex, textBox1.Text, "").Count;
                    _groupArrayList = _groupFunc.GetGroupInfo(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                        textBox1.Text,
                        $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    Tool.Close(panel1);
                    panel1.Controls.Add(CreateListBox(_groupArrayList, AddArrayList));
                    break;
            }

            Tool.Close(panel3);
            panel3.Controls.Add(CreatePagination());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (_infoTabindex)
            {
                case 2:
                    MessageBox.Show(
                        "无法直接导入/新建队伍,因为小组需要组长的存在,如需新建队伍,请通过 \n  < 学生信息 —> 编辑 —> 新建队伍 >\n( 所有身份为组长且尚未拥有队伍的学生均可新建队伍 ) ",
                        @"无法导入", MessageBoxButtons.OK);
                    return;
                case 3:
                    Message.ShowWarn("此项无法直接导入/新建");
                    return;
                default:
                    new AdminChannelSelect(_infoTabindex).Show();
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (_infoTabindex)
            {
                case 0:
                    new AdminPrint(_stuArrayList).Show();
                    break;
                case 1:
                    new AdminPrint(_teaArrayList).Show();
                    break;
                case 2:
                    new AdminPrint(_groupArrayList).Show();
                    break;
            }
        }
    }
}