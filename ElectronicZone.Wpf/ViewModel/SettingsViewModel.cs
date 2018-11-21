using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static ElectronicZone.Wpf.Utility.CommonEnum;

namespace ElectronicZone.Wpf.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public ObservableCollection<String> DownloadPathList { get; set; }
        private IDialogCoordinator dialogCoordinator;
        private bool showReportMenu;
        public bool ShowReportMenu { get => showReportMenu; set => showReportMenu = value; }

        // Commands
        private ICommand asdasdJKLKJNHBGV { get; set; }

        public SettingsViewModel(IDialogCoordinator instance)
        {
            this.dialogCoordinator = instance;
            this.DownloadPathList = new ObservableCollection<string>();
            this.ShowReportMenu = false;

            this.asdasdJKLKJNHBGV = new CommandHandler(AddSettings, CanExecuteAddSettingsCmd);

            LoadSalutation();
        }

        private void AddSettings(object obj)
        {
            throw new NotImplementedException();
        }

        private bool CanExecuteAddSettingsCmd(object arg)
        {
            throw new NotImplementedException();
        }

        private void LoadSalutation()
        {
            // ObservableCollection<string> downloadPathList = CommonEnum.GetEnumNamesObservableCollection<DownloadPath>;
            DownloadPathList = CommonEnum.GetDownloadPathObservableCollection();
        }
    }
}
