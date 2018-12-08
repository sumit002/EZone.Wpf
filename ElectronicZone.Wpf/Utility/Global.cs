namespace ElectronicZone.Wpf.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class Global
    {
        #region Fields & Properties
        private static int _UserId;
        private static string _UserName;
        private static string _Name;
        private static bool _isAdmin;

        public static int UserId { get => _UserId; set => _UserId = value; }
        public static string Name { get => _Name; set => _Name = value; }
        public static string UserName { get => _UserName; set => _UserName = value; }
        public static bool IsAdmin { get => _isAdmin; set => _isAdmin = value; } 
        #endregion

        public static bool VerifyUser() {
            return (UserId > 0) ? true : false;
        }
    }
}
