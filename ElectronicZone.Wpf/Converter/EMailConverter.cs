using System;
using System.Windows.Data;

namespace ElectronicZone.Wpf.Converter
{
    /// <summary>
    /// Converts the mailto uri to a string with just the customer alias
    /// </summary>
    public class EmailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.ToString() == "")
            {
                string email = "";
                return email;
            }
            else
            {
                string email = value.ToString();
                int index = email.IndexOf("@");
                string alias = email.Substring(1, index - 7);
                return alias;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Uri email = new Uri((string)value);
            return email;
        }
    }
}
