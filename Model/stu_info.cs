using System;

namespace Model
{
    public class stu_info
    {
        public int major_id { get; set; }
        
        public string sex { get; set; }

        public int role { get; set; }

        public int grade { get; set; }

        public int g_id { get; set; }

        public string s_name { get; set; }

        public string pwd { get; set; }

        public int s_id { get; set; }

        public int s_no { get; set; }

        private static volatile stu_info _instance;
        private static readonly object Padlock = new object();

        public static stu_info Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new stu_info();
                    }
                }
                return _instance;
            }
        }
    }
}