namespace Model
{
    public class admin_config
    {
        public string admPwd { get; set; }
        
        public string admUser { get; set; }
        
        public int stuGroupNum { get; set; }
        
        public string stuCloseTime { get; set; }
        
        public int stuChooseNum { get; set; }
        
        public int teaHaveNum { get; set; }
        
        public string teaCloseTime { get; set; }
        
        public int teaChooseNum { get; set; }
        
        public string admID { get; set; }

        public string defaultPwd { get; set; }


        // 单例
        private volatile static admin_config instance;
        private static readonly object padlock = new object();

        public static admin_config Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new admin_config();
                        }
                    }
                }
                return instance;
            }
        }

    }
}