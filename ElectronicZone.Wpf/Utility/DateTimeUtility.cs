namespace ElectronicZone.Wpf.Utility
{
    using System;
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// Basically this Utility handles the date time object to convert in 
    /// different formats
    /// </summary>
    public class DateTimeUtility
    {
        public string getUTCFormattedDate(string date) {
            DateTime dt = DateTime.ParseExact(date, ConfigurationManager.AppSettings["DateTimeFormat"], CultureInfo.InvariantCulture);
            return dt.ToString();
        }

        public DateTime GetMonthStartDate() {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            return firstDayOfMonth;
        }
    }
}
