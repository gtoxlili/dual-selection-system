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
    public sealed partial class CreateGroup : Form
    {
        private const int PageSize = 20;
        private readonly Action<List<stu_info>> _changeFatherInfo;
        private readonly Action _changeFatherTab;
        private readonly bool _isChooseTea;
        private readonly Read<stu_info> _stuFunc = new Read<stu_info>();
        private readonly List<stu_info> _stuSelectedList;
        private readonly Read<tea_info> _teaFunc = new Read<tea_info>();
        private readonly List<tea_info> _teaSelectedList;
        private readonly Update _up = new Update();
        private int _pageIndex = 1;
        private List<stu_info> _stuUnSelectList;
        private List<tea_info> _teaUnSelectList;
        private int _unSelectCount;

        public CreateGroup()
        {
            List<tea_info> Getseletedtea()
            {
                string term = new Read<group_info>()
                    .GetOnlyContent($"g_id={stu_base_info.g_id}", "('('||c1||','||c2||','||c3||')')").ToString();
                return _teaFunc.GetDbContent($"t_id in {term}");
            }

            _unSelectCount = (int)(long)_teaFunc.GetOnlyContent("", "count(*)");

            _teaUnSelectList = _teaFunc.GetDbContent($@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");
            _teaSelectedList = Getseletedtea();

            List<tea_info> unSelectListTemp = new List<tea_info>(_teaUnSelectList);

            foreach (tea_info t1 in unSelectListTemp.Where(t1 => _teaSelectedList.Exists(t => t.t_id == t1.t_id)))
                _teaUnSelectList.Remove(t1);

            _isChooseTea = true;

            InitializeComponent();

            label7.Text = @"工号";
            label16.Text = @"工号";
            Text = @"选择导师";
            label4.Text = @"待选导师";
            label19.Text = @"已选导师";
            button1.Text = @"确定导师";
        }

        public CreateGroup(Action changeFatherTab)
        {
            _changeFatherTab = changeFatherTab;
            _unSelectCount = (int)(long)_stuFunc.GetOnlyContent("role !=1 and g_id is null", "count(*)");
            _stuUnSelectList =
                _stuFunc.GetDbContent(
                    $@"role !=1 and g_id is null limit {PageSize} offset {(_pageIndex - 1) * PageSize}");
            _stuSelectedList = new List<stu_info>();
            InitializeComponent();
            Text = @"创建小组";
        }

        public CreateGroup(Action<List<stu_info>> changeFatherInfo)
        {
            _changeFatherInfo = changeFatherInfo;
            _unSelectCount = (int)(long)_stuFunc.GetOnlyContent("role !=1 and g_id is null", "COUNT(*)");
            _stuUnSelectList =
                _stuFunc.GetDbContent(
                    $@"role !=1 and g_id is null limit {PageSize} offset {(_pageIndex - 1) * PageSize}");
            _stuSelectedList = _stuFunc.GetDbContent("role != 1 and g_id=" + stu_base_info.g_id);
            InitializeComponent();
            Text = @"修改小组";
        }

        private void CreateGroup_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(AppSettings["Grade"].Split(','));
            comboBox2.Items.AddRange(AppSettings["Speciality"].Split(','));
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            panel1.Controls.Add(CreateListBox(0));
            panel2.Controls.Add(CreateListBox(1));
            panel3.Controls.Add(CreatePagination());
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


                if (_isChooseTea)
                {
                    _teaUnSelectList = _teaFunc.GetDbContent(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                        textBox1.Text, $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    List<tea_info> unSelectListTemp = new List<tea_info>(_teaUnSelectList);

                    foreach (tea_info t1 in unSelectListTemp.Where(
                                 t1 => _teaSelectedList.Exists(t => t.t_id == t1.t_id)))
                        _teaUnSelectList.Remove(t1);

                    toolTip1.RemoveAll();
                }
                else
                {
                    _stuUnSelectList = _stuFunc.GetDbContent(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                        textBox1.Text,
                        $@"role !=1 and (g_id is null  or g_id ={stu_base_info.g_id}) limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                    List<stu_info> unSelectListTemp = new List<stu_info>(_stuUnSelectList);

                    foreach (stu_info t1 in unSelectListTemp.Where(
                                 t1 => _stuSelectedList.Exists(t => t.s_id == t1.s_id)))
                        _stuUnSelectList.Remove(t1);
                }

                Tool.Close(panel1);
                Tool.Close(panel3);


                panel1.Controls.Add(CreateListBox(0));
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

        private Panel CreateListBox(int status)
        {
            Panel p = new Panel
            {
                Location = new Point(0, 0),
                AutoSize = true
            };

            if (_isChooseTea)
            {
                List<tea_info> db = status.Equals(0) ? _teaUnSelectList : _teaSelectedList;
                for (int i = 0; i < db.Count; i++) p.Controls.Add(AddTeaList(i, db[i], status));
            }
            else
            {
                List<stu_info> db = status.Equals(0) ? _stuUnSelectList : _stuSelectedList;
                for (int i = 0; i < db.Count; i++) p.Controls.Add(AddStuList(i, db[i], status));
            }


            return p;
        }

        private Panel AddStuList(int index, stu_info content, int status)
        {
            int[] xlocation = status.Equals(0) ? new[] { 0, 62, 144, 197, 350 } : new[] { 49, 111, 193, 246, 0 };


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

            Color fontColor = index >= 3 && status == 1 ? Color.Red : Color.FromArgb(96, 98, 102);

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
            i2.ForeColor = fontColor;
            i2.Location = new Point(xlocation[0], 10); //49
            i2.Text = content.s_name;
            i2.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i2.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i3.AutoSize = true;
            i3.BackColor = Color.Transparent;
            i3.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i3.ForeColor = fontColor;
            i3.Location = new Point(xlocation[1], 10); //111
            i3.Text = content.s_id.ToString();
            i3.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i3.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i4.AutoSize = true;
            i4.BackColor = Color.Transparent;
            i4.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i4.ForeColor = fontColor;
            i4.Location = new Point(xlocation[2], 10);
            i4.Text = content.grade.ToString();
            i4.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i4.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i5.AutoSize = true;
            i5.BackColor = Color.Transparent;
            i5.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i5.ForeColor = fontColor;
            i5.Location = new Point(xlocation[3], 10); //246
            i5.Text = AppSettings["Speciality"].Split(',')[content.major_id + 1];
            i5.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i5.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i1.AutoSize = true;
            i1.Cursor = Cursors.Hand;
            i1.BackColor = Color.Transparent;
            i1.Font = new Font(CustomFont.Font.bPFC.Families[0], 10F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i1.ForeColor = ReturnRandomColor(content.s_no);
            i1.Location = new Point(xlocation[4], 8);
            i1.Text = status.Equals(0) ? "▶" : "◀";
            i1.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i1.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
            i1.Click += (sender, e) =>
            {
                if (status.Equals(0))
                {
                    _stuUnSelectList.RemoveAt(index);
                    _stuSelectedList.Add(content);
                }
                else
                {
                    _stuUnSelectList.Add(content);
                    _stuSelectedList.RemoveAt(index);
                }

                _stuUnSelectList.Sort((x, y) => x.s_no.CompareTo(y.s_no));
                //_stu_selectedList.Sort((x, y) => x.s_no.CompareTo(y.s_no));

                Tool.Close(panel1);
                Tool.Close(panel2);


                panel1.Controls.Add(CreateListBox(0));
                panel2.Controls.Add(CreateListBox(1));
            };

            p.Controls.Add(i1);
            p.Controls.Add(i2);
            p.Controls.Add(i3);
            p.Controls.Add(i4);
            p.Controls.Add(i5);
            p.Controls.Add(divider);

            return p;
        }

        private Panel AddTeaList(int index, tea_info content, int status)
        {
            int[] xlocation = status.Equals(0) ? new[] { 0, 62, 144, 197, 350 } : new[] { 49, 111, 193, 246, 0 };


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

            Color fontColor = index >= 3 && status == 1 ? Color.Red : Color.FromArgb(96, 98, 102);

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
            i2.ForeColor = fontColor;
            i2.Location = new Point(xlocation[0], 10); //49
            i2.Text = content.t_name;
            i2.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i2.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i3.AutoSize = true;
            i3.BackColor = Color.Transparent;
            i3.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i3.ForeColor = fontColor;
            i3.Location = new Point(xlocation[1], 10); //111
            i3.Text = content.t_id.ToString();
            i3.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i3.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i4.AutoSize = true;
            i4.BackColor = Color.Transparent;
            i4.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i4.ForeColor = fontColor;
            i4.Location = new Point(xlocation[2], 10);
            i4.Text = content.grade.ToString();
            i4.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i4.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i5.AutoSize = true;
            i5.BackColor = Color.Transparent;
            i5.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i5.ForeColor = fontColor;
            i5.Location = new Point(xlocation[3], 10); //246
            i5.Text = AppSettings["Speciality"].Split(',')[content.major_id + 1];
            i5.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i5.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i1.AutoSize = true;
            i1.Cursor = Cursors.Hand;
            i1.BackColor = Color.Transparent;
            i1.Font = new Font(CustomFont.Font.bPFC.Families[0], 10F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i1.ForeColor = ReturnRandomColor(content.t_no);
            i1.Location = new Point(xlocation[4] + 12, 8);
            i1.Text = status.Equals(0) ? "▶" : "◀";
            i1.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i1.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
            i1.Click += (sender, e) =>
            {
                if (status.Equals(0))
                {
                    _teaUnSelectList.RemoveAt(index);
                    _teaSelectedList.Add(content);
                }
                else
                {
                    _teaUnSelectList.Add(content);
                    _teaSelectedList.RemoveAt(index);
                }

                _teaUnSelectList.Sort((x, y) => x.t_no.CompareTo(y.t_no));
                //_stu_selectedList.Sort((x, y) => x.s_no.CompareTo(y.s_no));
                toolTip1.RemoveAll();
                Tool.Close(panel1);
                Tool.Close(panel2);


                panel1.Controls.Add(CreateListBox(0));
                panel2.Controls.Add(CreateListBox(1));
            };

            if (status.Equals(0))
            {
                Label i6 = new Label();

                i6.Cursor = Cursors.Hand;
                i6.BackColor = Color.Transparent;
                i6.Size = new Size(16, 16);
                i6.Location = new Point(xlocation[4] - 12, 10);
                i6.Image = Resources.信息;
                i6.ImageAlign = ContentAlignment.MiddleCenter;
                i6.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
                i6.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);


                string direction = content.direction == null
                    ? "\n该导师暂无课题方向" + content.t_name
                    : "\n" + content.direction;
                string tt = "";
                for (int i = 0; i < direction.Length; i++)
                {
                    if (i != 0 && i % 20 == 0) tt += "\n";
                    tt += direction.Substring(i, 1);
                }

                toolTip1.SetToolTip(i6, tt);
                p.Controls.Add(i6);
            }


            p.Controls.Add(i1);
            p.Controls.Add(i2);
            p.Controls.Add(i3);
            p.Controls.Add(i4);
            p.Controls.Add(i5);

            p.Controls.Add(divider);

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

            if (_isChooseTea)
            {
                _unSelectCount = (int)(long)_teaFunc.GetConditionContent("", "count(*)", comboBox1.SelectedIndex,
                    comboBox2.SelectedIndex, textBox1.Text);

                _teaUnSelectList = _teaFunc.GetDbContent(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                    textBox1.Text,
                    $@"1=1 limit {PageSize} offset {(_pageIndex - 1) * PageSize}");
                List<tea_info> unSelectListTemp = new List<tea_info>(_teaUnSelectList);

                foreach (tea_info t1 in unSelectListTemp.Where(t1 => _teaSelectedList.Exists(t => t.t_id == t1.t_id)))
                    _teaUnSelectList.Remove(t1);

                toolTip1.RemoveAll();
            }

            else
            {
                _unSelectCount = (int)(long)_stuFunc.GetConditionContent(
                    $"role !=1 and (g_id is null or g_id ={stu_base_info.g_id})", "count(*)", comboBox1.SelectedIndex,
                    comboBox2.SelectedIndex, textBox1.Text);

                _stuUnSelectList = _stuFunc.GetDbContent(comboBox1.SelectedIndex, comboBox2.SelectedIndex,
                    textBox1.Text,
                    $@"role != 1 and (g_id is null or g_id ={stu_base_info.g_id}) limit {PageSize} offset {(_pageIndex - 1) * PageSize}");

                List<stu_info> unSelectListTemp = new List<stu_info>(_stuUnSelectList);

                foreach (stu_info t1 in unSelectListTemp.Where(t1 => _stuSelectedList.Exists(t => t.s_id == t1.s_id)))
                    _stuUnSelectList.Remove(t1);
            }

            Tool.Close(panel1);
            panel1.Controls.Add(CreateListBox(0));

            Tool.Close(panel3);
            panel3.Controls.Add(CreatePagination());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_isChooseTea)
            {
                if (_teaSelectedList.Count == 0)
                {
                    Message.ShowWarn(@"已选择的导师为空！", this);
                    return;
                }

                if (_teaSelectedList.Count > 3)
                {
                    Message.ShowWarn(@"最多只能选择3个导师，请修改后进行再提交。", this);
                    return;
                }

                if (MessageBox.Show(@"确定要提交吗？", @"提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) !=
                    DialogResult.OK) return;

                int[] tidArr = new int[3];
                for (int i = 0; i < _teaSelectedList.Count; i++) tidArr[i] = _teaSelectedList[i].t_id;
                if (_up.UpdateChooseTeaInfo(tidArr, stu_base_info.g_id.ToString()))
                {
                    MessageBox.Show(@"提交成功！", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    Message.ShowError(@"提交失败！", this);
                }
            }
            else
            {
                if (_stuSelectedList.Count == 0)
                {
                    Message.ShowWarn(@"已选择的学生为空！", this);
                    return;
                }

                if (_stuSelectedList.Count > 3)
                {
                    Message.ShowWarn(@"最多只能选择3个学生，请修改后进行再提交。", this);
                    return;
                }

                if (MessageBox.Show(@"确定要提交吗？", @"提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) !=
                    DialogResult.OK) return;

                string gId = _changeFatherTab != null ? _up.CreateGroup() : stu_base_info.g_id.ToString();

                int[] sidArr = new int[_stuSelectedList.Count + 1];
                for (int i = 0; i < _stuSelectedList.Count; i++) sidArr[i] = _stuSelectedList[i].s_id;
                sidArr[_stuSelectedList.Count] = stu_base_info.s_id;

                if (_up.UpdateGidInfo(sidArr, gId))
                {
                    MessageBox.Show(@"提交成功！", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (_changeFatherTab != null)
                        _changeFatherTab();
                    else
                        _changeFatherInfo(_stuSelectedList);
                    Close();
                }
                else
                {
                    Message.ShowError(@"提交失败！", this);
                }
            }
        }
    }
}