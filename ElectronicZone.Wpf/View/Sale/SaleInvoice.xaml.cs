using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ElectronicZone.Wpf.View.Sale
{
    /// <summary>
    /// Interaction logic for SaleInvoice.xaml
    /// </summary>
    public partial class SaleInvoice : MetroWindow
    {
        SaleInvoiceViewModel vm = new SaleInvoiceViewModel();

        public SaleInvoice()
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
