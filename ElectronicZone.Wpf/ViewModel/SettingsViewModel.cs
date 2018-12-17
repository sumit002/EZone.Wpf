using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ElectronicZone.Wpf.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Properties
        public ObservableCollection<String> DownloadPathList { get; set; }
        ILogger logger = new Logger(typeof(SettingsViewModel));
        private IDialogCoordinator _dialogCoordinator;
        private bool showReportMenu;
        public bool ShowReportMenu { get => showReportMenu; set => showReportMenu = value; } 
        #endregion

        // Commands
        //private ICommand SaveSettingsCommand { get; set; }

        /// <summary>
        /// Settings ViewModel Constructor
        /// </summary>
        /// <param name="instance"></param>
        public SettingsViewModel(IDialogCoordinator instance)
        {
            this._dialogCoordinator = instance;
            this.DownloadPathList = new ObservableCollection<string>();
            this.ShowReportMenu = false;

            //this.SaveSettingsCommand = new CommandHandler(AddSettings, CanExecuteAddSettingsCmd);

            this.DownloadPathList = CommonEnum.GetDownloadPathObservableCollection();
        }

        private void AddSettings(object obj)
        {
            try
            {
                // Implemet Save Settings

            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private bool CanExecuteAddSettingsCmd(object arg)
        {
            return true;
        }

        //private void LoadSalutation()
        //{
        //    // ObservableCollection<string> downloadPathList = CommonEnum.GetEnumNamesObservableCollection<DownloadPath>;
        //    DownloadPathList = CommonEnum.GetDownloadPathObservableCollection();
        //}
    }
}
