using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using 师生双选系统.Properties;
using Message = UI.Message;

namespace 师生双选系统
{
    public partial class AdmAutoDispense : Form
    {
        private const int PageSize = 20;
        private readonly List<Action> _queue;

        private readonly Read<admin_config> _read = new Read<admin_config>();
        private readonly Update _up = new Update();
        private DataSet _allDt;
        private int _count;
        private DataTable _dt;
        private int _pageIndex = 1;

        public AdmAutoDispense()
        {
            _queue = new List<Action>();
            InitializeComponent();
        }

        private void AdmAutoDispense_Load(object sender, EventArgs e)
        {
            _allDt = _read.GetAutoDispenseDt();
            _dt = _allDt.Tables[0].AsEnumerable().Skip((_pageIndex - 1) * PageSize).Take(PageSize).CopyToDataTable();
            _count = _allDt.Tables[0].Rows.Count;
            panel1.Controls.Add(CreateListBox(_dt));
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


                _dt = _allDt.Tables[0].AsEnumerable().Skip((_pageIndex - 1) * PageSize).Take(PageSize)
                    .CopyToDataTable();

                Tool.Close(panel1);
                panel1.Controls.Add(CreateListBox(_dt));

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

        private Panel CreateListBox(DataTable valuesArr)
        {
            Panel p = new Panel
            {
                Location = new Point(0, 0),
                AutoSize = true
            };

            for (int i = 0; i < valuesArr.Rows.Count; i++) p.Controls.Add(AddArrayList(i, valuesArr.Rows[i]));
            return p;
        }

        private Panel AddArrayList(int index, DataRow values)
        {
            Panel p = new Panel();
            Label divider = new Label();

            Label i1 = new Label();
            Label i2 = new Label();
            Label i3 = new Label();
            Label i4 = new Label();
            Label i5 = new Label();
            Label i6 = new Label();
            Label i7 = new Label();
            Label i8 = new Label();
            Label i9 = new Label();
            Label i10 = new Label();
            Label i11 = new Label();
            Label i12 = new Label();

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
            divider.Size = new Size(535, 1);

            p.Location = new Point(0, 0 + index * 36);
            p.Size = new Size(535, 36);
            p.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            p.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i1.AutoSize = true;
            i1.BackColor = Color.Transparent;
            i1.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i1.ForeColor = Color.FromArgb(96, 98, 102);
            i1.Location = new Point(0, 10); //49
            i1.Text = values["g_name"].ToString();
            i1.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i1.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i2.AutoSize = true;
            i2.BackColor = Color.Transparent;
            i2.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i2.ForeColor = Color.FromArgb(96, 98, 102);
            i2.Location = new Point(73, 10); //49
            i2.Text = values["t_name"].ToString();
            i2.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i2.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i3.AutoSize = true;
            i3.BackColor = Color.Transparent;
            i3.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i3.ForeColor = Color.FromArgb(96, 98, 102);
            i3.Location = new Point(124, 10); //111
            i3.Text = values["g_c1"].ToString();
            i3.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i3.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i4.AutoSize = true;
            i4.BackColor = Color.Transparent;
            i4.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i4.ForeColor = Color.FromArgb(96, 98, 102);
            i4.Location = new Point(161, 10);
            i4.Text = values["g_c2"].ToString();
            i4.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i4.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i5.AutoSize = true;
            i5.BackColor = Color.Transparent;
            i5.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i5.ForeColor = Color.FromArgb(96, 98, 102);
            i5.Location = new Point(198, 10);
            i5.Text = values["g_c3"].ToString();
            i5.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i5.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i6.AutoSize = true;
            i6.BackColor = Color.Transparent;
            i6.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i6.ForeColor = Color.FromArgb(96, 98, 102);
            i6.Location = new Point(235, 10);
            i6.Text = values["t_c1"].ToString();
            i6.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i6.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i7.AutoSize = true;
            i7.BackColor = Color.Transparent;
            i7.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i7.ForeColor = Color.FromArgb(96, 98, 102);
            i7.Location = new Point(272, 10);
            i7.Text = values["t_c2"].ToString();
            i7.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i7.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i8.AutoSize = true;
            i8.BackColor = Color.Transparent;
            i8.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i8.ForeColor = Color.FromArgb(96, 98, 102);
            i8.Location = new Point(309, 10);
            i8.Text = values["t_c3"].ToString();
            i8.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i8.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i9.AutoSize = true;
            i9.BackColor = Color.Transparent;
            i9.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i9.ForeColor = Color.FromArgb(96, 98, 102);
            i9.Location = new Point(346, 10);
            i9.Text = values["t_c4"].ToString();
            i9.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i9.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i10.AutoSize = true;
            i10.BackColor = Color.Transparent;
            i10.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i10.ForeColor = Color.FromArgb(96, 98, 102);
            i10.Location = new Point(383, 10);
            i10.Text = values["t_c5"].ToString();
            i10.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i10.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i11.AutoSize = true;
            i11.BackColor = Color.Transparent;
            i11.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i11.ForeColor = Color.FromArgb(96, 98, 102);
            i11.Location = new Point(426, 10);
            i11.Text = values["score"].ToString();
            i11.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i11.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);

            i12.AutoSize = true;
            i12.BackColor = Color.Transparent;
            i12.Font = new Font(CustomFont.Font.bPFC.Families[0], 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            i12.ForeColor = Color.FromArgb(96, 98, 102);
            i12.Location = new Point(463, 10);
            i12.Text = values["assigned"].ToString();
            i12.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i12.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);


            Label i13 = new Label();

            i13.Cursor = Cursors.Hand;
            i13.BackColor = Color.Transparent;
            i13.Size = new Size(16, 16);
            i13.Location = new Point(505, 10);
            i13.Image = ReturnRandomColor(Convert.ToInt32(values["score"]));
            i13.ImageAlign = ContentAlignment.MiddleCenter;
            i13.MouseEnter += new EventHandler((Action<object, EventArgs>)Me);
            i13.MouseLeave += new EventHandler((Action<object, EventArgs>)Ml);
            i13.Click += (sender, e) =>
            {
                string tidTmp = _allDt.Tables[1].Select("g_id=" + values["g_id"])[0]["t_id"].ToString();
                if (tidTmp == "")
                {
                    tidTmp = "0";
                }
                DataRow[] rowTeacher = _allDt.Tables[2].Select($"res_count < 5 and t_id <> {tidTmp}");
                string nowtea = "";
                string[] teaNameArr;
                if (tidTmp != "0")
                {
                    nowtea = _allDt.Tables[2]
                        .Select("t_id=" + _allDt.Tables[1].Select("g_id=" + values["g_id"])[0]["t_id"])[0]["t_name"]
                        .ToString();

                    teaNameArr = new string[rowTeacher.Length + 1];
                    for (int i = 1; i < rowTeacher.Length + 1; i++) teaNameArr[i] = rowTeacher[i-1]["t_name"].ToString();
                    teaNameArr[0] = nowtea;
                }
                else
                {
                    teaNameArr = new string[rowTeacher.Length];
                    for (int i = 0; i < rowTeacher.Length; i++) teaNameArr[i] = rowTeacher[i]["t_name"].ToString();
                }
                
                
                new AdminChangeInfo(teaNameArr, values["g_name"].ToString(), nowtea, newteaName =>
                {
                    if(newteaName==nowtea)
                        return;

                    DataRow newtea = _allDt.Tables[2].Select("t_name='" + newteaName + "'")[0];
                    int resCount = Convert.ToInt32(newtea["res_count"]);

                    string gName = values["g_name"].ToString();
                    int tId = Convert.ToInt32(newtea["t_id"]);
                    int gId = Convert.ToInt32(values["g_id"]);
                    
                    _queue.Add(() =>
                    {
                        _up.AddRecordtoAutoDisTmp(new autodispense_tmp
                        {
                            t_id = tId,
                            g_id = gId,
                            t_name = newteaName,
                            g_name = gName,
                            score = 0F
                        });
                    });

                    DataRow[] gids = _allDt.Tables[0].Select("g_id=" + gId);
                    foreach (DataRow t in gids)
                        t["assigned"] = 1;

                    newtea["g" + (resCount + 1)] = gId;
                    newtea["res_count"] = resCount + 1;
                    DataRow myGroup = _allDt.Tables[1].Select("g_id=" + gId)[0];
                    myGroup["t_id"] = newtea["t_id"];

                    if (tidTmp != "0")
                    {
                        DataRow oldtea = _allDt.Tables[2].Select("t_id=" + nowtea)[0];

                        var _resCount = Convert.ToInt32(oldtea["res_count"]);
                        for (int i = 1; i <= resCount; i++)
                        {
                            if (Convert.ToInt32(oldtea["g" + i]) == gId)
                            {
                                for (int x = i; x < resCount; x++)
                                {
                                    oldtea["g" + x] = oldtea["g" + (x + 1)];
                                }
                                oldtea["g" + resCount] = -1;
                                oldtea["res_count"] = resCount - 1;
                                break;
                            }
                                
                        }
                    }
                        
                    _dt = _allDt.Tables[0].AsEnumerable().Skip((_pageIndex - 1) * PageSize).Take(PageSize)
                    .CopyToDataTable();
                    Tool.Close(panel1);
                    panel1.Controls.Add(CreateListBox(_dt));
                }).Show();
            };

            p.Controls.Add(i13);


            p.Controls.Add(i1);
            p.Controls.Add(i2);
            p.Controls.Add(i3);
            p.Controls.Add(i4);
            p.Controls.Add(i5);
            p.Controls.Add(i6);
            p.Controls.Add(i7);
            p.Controls.Add(i8);
            p.Controls.Add(i9);
            p.Controls.Add(i10);
            p.Controls.Add(i11);
            p.Controls.Add(i12);

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

        private void button1_Click(object sender, EventArgs e)
        {
            AutoDispense();

            _dt = _allDt.Tables[0].AsEnumerable().Skip((_pageIndex - 1) * PageSize).Take(PageSize).CopyToDataTable();

            Tool.Close(panel1);
            panel1.Controls.Add(CreateListBox(_dt));
        }

        private void AutoDispense()
        {
            for (int i = 0; i < _allDt.Tables[0].Rows.Count; i++)
            {
                DataRow dr = _allDt.Tables[0].Rows[i]; //遍历的每一行记录

                int gid = Convert.ToInt32(dr["g_id"]);
                int tid = Convert.ToInt32(dr["t_id"]);
                int assigned = Convert.ToInt32(dr["assigned"]);
                DataRow rowTeacher = _allDt.Tables[2].Select("t_id=" + tid)[0];
                DataRow rowGroup = _allDt.Tables[1].Select("g_id=" + gid)[0];
                int tRescount = Convert.ToInt32(rowTeacher["res_count"]);

                if (assigned == 1)
                    continue;

                DataRow[] gids = _allDt.Tables[0].Select("g_id=" + gid); //每个队伍在算分表中的所有记录ID

                for (int n = 0; n < 5; n++)
                {
                    if (tRescount == 5)
                        break;

                    int tGchoice = Convert.ToInt32(rowTeacher["g" + (n + 1)]);
                    if (tGchoice != -1) continue;
                    rowTeacher["g" + (n + 1)] = gid;
                    rowTeacher["res_count"] = tRescount + 1;
                    rowGroup["t_id"] = tid;

                    string tName = rowTeacher["t_name"].ToString();
                    string gName = rowGroup["g_name"].ToString();
                    float score = (float)dr["score"];


                    _queue.Add(() =>
                    {
                        _up.AddRecordtoAutoDisTmp(new autodispense_tmp
                        {
                            t_id = tid,
                            g_id = gid,
                            t_name = tName,
                            g_name = gName,
                            score = score
                        });
                    });

                    foreach (DataRow t in gids)
                        t["assigned"] = 1;

                    break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Action item in _queue)
                item();
            Message.ShowSuccess(_queue.Count + "条记录已提交");
        }
    }
}