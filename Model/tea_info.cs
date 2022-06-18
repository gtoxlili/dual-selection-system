namespace Model
{
    public class tea_info
    {
        public int major_id { get; set; }
        
        public string sex { get; set; }
        
        public int grade { get; set; }
        
        public string t_name { get; set; }
        
        public string pwd { get; set; }
        
        public int t_id { get; set; }
        
        public int t_no { get; set; }

        public string direction { get; set; }

        // 单例
        private volatile static tea_info instance;
        private static readonly object padlock = new object();

        public static tea_info Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new tea_info();
                        }
                    }
                }
                return instance;
            }
        }
    }

}