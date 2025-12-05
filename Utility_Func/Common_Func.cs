namespace Infusive_back.Utility_Func
{
    public static class Common_Func
    {
        private readonly static string Env = "STAGE"; // STAGE, LIVE
        public static string ConnectionString()
        {
            if (Env == "STAGE")
                return "Server=103.38.50.46;Port=3306;Database=decrypt_talent_stg;User=jungle_avengers;Password=Tek123!@#;";
            else if (Env == "LIVE")
                return "Server=103.38.50.46;Port=3306;Database=infusive_live;User=jungle_avengers;Password=Tek123!@#;";
            else
                return "Server=127.0.0.1;Port=3306;Database=infusive;User=root;Password=123456;";
        }
    }
}
