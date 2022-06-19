using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using static System.Configuration.ConfigurationManager;

namespace JView
{
    public partial class AdminPrint : Form
    {
        private readonly Font _f = new Font("楷体", 10f); //字体
        private readonly Font _titleFont = new Font("微软雅黑", 20f, FontStyle.Bold);
        private int _x;
        private int _y;

        public AdminPrint(List<stu_info> stuArr)
        {
            InitializeComponent();
            DefaultSetting();
            printDocument1.PrintPage += (_, e) =>
            {
                _x = e.MarginBounds.Left;
                _y = e.MarginBounds.Top;
                string[] itemName = { "学号", "姓名", "性别", "年级", "专业" };

                int titleTop = (_y - _titleFont.Height) / 2 + 12;

                e.Graphics.DrawString("学生信息", _titleFont, Brushes.Black,
                    e.PageBounds.Width / 2 - 70, titleTop);

                PrintHead(e, itemName);


                foreach (stu_info stu in stuArr)
                {
                    _y += _f.Height + 2;
                    e.Graphics.DrawLine(Pens.Gainsboro, new Point(_x, _y), new Point(e.MarginBounds.Width + _x, _y));
                    _y += 2;
                    PrintBody(e, stu);
                }
            };
        }

        public AdminPrint(List<tea_info> teaArr)
        {
            InitializeComponent();
            DefaultSetting();
            printDocument1.PrintPage += (_, e) =>
            {
                _x = e.MarginBounds.Left;
                _y = e.MarginBounds.Top;
                string[] itemName = { "工号", "姓名", "性别", "年级", "专业" };

                int titleTop = (_y - _titleFont.Height) / 2 + 12;

                e.Graphics.DrawString("教师信息", _titleFont, Brushes.Black,
                    e.PageBounds.Width / 2 - 70, titleTop);

                PrintHead(e, itemName);


                foreach (tea_info tea in teaArr)
                {
                    _y += _f.Height + 2;
                    e.Graphics.DrawLine(Pens.Gainsboro, new Point(_x, _y), new Point(e.MarginBounds.Width + _x, _y));
                    _y += 2;
                    PrintBody(e, tea);
                }
            };
        }

        public AdminPrint(List<group_info> groupArr)
        {
            InitializeComponent();
            DefaultSetting();
            printDocument1.PrintPage += (_, e) =>
            {
                _x = e.MarginBounds.Left;
                _y = e.MarginBounds.Top;
                string[] itemName = { "组号", "组名", "年级", "专业" };

                int titleTop = (_y - _titleFont.Height) / 2 + 12;

                e.Graphics.DrawString("队伍信息", _titleFont, Brushes.Black,
                    e.PageBounds.Width / 2 - 70, titleTop);

                PrintHead(e, itemName);


                foreach (group_info gr in groupArr)
                {
                    _y += _f.Height + 2;
                    e.Graphics.DrawLine(Pens.Gainsboro, new Point(_x, _y), new Point(e.MarginBounds.Width + _x, _y));
                    _y += 2;
                    PrintBody(e, gr);
                }
            };
        }

        private void DefaultSetting()
        {
            // setting printDocument Paper to A5
            printDocument1.DefaultPageSettings.PaperSize = new PaperSize("A5", 595, 842);
            printDocument1.DefaultPageSettings.Landscape = true;
            printPreviewDialog1.ClientSize = new Size(900, 600);
        }

        private void PrintHead(PrintPageEventArgs e, string[] itemName)
        {
            int xgap = e.MarginBounds.Width / itemName.Length;
            StringFormat format = new StringFormat
            { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

            foreach (string t in itemName)
            {
                e.Graphics.DrawString(t, _f, Brushes.Gray, _x, _y, format);
                _x += xgap;
            }

            _x = e.MarginBounds.Left;
        }

        private void PrintBody(PrintPageEventArgs e, stu_info stu)
        {
            int xgap = e.MarginBounds.Width / 5;
            StringFormat format = new StringFormat
            { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

            e.Graphics.DrawString(stu.s_id.ToString(), _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(stu.s_name, _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(stu.sex, _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(stu.grade.ToString(), _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(AppSettings["Speciality"].Split(',')[stu.major_id + 1], _f, Brushes.Black, _x, _y,
                format);

            _x = e.MarginBounds.Left;
        }

        private void PrintBody(PrintPageEventArgs e, group_info stu)
        {
            int xgap = e.MarginBounds.Width / 4;
            StringFormat format = new StringFormat
            { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

            e.Graphics.DrawString(stu.g_id.ToString(), _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(stu.g_name, _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(stu.grade.ToString(), _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(AppSettings["Speciality"].Split(',')[stu.major_id + 1], _f, Brushes.Black, _x, _y,
                format);

            _x = e.MarginBounds.Left;
        }

        private void PrintBody(PrintPageEventArgs e, tea_info stu)
        {
            int xgap = e.MarginBounds.Width / 5;
            StringFormat format = new StringFormat
            { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

            e.Graphics.DrawString(stu.t_id.ToString(), _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(stu.t_name, _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(stu.sex, _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(stu.grade.ToString(), _f, Brushes.Black, _x, _y, format);
            _x += xgap;
            e.Graphics.DrawString(AppSettings["Speciality"].Split(',')[stu.major_id + 1], _f, Brushes.Black, _x, _y,
                format);

            _x = e.MarginBounds.Left;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.Document = printDocument1;

            pageSetupDialog1.PageSettings = printDocument1.DefaultPageSettings;
            pageSetupDialog1.PageSettings.Margins = PrinterUnitConvert.Convert(pageSetupDialog1.PageSettings.Margins,
                PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter);

            if (pageSetupDialog1.ShowDialog() == DialogResult.OK)
                printDocument1.DefaultPageSettings = pageSetupDialog1.PageSettings;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() != DialogResult.OK) printDocument1 = printDialog1.Document;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;

            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
                printDocument1.Print();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }
    }
}