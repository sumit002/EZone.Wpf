namespace ElectronicZone.Wpf.Utility
{
    using System;

    /// <summary>
    /// Basically this Utility handles the date time object to convert in 
    /// different formats
    /// </summary>
    public class DateTimeUtility
    {
        //public string GetUTCFormattedDate(string date) {
        //    DateTime dt = DateTime.ParseExact(date, ConfigurationManager.AppSettings["DateTimeFormat"], CultureInfo.InvariantCulture);
        //    return dt.ToString();
        //}

        /// <summary>
        /// Get Month Start Date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime GetMonthStartDate(DateTime? date) {
            date = date == null ? DateTime.Now : date;
            var firstDayOfMonth = new DateTime(date.Value.Year, date.Value.Month, 1);
            return firstDayOfMonth;
        }
    }
}
