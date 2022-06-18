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

        private volatile static stu_info instance;
        private static readonly object padlock = new object();

        public static stu_info Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new stu_info();
                        }
                    }
                }
                return instance;
            }
        }
    }
}