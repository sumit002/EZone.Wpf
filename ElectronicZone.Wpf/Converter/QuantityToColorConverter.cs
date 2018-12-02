using ElectronicZone.Wpf.Model;
using System;
using System.Windows.Data;

namespace ElectronicZone.Wpf.Converter
{
    /// <summary>
    /// Changing Color based on Available Qty For Purchase Order
    /// </summary>
    public class QuantityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Purchase drv = value as Purchase;
            if (drv != null)
                return drv.Quantity == drv.AvlQuantity;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
