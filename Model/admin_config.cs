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
        private static volatile admin_config _instance;
        private static readonly object Padlock = new object();

        public static admin_config Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new admin_config();
                    }
                }
                return _instance;
            }
        }

    }
}