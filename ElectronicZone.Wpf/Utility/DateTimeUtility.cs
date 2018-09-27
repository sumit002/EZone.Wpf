namespace ElectronicZone.Wpf.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Globalization;

    /// <summary>
    /// Basically this Utility handles the date time object to convert in 
    /// different formats
    /// </summary>
    public class DateTimeUtility
    {
        public string getUTCFormattedDate(string date) {
            DateTime dt = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            return dt.ToString();
        }

        public DateTime getMonthStartDate() {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            return firstDayOfMonth;
        }
    }
}
