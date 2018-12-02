using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicZone.Wpf.Utility
{
    public static class Global
    {
        // private static readonly Global instance = new Global();

        private static int _UserId;
        //private static string _UserName;
        private static string _Name;
        //private static string _Password;

        public static int UserId { get => _UserId; set => _UserId = value; }
        public static string Name { get => _Name; set => _Name = value; }

        public static bool VerifyUser() {
            return (UserId > 0) ? true : false;
        }

        //public Global(string userName, int id)
        //{
        //    _UserName = userName;
        //    _UserId = id;
        //}
    }
}
