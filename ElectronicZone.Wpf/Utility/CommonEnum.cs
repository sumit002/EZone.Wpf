using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ElectronicZone.Wpf.Utility
{
    public class CommonEnum
    {
        public enum PaymentStatus
        {
            PENDING_PAYMENT,
            PURCHASE_PAYMENT,
            SALE_PAYMENT,
            SUPPORT_PAYMENT,
            SALEREVERSAL_PAYMENT,
            PURCHASEREVERSAL_PAYMENT,
            SUPPORTREVERSAL_PAYMENT
        }

        public enum Salutation
        {
            Mr,
            Miss,
            Mrs,
        }

        public enum DownloadPath
        {
            [Description("Application Data")]
            ApplicationData = Environment.SpecialFolder.ApplicationData,

            [Description("My Documents")]
            Documents = Environment.SpecialFolder.MyDocuments,

            [Description("Desktop")]
            Desktop = Environment.SpecialFolder.Desktop,

            MyMusic,
            MyPictures,
            MyVideos,

            [Description("Downloads")]
            Downloads,
        }
        public static List<T> GetEnumValuesList<T>()
        {
            List<T> list = System.Enum.GetValues(typeof(T))
                          .Cast<T>()
                          .ToList<T>();
            return list;
        }

        public static ObservableCollection<string> GetEnumNamesObservableCollection<T>()
        {
            return new ObservableCollection<string>(Enum.GetNames(typeof(T)));
        }

        public static ObservableCollection<string> GetDownloadPathObservableCollection()
        {
            return new ObservableCollection<string>(Enum.GetNames(typeof(DownloadPath)));
        }

        //public static ObservableCollection<string> GetPaymentStatusObservableCollection()
        //{
        //    return new ObservableCollection<string>(Enum.GetNames(typeof(PaymentStatus)));
        //}
    }
}
