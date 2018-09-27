using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls;

namespace ElectronicZone.Wpf.View.Report
{
    /// <summary>
    /// Interaction logic for ContactReport.xaml
    /// </summary>
    public partial class ContactReport : MetroWindow
    {
        ContactViewModel vm = new ContactViewModel();
        public ContactReport()
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
