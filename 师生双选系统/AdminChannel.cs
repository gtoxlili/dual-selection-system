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
    public partial class AdminChannel : Form
    {
        private const int PageSize = 20;

        private readonly int _infoTabindex;

        private int _count;
        private int _pageIndex = 1;

        private List<stu_info> _stuAllArr;
        private List<stu_info> _stuArr;

        private Read<stu_info> _stuFunc;
        private List<tea_info> _teaAllArr;
        private List<tea_info> _teaArr;
        private Read<tea_info> _teaFunc;

        public AdminChannel(int tabindex)
        {
            _infoTabindex = tabindex;
            InitializeComponent();
            switch (_infoTabindex)
            {
                case 0:
                    {
                        label1.Text = @"学生信息导入";

                        Label tip = new Label
                        {
                            Size = new Size(label2.Width, 30),
                            Location = new Point(label2.Location.X, label2.Location.Y - 31),
                            TextAlign = ContentAlignment.MiddleRight,
                            BackColor = Color.White,
                            Font = new Font(CustomFont.Font.rPFC.Families[0], 9F, FontStyle.Regular, GraphicsUnit.Point, 134),
                            ForeColor = Color.Red,
                            Text = "* Execl 表格必须要有 s_name、sex、grade、major_id 字段,\nrole 与 pwd 为可选字段, 若无时 role 为 0,pwd 为 默认密码"
                        };
                        Controls.Add(tip);
                        break;
                    }
                case 1:
                    {
                        label1.Text = @"导师信息导入";

                        Label tip = new Label
                        {
                            Size = new Size(label2.Width, 30),
                            Location = new Point(label2.Location.X, label2.Location.Y - 31),
                            TextAlign = ContentAlignment.MiddleRight,
                            BackColor = Color.White,
                            Font = new Font(CustomFont.Font.rPFC.Families[0], 9F, FontStyle.Regular, GraphicsUnit.Point, 134),
                            ForeColor = Color.Red,
                            Text =
                                "* Execl 表格必须要有 t_name、sex、grade、major_id 字段,\ndirection 与 pwd 为可选字段, 若无时 direction 为 空,pwd 为 默认密码"
                        };
                        Controls.Add(tip);
                        break;
                    }
            }
        }

        private void AdminChannel_Load(object sender, EventArgs e)
        {
            _stuFunc = new Read<stu_info>();
            _teaFunc = new Read<tea_info>();
            comboBox1.Items.AddRange(AppSettings["Grade"].Split(','));
            comboBox2.Items.AddRange(AppSettings["Speciality"].Split(','));
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
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
                        _stuArr = _stuAllArr.Skip((_pageIndex - 1) * PageSize).Take(PageSize).ToList();

                        Tool.Close(panel1);
                        panel1.Controls.Add(CreateListBox(_stuArr, AddArrayList));
                        break;
                    case 1:
                        _teaArr = _teaAllArr.Skip((_pageIndex - 1) * PageSize).Take(PageSize).ToList();

                        Tool.Close(panel1);
                        panel1.Controls.Add(CreateListBox(_teaArr, AddArrayList));
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
            i3.Text = typeof(T).GetProperty("sex")?.GetValue(values, null).ToString();
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
                    new AdminChangeInfo((stu_info)(object)values, nowValues =>
                    {
                        i2.Text = nowValues.s_name;
                        i3.Text = nowValues.sex;
                        i4.Text = nowValues.grade.ToString();
                        i5.Text = AppSettings["Speciality"].Split(',')[nowValues.major_id + 1];

                        for (int i = 0; i < _stuAllArr.Count; i++)
                            if (_stuAllArr[i].s_no == nowValues.s_no)
                            {
                                _stuAllArr[i] = nowValues;
                                break;
                            }
                    }).Show();
                else if (typeof(T) == typeof(tea_info))
                    new AdminChangeInfo((tea_info)(object)values, nowValues =>
                    {
                        i2.Text = nowValues.t_name;
                        i3.Text = nowValues.sex;
                        i4.Text = nowValues.grade.ToString();
                        i5.Text = AppSettings["Speciality"].Split(',')[nowValues.major_id + 1];

                        for (int i = 0; i < _teaAllArr.Count; i++)
                            if (_teaAllArr[i].t_no == nowValues.t_no)
                            {
                                _teaAllArr[i] = nowValues;
                                break;
                            }
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

        private static Bitmap ReturnRandomColor(int i)
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

            Bitmap img = Resources.编辑;
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

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Environment.Is64BitProcess?@".xls、.xlsx|*.xls;*.xlsx": @".xls|*.xls";

            // 将初始路径设置为桌面
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() != DialogResult.OK || ofd.FileNames.Length == 0) return;

            string fileName = ofd.FileName;

            switch (_infoTabindex)
            {
                case 0:
                    {
                        _stuAllArr = _stuFunc.GetExeclContent(fileName, comboBox1.SelectedIndex, comboBox2.SelectedIndex);
                        if (_stuAllArr == default)
                        {
                            Message.ShowWarn("读取失败，请检查关键字/条件是否符合", this);
                            return;
                        }

                        _count = _stuAllArr.Count;
                        _stuArr = _stuAllArr.Skip((_pageIndex - 1) * PageSize).Take(PageSize).ToList();

                        Tool.Close(panel1);
                        panel1.Controls.Add(CreateListBox(_stuArr, AddArrayList));
                        break;
                    }
                case 1:
                    {
                        _teaAllArr = _teaFunc.GetExeclContent(fileName, comboBox1.SelectedIndex, comboBox2.SelectedIndex);
                        if (_stuAllArr == default)
                        {
                            Message.ShowWarn("读取失败，请检查文件是否正确/关键字[条件]是否符合", this);
                            return;
                        }

                        _count = _teaAllArr.Count;
                        _teaArr = _teaAllArr.Skip((_pageIndex - 1) * PageSize).Take(PageSize).ToList();

                        Tool.Close(panel1);
                        panel1.Controls.Add(CreateListBox(_teaArr, AddArrayList));
                        break;
                    }
            }

            Tool.Close(panel3);
            panel3.Controls.Add(CreatePagination());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Update up = new Update();
            switch (_infoTabindex)
            {
                case 0:
                    {
                        foreach (stu_info item in _stuAllArr)
                            up.CreateStuPerson(item);
                        break;
                    }
                case 1:
                    {
                        foreach (tea_info item in _teaAllArr)
                            up.CreateTeaPerson(item);
                        break;
                    }
            }

            Message.ShowSuccess($@"{_count}条数据 批量添加完成！");
            Close();
        }
    }
}