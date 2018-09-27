using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicZone.Wpf.DataAccessLayer
{
    public abstract class SqlDataAccess
    {
        protected static readonly string m_connectionString;
        static SqlDataAccess()
        {
            m_connectionString = GetConnString();
        }

        private static string GetConnString()
        {
            /*
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            path = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            DirectoryInfo d = new DirectoryInfo(path);
            path = d.Parent.Parent.FullName;
            //path = path + "\\App_Data";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            string connString = @"Data Source=|DataDirectory|\database.db;Version=3;New=False;Compress=True;";*/

            //string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //Directory.SetCurrentDirectory(exeDir);

            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            string connString = @"Data Source=|DataDirectory|\JackDB.db;Version=3;New=False;Compress=True;";// @"Data Source=database.db; Version=3;";
            return connString;
        }
    }
}
