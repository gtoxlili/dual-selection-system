using System.Drawing;
using System.Windows.Forms;
using UI.Properties;

namespace UI
{
    public class Message
    {
        private static int index = -1;
        private readonly Control _con;
        private readonly int _index;
        private readonly int _messageWidth = 380;
        private readonly int _paddingLeft;
        private readonly int _sleepTime;
        private Label _alert;
        private Panel _border;
        private Panel _main;
        private Timer _t;

        public Message(Control con, int sleepTime, int minWidth)
        {
            _paddingLeft = (con.Width - 8 - 380) / 2;
            _index = ++index;

            if (_paddingLeft <= minWidth)
            {
                _paddingLeft = minWidth;
                _messageWidth = con.Width - 8 - minWidth * 2;
            }

            _sleepTime = sleepTime;
            _con = con;

            MessageLoad();
            CreateTimer();
        }

        private void MessageLoad()
        {
            _border = new Panel
            {
                Location = new Point(_paddingLeft, -48 + 58 * _index),
                Size = new Size(_messageWidth, 48)
            };

            _main = new Panel
            {
                Location = new Point(1, 1),
                Size = new Size(_border.Width - 2, 46)
            };

            _alert = new Label
            {
                Size = new Size(_main.Width - 30, 16),
                Location = new Point(15, 15),
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                Font = new Font(CustomFont.Font.rPFC.Families[0], 10.5F),

            };

            _main.Controls.Add(_alert);
            _border.Controls.Add(_main);

            _con.Controls.Add(_border);
            _con.Controls.SetChildIndex(_border, 0);
        }

        private void CreateTimer()
        {
            bool tstatus = true;
            int count = 0;

            _t = new Timer
            {
                Enabled = false,
                Interval = 10
            };


            _t.Tick += (sender, e) =>
            {
                if (tstatus)
                {
                    if (_border.Top <= 10 + 58 * _index)
                    {
                        _border.Top += 4;
                    }
                    else
                    {
                        //Thread.Sleep(_sleepTime);

                        count++;
                        if (count >= _sleepTime / _t.Interval)
                            tstatus = false;
                    }
                }
                else
                {
                    if (_border.Top >= -48 + 58 * _index)
                    {
                        _border.Top -= 6;
                    }
                    else
                    {
                        index--;
                        _con.Controls.Remove(_border);
                        _border.Dispose();
                        _t.Dispose();
                    }
                }
            };
        }

        public static void ShowSuccess(string value, Control con = null, int sleepTime = 1000, int minWidth = 60)
        {
            if (con == null)
                con = Application.OpenForms[0];
            if (58 * (index + 3) >= con.Height)
                return;

            Message message = new Message(con, sleepTime, minWidth)
            {
                _main =
                {
                    BackColor = Color.FromArgb(240, 249, 235)
                },
                _border =
                {
                    BackColor = Color.FromArgb(225, 243, 216)
                },
                _alert =
                {
                    ForeColor = Color.FromArgb(103, 194, 58),
                    Image = UIResource.成功,
                    Text = $@"      {value}"
                }
            };

            message._t.Start();
        }

        public static void ShowWarn(string value, Control con = null, int sleepTime = 1000, int minWidth = 60)
        {
            if (con == null)
                con = Application.OpenForms[0];
            if (58 * (index + 3) >= con.Height)
                return;

            Message message = new Message(con, sleepTime, minWidth)
            {
                _main =
                {
                    BackColor = Color.FromArgb(253, 246, 236)
                },
                _border =
                {
                    BackColor = Color.FromArgb(250, 236, 216)
                },
                _alert =
                {
                    ForeColor = Color.FromArgb(230, 162, 60),
                    Image = UIResource.警告,
                    Text = $@"      {value}"
                }
            };

            message._t.Start();
        }

        public static void ShowInfo(string value, Control con = null, int sleepTime = 1000, int minWidth = 60)
        {
            if (con == null)
                con = Application.OpenForms[0];
            if (58 * (index + 3) >= con.Height)
                return;

            Message message = new Message(con, sleepTime, minWidth)
            {
                _main =
                {
                    BackColor = Color.FromArgb(237, 242, 252)
                },
                _border =
                {
                    BackColor = Color.FromArgb(235, 238, 245)
                },
                _alert =
                {
                    ForeColor = Color.FromArgb(144, 147, 153),
                    Image = UIResource.消息,
                    Text = $@"      {value}"
                }
            };

            message._t.Start();
        }

        public static void ShowError(string value, Control con = null, int sleepTime = 1000, int minWidth = 60)
        {
            if (con == null)
                con = Application.OpenForms[0];
            if (58 * (index + 3) >= con.Height)
                return;

            Message message = new Message(con, sleepTime, minWidth)
            {
                _main =
                {
                    BackColor = Color.FromArgb(254, 240, 240)
                },
                _border =
                {
                    BackColor = Color.FromArgb(253, 226, 226)
                },
                _alert =
                {
                    ForeColor = Color.FromArgb(245, 108, 108),
                    Image = UIResource.错误,
                    Text = $@"      {value}"
                }
            };

            message._t.Start();
        }

        public override string ToString()
        {
            return @"MADE BY JUNIAN";
        }
    }
}