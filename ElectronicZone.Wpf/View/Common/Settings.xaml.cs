using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;

namespace ElectronicZone.Wpf.View.Common
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        SettingsViewModel vm = new SettingsViewModel(DialogCoordinator.Instance);

        public Settings()
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
