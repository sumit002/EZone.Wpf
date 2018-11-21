using ElectronicZone.Wpf.Helper;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicZone.Wpf.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Fields Properties
        private readonly IDialogCoordinator _dialogCoordinator;
        public string Title { get; set; }
        public int SelectedIndex { get; set; }
        public Uri[] FlipViewImages
        {
            get;
            set;
        }

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.Title = "Electronic Zone - Wpf Application";
            this._dialogCoordinator = dialogCoordinator;
            this.FlipViewImages = new Uri[] {
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-4/landscapes/mountains/painted-desert.jpg", UriKind.Absolute),
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-3/landscapes/forest/breaking-the-clouds-on-winter-day.jpg", UriKind.Absolute),
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-4/travel/bodie/bodie-streets.jpg", UriKind.Absolute)
                             };
        }

        #region 
        private bool _quitConfirmationEnabled;
        public bool QuitConfirmationEnabled
        {
            get { return _quitConfirmationEnabled; }
            set
            {
                if (value.Equals(_quitConfirmationEnabled)) return;
                _quitConfirmationEnabled = value;
                // RaisePropertyChanged("QuitConfirmationEnabled");
            }
        }

        private bool showMyTitleBar = true;
        public bool ShowMyTitleBar
        {
            get { return showMyTitleBar; }
            set
            {
                if (value.Equals(showMyTitleBar)) return;
                showMyTitleBar = value;
                //RaisePropertyChanged("ShowMyTitleBar");
            }
        }
        #endregion
    }
}
