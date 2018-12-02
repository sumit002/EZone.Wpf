using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ElectronicZone.Wpf.View.Report
{
    /// <summary>
    /// Interaction logic for ContactReport.xaml
    /// </summary>
    public partial class ContactReport : MetroWindow
    {
        ContactViewModel vm = new ContactViewModel(DialogCoordinator.Instance);
        public ContactReport()
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
