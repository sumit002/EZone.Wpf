using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ElectronicZone.Wpf.Utility
{
    public class CommonMethods
    {
        #region properties
        static readonly int topRow = 5;
        #endregion properties

        /// <summary>
        /// Sort Table By Column
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sortByColumn"></param>
        /// <param name="isDesc"></param>
        /// <returns></returns>
        public static DataTable SortTable(DataTable dt, string sortByColumn, bool? isDesc = false)
        {
            DataView dv = dt.DefaultView;
            dv.Sort = string.Format("{0} {1}", sortByColumn, isDesc == true ? "DESC" : "ASC");
            dt = dv.ToTable();
            return dt;
        }
        /// <summary>
        /// Select Top 5 rows from DataTable By Linq
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static DataTable GetTopRow(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
                return dataTable;
            return dataTable.Rows.Cast<System.Data.DataRow>().Take(topRow).CopyToDataTable();
        }

        public static decimal GetSum(DataTable dataTable, string columnName)
        {
            if (dataTable.Rows.Count == 0)
                return 0;
            var d = dataTable.Compute("Sum(" + columnName + ")", "");
            //return (decimal) d.ToString("C2");
            return decimal.Parse(d.ToString());
        }

        public static List<T> ConvertDataTable<T>(DataTable dt) {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        /// <summary>
        /// Setting Text To Clipboard
        /// </summary>
        /// <param name="text"></param>
        public static void SetTextToClipboard(string text) {
            System.Windows.Clipboard.SetText($"{text}");
        }

        /// <summary>
        /// Generate Sale/Purchase invoice Number Format
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_dateTime"></param>
        /// <returns></returns>
        public static string GenerateInvoice(int _id, DateTime _dateTime) {
            string invoice = $"{_dateTime.ToString(ConfigurationManager.AppSettings["InvoiceDatePattern"])}-{_id.ToString(ConfigurationManager.AppSettings["InvoiceIdPattern"])}";
            return invoice;
        }
    }
}
