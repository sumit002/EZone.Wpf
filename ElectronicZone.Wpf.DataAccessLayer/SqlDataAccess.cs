using System;

namespace ElectronicZone.Wpf.DataAccessLayer
{
    public abstract class SqlDataAccess
    {
        protected static readonly string m_connectionString;

        /// <summary>
        /// SQL DataAccess
        /// </summary>
        static SqlDataAccess()
        {
            m_connectionString = GetConnString();
        }

        /// <summary>
        /// Get Connection String for SQLite DB
        /// </summary>
        /// <returns></returns>
        private static string GetConnString()
        {
            /*
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            DirectoryInfo d = new DirectoryInfo(path);
            path = d.Parent.Parent.FullName;
            //path = path + "\\App_Data";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            string connString = @"Data Source=|DataDirectory|\database.db;Version=3;New=False;Compress=True;";
            */

            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string _path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", _path);
            // MessageBox.Show($"Connection String: {path}");
            return @"Data Source=|DataDirectory|\JackDB.db;Version=3;New=False;Compress=True;";
        }
    }
}
