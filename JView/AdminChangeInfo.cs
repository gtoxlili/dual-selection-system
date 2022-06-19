using Bll;
using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Message = UI.Message;

namespace JView
{
    public partial class AdminChangeInfo : Form
    {
        private readonly Dictionary<string, string> _domains = new Dictionary<string, string>();
        private readonly string[] _grade = ConfigurationManager.AppSettings["Grade"].Split(',').Skip(1).ToArray();

        private readonly string[] _speciality =
            ConfigurationManager.AppSettings["Speciality"].Split(',').Skip(1).ToArray();

        private int _paddingTop = 50;

        public AdminChangeInfo(int sid, Action<stu_info> action)
        {
            InitializeComponent();
            Text = @"修改学生信息";
            stu_info stuInfo = new Read<stu_info>().GetEntityValue("s_id=" + sid);
            CreateStuFrom(stuInfo, action);
        }


        public AdminChangeInfo(int tid, Action<tea_info> action)
        {
            InitializeComponent();
            Text = @"修改导师信息";
            tea_info teainfo = new Read<tea_info>().GetEntityValue("t_id=" + tid);
            CreateTeaFrom(teainfo, action);
        }

        public AdminChangeInfo(int gid, Action<group_info> action)
        {
            InitializeComponent();

            Text = @"修改队伍信息";
            group_info groupinfo = new Read<group_info>().GetEntityValue("g_id=" + gid);
            CreateGroupFrom(groupinfo, action);
        }


        public AdminChangeInfo(int infoTabindex)
        {
            InitializeComponent();

            switch (infoTabindex)
            {
                case 0:
                    Text = @"新增学生";
                    CreateNewStuFrom(new stu_info
                    {
                        pwd = admin_config.Instance.defaultPwd
                    }, Convert.ToInt32(new Read<sqlite_sequence>().GetOnlyContent("name = 'stu_info'", "seq")) + 1); ;
                    break;
                case 1:
                    Text = @"新增教师";
                    CreateNewTeaFrom(new tea_info
                    {
                        pwd = admin_config.Instance.defaultPwd
                    }, Convert.ToInt32(new Read<sqlite_sequence>().GetOnlyContent("name = 'tea_info'", "seq")) + 1);
                    break;
            }
        }

        public AdminChangeInfo(stu_info stuinfo, Action<stu_info> action)
        {
            InitializeComponent();
            Text = @"修改学生信息";
            CreateNewStuFrom(stuinfo, stuinfo.s_no, false, action);
        }

        public AdminChangeInfo(tea_info teainfo, Action<tea_info> action)
        {
            InitializeComponent();
            Text = @"修改导师信息";
            CreateNewTeaFrom(teainfo, teainfo.t_no, false, action);
        }

        public AdminChangeInfo(string[] teaArr, string groupName, string nowTea, Action<string> action)
        {
            InitializeComponent();
            Text = $@"分配信息修改( {groupName}队 )";

            Controls.Add(AddcomboBox("导师", "t_name", teaArr, nowTea, "在带队数未满导师中选择", true));

            Controls.Add(AddBtn("确认修改", (_, __) =>
            {
                if (_domains.Count != 0)
                {
                    Message.ShowSuccess("修改成功");
                    action(_domains["t_name"]);
                }

                Close();
            }));
            Height = _paddingTop + 90;
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public void CreateStuFrom(stu_info values, Action<stu_info> action)
        {
            Controls.Add(AddInput("姓名", "s_name", values.s_name));
            Controls.Add(AddInput("学号", "", values.s_id.ToString(), true, "自动生成，不予修改"));
            Controls.Add(AddcomboBox("性别", "sex", new[] { "男", "女" }, values.sex, isitems: true));
            if (values.g_id.Equals(0))
                Controls.Add(AddcomboBox("身份", "role", new[] { "未选", "队长", "队员" },
                    new[] { "未选", "队长", "队员" }[values.role], callback: () =>
                    {
                        stu_info.Instance.s_id = values.s_id;
                        new Update().UpdateStuInfo(_domains["role"], "role");
                        Controls.Clear();
                        _paddingTop = 50;
                        foreach (KeyValuePair<string, string> item in _domains)
                        {
                            object value = item.Value;

                            PropertyInfo p = typeof(stu_info).GetProperty(item.Key);

                            if (p == null) continue;
                            if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                                value = Convert.ToInt32(item.Value);
                            p.SetValue(values, value, null);
                        }

                        CreateStuFrom(values, action);
                    }));
            else
                Controls.Add(AddInput("身份", "role", new[] { "未选", "队长", "队员" }[values.role], true, "请先退出队伍再进行调整"));

            Controls.Add(AddcomboBox("年级", "grade", _grade, values.grade.ToString(), isitems: true));
            Controls.Add(AddcomboBox("专业", "major_id", _speciality, _speciality[values.major_id]));
            Controls.Add(AddInput("密码", "pwd", "", tip: "请直接填写新的密码"));
            if (!values.g_id.Equals(0))
            {
                Controls.Add(AddInput("所在队名", "g_name",
                    new Read<group_info>().GetOnlyContent("g_id=" + values.g_id, "g_name").ToString()));
                switch (values.role)
                {
                    case 2:
                        Controls.Add(AddBtn("退出所在队伍", (_, __) =>
                        {
                            if (!new Update().CleanOnlyUserGid(values.s_id.ToString()))
                            {
                                Message.ShowError("退出失败", this);
                            }
                            else
                            {
                                Tool.Close(this);
                                Message.ShowSuccess("退出成功", this);
                                _paddingTop = 50;
                                foreach (KeyValuePair<string, string> item in _domains)
                                {
                                    object value = item.Value;

                                    PropertyInfo p = typeof(stu_info).GetProperty(item.Key);

                                    if (p == null) continue;
                                    if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                                        value = Convert.ToInt32(item.Value);
                                    p.SetValue(values, value, null);
                                }

                                values.g_id = 0;
                                CreateStuFrom(values, action);
                            }
                        }));
                        break;
                    case 1:
                        Controls.Add(AddBtn("修改队伍人员", (_, __) =>
                        {
                            stu_info.Instance.s_id = values.s_id;
                            stu_info.Instance.g_id = values.g_id;
                            new CreateGroup(memberList => { }).Show();
                        }));
                        break;
                }

                Controls.Add(AddBtn("修改所选导师", (_, __) =>
                {
                    stu_info.Instance.s_id = values.s_id;
                    stu_info.Instance.g_id = values.g_id;
                    new CreateGroup().Show();
                }));
                Controls.Add(AddBtn("解散所在队伍", (_, __) =>
                {
                    if (!new Update().CleanUserGid(values.g_id.ToString()))
                    {
                        Message.ShowError("解散失败", this);
                    }
                    else
                    {
                        Tool.Close(this);
                        Message.ShowSuccess("解散成功", this);
                        _paddingTop = 50;
                        foreach (KeyValuePair<string, string> item in _domains)
                        {
                            object value = item.Value;

                            PropertyInfo p = typeof(stu_info).GetProperty(item.Key);

                            if (p == null) continue;
                            if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                                value = Convert.ToInt32(item.Value);
                            p.SetValue(values, value, null);
                        }

                        values.g_id = 0;
                        CreateStuFrom(values, action);
                    }
                }, true));
            }
            else
            {
                if (values.role.Equals(1))
                    Controls.Add(AddBtn("新建队伍", (_, __) =>
                    {
                        stu_info.Instance.s_id = values.s_id;
                        stu_info.Instance.major_id = values.major_id;
                        stu_info.Instance.grade = values.grade;
                        new CreateGroup(() =>
                        {
                            Tool.Close(this);
                            _paddingTop = 50;
                            stu_info stuInfo = new Read<stu_info>().GetEntityValue("s_id=" + values.s_id);
                            CreateStuFrom(stuInfo, action);
                        }).Show();
                    }));
            }

            Controls.Add(AddBtn("确认修改", (_, __) =>
            {
                Update up = new Update();
                Read<stu_info> read = new Read<stu_info>();
                stu_info.Instance.s_id = values.s_id;
                stu_info.Instance.g_id = values.g_id;

                foreach (KeyValuePair<string, string> item in _domains)
                {
                    if (item.Key == "g_name")
                    {
                        up.UpdateGroupdata(item.Value, "g_name");
                        continue;
                    }

                    object value = item.Value;

                    if (item.Key == "pwd")
                    {
                        if (item.Value.Trim().Equals(""))
                        {
                            Message.ShowWarn("密码不得为空", this);
                            return;
                        }

                        value = read.GetMd5(item.Value, values.s_no);
                    }

                    PropertyInfo p = typeof(stu_info).GetProperty(item.Key);

                    if (p != null)
                    {
                        if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                            value = Convert.ToInt32(item.Value);
                        p.SetValue(values, value, null);
                    }

                    up.UpdateStuInfo(value.ToString(), item.Key);
                }

                Message.ShowSuccess("修改成功");
                action(values);
                Close();
            }));

            Controls.Add(AddBtn("删除该用户", (_, __) =>
            {
                Update up = new Update();

                up.CloseStuUser(values.s_id.ToString());

                Message.ShowSuccess("删除成功");
                action(default);
                Close();
            }, true));

            Height = _paddingTop + 90;
            Location = new Point((SystemInformation.PrimaryMonitorSize.Width - Width) / 2,
                (SystemInformation.PrimaryMonitorSize.Height - Height) / 2);
        }


        public void CreateTeaFrom(tea_info values, Action<tea_info> action)
        {
            Controls.Add(AddInput("姓名", "t_name", values.t_name));
            Controls.Add(AddInput("工号", "", values.t_id.ToString(), true, "自动生成，不予修改"));
            Controls.Add(AddcomboBox("性别", "sex", new[] { "男", "女" }, values.sex, isitems: true));

            Controls.Add(AddcomboBox("年级", "grade", _grade, values.grade.ToString(), isitems: true));
            Controls.Add(AddcomboBox("专业", "major_id", _speciality, _speciality[values.major_id]));

            Controls.Add(AddInput("密码", "pwd", "", tip: "请直接填写新的密码"));

            Controls.Add(AddRichInput("研究方向", "direction", values.direction));

            Controls.Add(AddBtn("修改所选队伍", (_, __) =>
            {
                tea_info.Instance.t_id = values.t_id;
                new ChooseGroup(memberList => { }).Show();
            }));

            Controls.Add(AddBtn("确认修改", (_, __) =>
            {
                Update up = new Update();
                Read<tea_info> read = new Read<tea_info>();
                tea_info.Instance.t_id = values.t_id;

                foreach (KeyValuePair<string, string> item in _domains)
                {
                    object value = item.Value;

                    if (item.Key == "pwd")
                    {
                        if (item.Value.Trim().Equals(""))
                        {
                            Message.ShowWarn("密码不得为空", this);
                            return;
                        }

                        value = read.GetMd5(item.Value, values.t_no);
                    }

                    PropertyInfo p = typeof(tea_info).GetProperty(item.Key);

                    if (p != null)
                    {
                        if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                            value = Convert.ToInt32(item.Value);
                        p.SetValue(values, value, null);
                    }

                    up.UpdateTeaInfo(value.ToString(), item.Key);
                }

                Message.ShowSuccess("修改成功");
                action(values);
                Close();
            }));

            Controls.Add(AddBtn("删除该用户", (_, __) =>
            {
                Update up = new Update();

                up.CloseTeaUser(values.t_id.ToString());

                Message.ShowSuccess("删除成功");
                action(default);
                Close();
            }, true));
            Height = _paddingTop + 90;
        }

        public void CreateGroupFrom(group_info values, Action<group_info> action)
        {
            Controls.Add(AddInput("组名", "g_name", values.g_name));
            Controls.Add(AddInput("组号", "", values.g_id.ToString(), true, "自动生成，不予修改"));
            Controls.Add(AddInput("组长名", "",
                new Read<stu_info>().GetOnlyContent("role = 1 and g_id=" + values.g_id, "s_name").ToString(), true,
                "唯一绑定，不予修改"));

            Controls.Add(AddcomboBox("年级", "grade", _grade, values.grade.ToString(), isitems: true));
            Controls.Add(AddcomboBox("专业", "major_id", _speciality, _speciality[values.major_id]));

            Controls.Add(AddInput("项目名称", "project_name", values.project_name));
            Controls.Add(AddRichInput("项目说明", "project_info", values.project_info));

            Controls.Add(AddBtn("修改所选导师", (_, __) =>
            {
                stu_info.Instance.g_id = values.g_id;
                new CreateGroup().Show();
            }));

            Controls.Add(AddBtn("修改队伍人员", (_, __) =>
            {
                stu_info.Instance.s_id =
                    Convert.ToInt32(new Read<stu_info>().GetOnlyContent("role = 1 and g_id=" + values.g_id, "s_id"));
                stu_info.Instance.g_id = values.g_id;
                new CreateGroup(memberList => { }).Show();
            }));

            Controls.Add(AddBtn("确认修改", (_, __) =>
            {
                Update up = new Update();
                stu_info.Instance.g_id = values.g_id;

                foreach (KeyValuePair<string, string> item in _domains)
                {
                    object value = item.Value;

                    PropertyInfo p = typeof(group_info).GetProperty(item.Key);

                    if (p != null)
                    {
                        if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                            value = Convert.ToInt32(item.Value);

                        p.SetValue(values, value, null);
                    }

                    up.UpdateGroupdata(value.ToString(), item.Key);
                }

                Message.ShowSuccess("修改成功");
                action(values);
                Close();
            }));

            Controls.Add(AddBtn("解散该队伍", (_, __) =>
            {
                if (!new Update().CleanUserGid(values.g_id.ToString()))
                {
                    Message.ShowError("解散失败", this);
                }
                else
                {
                    Message.ShowSuccess("解散成功");
                    action(default);
                    Close();
                }
            }, true));

            Height = _paddingTop + 90;
        }


        public void CreateNewStuFrom(stu_info stuinfo, int index, bool isNewAdd = true,
            Action<stu_info> action = default)
        {
            stu_info.Instance.s_no = index;

            Controls.Add(AddInput("姓名", "s_name", stuinfo.s_name));
            Controls.Add(AddcomboBox("性别", "sex", new[] { "男", "女" }, stuinfo.sex, isitems: true));
            Controls.Add(
                AddcomboBox("身份", "role", new[] { "未选", "队长", "队员" }, new[] { "未选", "队长", "队员" }[stuinfo.role]));
            _domains["role"] = stuinfo.role.ToString();
            Controls.Add(AddcomboBox("年级", "grade", _grade, stuinfo.grade.ToString(), isitems: true));
            Controls.Add(AddcomboBox("专业", "major_id", _speciality, _speciality[stuinfo.major_id]));
            _domains["major_id"] = stuinfo.major_id.ToString();
            Controls.Add(AddInput("密码", "pwd", stuinfo.pwd));
            _domains["pwd"] = stuinfo.pwd;
            if (isNewAdd)
                Controls.Add(AddBtn("确认新增", (_, __) =>
                {
                    if (_domains.Count < 6)
                    {
                        Message.ShowWarn("请填写完整信息", this);
                        return;
                    }

                    Update up = new Update();
                    Read<stu_info> read = new Read<stu_info>();
                    stu_info result = new stu_info();

                    foreach (KeyValuePair<string, string> item in _domains)
                    {
                        object values = item.Value;

                        if (item.Key == "pwd")
                        {
                            if (item.Value.Trim().Equals(""))
                            {
                                Message.ShowWarn("密码不得为空", this);
                                return;
                            }

                            values = read.GetMd5(item.Value);
                        }

                        PropertyInfo p = typeof(stu_info).GetProperty(item.Key);

                        if (p == null) continue;
                        if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                            values = Convert.ToInt32(item.Value);
                        p.SetValue(result, values, null);
                    }

                    up.CreateStuPerson(result);
                    Message.ShowSuccess("修改成功");
                    Close();
                }));
            else
                Controls.Add(AddBtn("确认修改", (_, __) =>
                {
                    Read<stu_info> read = new Read<stu_info>();

                    foreach (KeyValuePair<string, string> item in _domains)
                    {
                        object value = item.Value;
                        if (item.Key == "pwd")
                        {
                            if (item.Value.Trim().Equals(""))
                            {
                                Message.ShowWarn("密码不得为空", this);
                                return;
                            }

                            value = read.GetMd5(item.Value);
                        }

                        PropertyInfo p = typeof(stu_info).GetProperty(item.Key);
                        if (p == null) continue;
                        if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                            value = Convert.ToInt32(item.Value);
                        p.SetValue(stuinfo, value, null);
                    }

                    Message.ShowSuccess("修改成功");
                    action?.Invoke(stuinfo);
                    Close();
                }));

            Height = _paddingTop + 90;
        }

        public void CreateNewTeaFrom(tea_info teainfo, int index, bool isNewAdd = true,
            Action<tea_info> action = default)
        {
            tea_info.Instance.t_no = index;

            Controls.Add(AddInput("姓名", "t_name", teainfo.t_name));

            Controls.Add(AddcomboBox("性别", "sex", new[] { "男", "女" }, teainfo.sex, isitems: true));
            Controls.Add(AddcomboBox("年级", "grade", _grade, teainfo.grade.ToString(), isitems: true));
            Controls.Add(AddcomboBox("专业", "major_id", _speciality, _speciality[teainfo.major_id]));
            _domains["major_id"] = teainfo.major_id.ToString();
            Controls.Add(AddInput("密码", "pwd", teainfo.pwd));
            _domains["pwd"] = teainfo.pwd;
            Controls.Add(AddRichInput("研究方向", "direction", teainfo.direction));
            _domains["direction"] = teainfo.direction;
            if (isNewAdd)
                Controls.Add(AddBtn("确认新增", (_, __) =>
                {
                    if (_domains.Count < 6)
                    {
                        Message.ShowWarn("请填写完整信息", this);
                        return;
                    }

                    Update up = new Update();
                    Read<tea_info> read = new Read<tea_info>();
                    tea_info result = new tea_info();

                    foreach (KeyValuePair<string, string> item in _domains)
                    {
                        object values = item.Value;

                        if (item.Key == "pwd")
                        {
                            if (item.Value.Trim().Equals(""))
                            {
                                Message.ShowWarn("密码不得为空", this);
                                return;
                            }

                            values = read.GetMd5(item.Value);
                        }

                        PropertyInfo p = typeof(tea_info).GetProperty(item.Key);

                        if (p == null) continue;
                        if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                            values = Convert.ToInt32(item.Value);
                        p.SetValue(result, values, null);
                    }

                    up.CreateTeaPerson(result);
                    Message.ShowSuccess("修改成功");
                    Close();
                }));
            else
                Controls.Add(AddBtn("确认修改", (_, __) =>
                {
                    Read<tea_info> read = new Read<tea_info>();

                    foreach (KeyValuePair<string, string> item in _domains)
                    {
                        object value = item.Value;

                        if (item.Key == "pwd")
                        {
                            if (item.Value.Trim().Equals(""))
                            {
                                Message.ShowWarn("密码不得为空", this);
                                return;
                            }

                            value = read.GetMd5(item.Value, teainfo.t_no);
                        }

                        PropertyInfo p = typeof(tea_info).GetProperty(item.Key);

                        if (p == null) continue;
                        if (p.GetMethod.ReturnParameter?.ParameterType.Name == "Int32")
                            value = Convert.ToInt32(item.Value);
                        p.SetValue(teainfo, value, null);
                    }

                    Message.ShowSuccess("修改成功");
                    action?.Invoke(teainfo);
                    Close();
                }));
            Height = _paddingTop + 90;
        }

        private Panel AddInput(string title, string readkey, string value = "", bool disable = false, string tip = "")
        {
            Panel p = new Panel
            {
                Size = new Size(Width - 120, 32),
                Location = new Point(55, _paddingTop)
            };

            _paddingTop += 40;

            Label _title = new Label
            {
                Text = $@"{title} :",
                Location = new Point(0, (p.Height - 18) / 2),
                Font = new Font(CustomFont.Font.RPfc.Families[0], 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134),
                AutoSize = true
            };

            Panel _value = new Panel
            {
                Location = new Point(p.Width - 150, 0),
                Size = new Size(150, p.Height - 2),
                BackColor = Color.White
            };

            TextBox t = new TextBox
            {
                Location = new Point(6, 6),
                Size = new Size(_value.Width - 12, _value.Height - 12),
                Text = value,
                BackColor = Color.White,
                Enabled = !disable,
                BorderStyle = BorderStyle.None,
                Font = new Font(CustomFont.Font.RPfc.Families[0], 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };

            _value.Controls.Add(t);

            Label divider = new Label
            {
                Location = new Point(p.Width - 150, _value.Height),
                Size = new Size(_value.Width, 1),
                BackColor = Color.Silver
            };

            if (tip != "")
            {
                p.Height += 17;
                _paddingTop += 17;
                Label _tip = new Label
                {
                    AutoSize = true,
                    BackColor = Color.White,
                    Font = new Font(CustomFont.Font.RPfc.Families[0], 9F, FontStyle.Regular, GraphicsUnit.Point, 134),
                    ForeColor = disable ? Color.Red : Color.Black,
                    Location = new Point(divider.Location.X, divider.Bottom + 1),
                    Text = $@"( {tip}"
                };

                p.Controls.Add(_tip);
            }

            if (!disable)
            {
                _value.MouseEnter += (_, __) => _value.BackColor = t.BackColor = Color.FromArgb(240, 240, 240);
                t.MouseEnter += (_, __) => _value.BackColor = t.BackColor = Color.FromArgb(240, 240, 240);
                _value.MouseLeave += (_, __) =>
                {
                    if (t.Focused)
                        return;
                    _value.BackColor = t.BackColor = Color.White;
                };
                t.MouseLeave += (_, __) =>
                {
                    if (t.Focused)
                        return;
                    _value.BackColor = t.BackColor = Color.White;
                };
                t.GotFocus += (_, __) => divider.BackColor = Color.Black;
                t.LostFocus += (_, __) =>
                {
                    divider.BackColor = Color.Silver;
                    _value.BackColor = t.BackColor = Color.White;
                };

                t.TextChanged += (_, __) => { _domains[readkey] = t.Text; };
            }


            p.Controls.Add(_title);
            p.Controls.Add(_value);
            p.Controls.Add(divider);

            return p;
        }

        private Panel AddcomboBox(string title, string readkey, string[] items, string value = null, string tip = "",
            bool isitems = false, Action callback = default)
        {
            Panel p = new Panel
            {
                Size = new Size(Width - 120, 32),
                Location = new Point(55, _paddingTop)
            };

            _paddingTop += 40;

            Label _title = new Label
            {
                Text = $@"{title} :",
                Location = new Point(0, (p.Height - 18) / 2),
                Font = new Font(CustomFont.Font.RPfc.Families[0], 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134),
                AutoSize = true
            };

            ComboBox com = new ComboBox
            {
                BackColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(CustomFont.Font.RPfc.Families[0], 10.5F),
                Location = new Point(p.Width - 150, 0),
                Size = new Size(150, p.Height - 2)
            };

            com.Items.AddRange(items);
            com.SelectedItem = value;

            Label divider = new Label
            {
                Location = new Point(p.Width - 150, com.Height),
                Size = new Size(com.Width, 1),
                BackColor = Color.Silver
            };

            if (tip != "")
            {
                p.Height += 17;
                _paddingTop += 17;
                Label _tip = new Label
                {
                    AutoSize = true,
                    BackColor = Color.White,
                    Font = new Font(CustomFont.Font.RPfc.Families[0], 9F, FontStyle.Regular, GraphicsUnit.Point, 134),
                    ForeColor = Color.Black,
                    Location = new Point(divider.Location.X, divider.Bottom + 1),
                    Text = $@"( {tip}"
                };

                p.Controls.Add(_tip);
            }

            com.GotFocus += (_, __) => divider.BackColor = Color.Black;
            com.LostFocus += (_, __) => divider.BackColor = Color.Silver;

            com.SelectedIndexChanged += (_, __) =>
            {
                if (isitems)
                    _domains[readkey] = com.SelectedItem.ToString();
                else
                    _domains[readkey] = com.SelectedIndex.ToString();

                if (callback != null && com.Focused)
                    callback();
            };


            p.Controls.Add(_title);
            p.Controls.Add(com);
            p.Controls.Add(divider);

            return p;
        }

        private Panel AddBtn(string title, Action<object, EventArgs> func = default, bool warn = false)
        {
            _paddingTop += 5;
            Panel p = new Panel
            {
                Size = new Size(Width - 120, 35),
                Location = new Point(55, _paddingTop)
            };

            _paddingTop += 48;

            Button btn = new Button
            {
                BackColor = warn ? Color.Red : Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font(CustomFont.Font.BPfc.Families[0], 12.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 0),
                Size = new Size(p.Width, 35),
                Text = title
            };

            btn.FlatAppearance.BorderSize = 0;
            if (func != null)
                btn.Click += new EventHandler(func);

            p.Controls.Add(btn);

            return p;
        }

        private Panel AddRichInput(string title, string readkey, string value = "", int richHeight = 100,
            bool disable = false, string tip = "")
        {
            Panel p = new Panel
            {
                Size = new Size(Width - 120, richHeight),
                Location = new Point(55, _paddingTop)
            };

            _paddingTop += richHeight + 8;

            Label _title = new Label
            {
                Text = $@"{title} :",
                Location = new Point(0, p.Height - 25),
                Font = new Font(CustomFont.Font.RPfc.Families[0], 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134),
                AutoSize = true
            };

            Panel _value = new Panel
            {
                Location = new Point(p.Width - 150, 0),
                Size = new Size(150, p.Height - 2),
                BackColor = Color.White
            };

            TextBox t = new TextBox
            {
                Location = new Point(6, 6),
                Size = new Size(_value.Width - 12, _value.Height - 12),
                Text = value,
                Enabled = !disable,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                MaxLength = 2147483647,
                Multiline = true,
                Font = new Font(CustomFont.Font.RPfc.Families[0], 10.5F, FontStyle.Regular, GraphicsUnit.Point, 134)
            };

            int multilineTextHeight = (t.GetLineFromCharIndex(t.Text.Length) + 1) * t.Font.Height;
            t.ScrollBars = t.Height < multilineTextHeight ? ScrollBars.Vertical : ScrollBars.None;

            t.TextChanged += (_, __) =>
            {
                multilineTextHeight = (t.GetLineFromCharIndex(t.Text.Length) + 1) * t.Font.Height;
                t.ScrollBars = t.Height < multilineTextHeight ? ScrollBars.Vertical : ScrollBars.None;
                _domains[readkey] = t.Text;
            };

            _value.Controls.Add(t);

            Label divider = new Label
            {
                Location = new Point(p.Width - 150, _value.Height),
                Size = new Size(_value.Width, 1),
                BackColor = Color.Silver
            };


            if (tip != "")
            {
                p.Height += 17;
                _paddingTop += 17;
                Label _tip = new Label
                {
                    AutoSize = true,
                    BackColor = Color.White,
                    Font = new Font(CustomFont.Font.RPfc.Families[0], 9F, FontStyle.Regular, GraphicsUnit.Point, 134),
                    ForeColor = disable ? Color.Red : Color.Black,
                    Location = new Point(divider.Location.X, divider.Bottom + 1),
                    Text = $@"( {tip}"
                };

                p.Controls.Add(_tip);
            }

            if (!disable)
            {
                _value.MouseEnter += (_, __) => _value.BackColor = t.BackColor = Color.FromArgb(240, 240, 240);
                t.MouseEnter += (_, __) => _value.BackColor = t.BackColor = Color.FromArgb(240, 240, 240);
                _value.MouseLeave += (_, __) =>
                {
                    if (t.Focused)
                        return;
                    _value.BackColor = t.BackColor = Color.White;
                };
                t.MouseLeave += (_, __) =>
                {
                    if (t.Focused)
                        return;
                    _value.BackColor = t.BackColor = Color.White;
                };
                t.GotFocus += (_, __) => divider.BackColor = Color.Black;
                t.LostFocus += (_, __) =>
                {
                    divider.BackColor = Color.Silver;
                    _value.BackColor = t.BackColor = Color.White;
                };
            }

            p.Controls.Add(_title);
            p.Controls.Add(_value);
            p.Controls.Add(divider);

            return p;
        }
    }
}