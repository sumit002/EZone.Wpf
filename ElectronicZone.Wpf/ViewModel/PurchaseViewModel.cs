using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility;
using System.Collections.ObjectModel;

namespace ElectronicZone.Wpf.ViewModel
{
    public class PurchaseViewModel : ViewModelBase
    {
        public ObservableCollection<Purchase> PurchaseList { get; set; }
        ILogger logger = new Logger(typeof(PurchaseViewModel));

        /// <summary>
        /// PurchaseViewModel Constructor
        /// </summary>
        public PurchaseViewModel()
        {
            this.PurchaseList = new ObservableCollection<Purchase>();

        }



        private bool CanExecuteAddPurchase(object arg)
        {
            return true;
        }

        private void AddPurchase(object obj)
        {
            // throw new NotImplementedException();
        }

        private void SearchPurchase(object obj)
        {
            // throw new NotImplementedException();
        }

        private void DeletePurchase(object param)
        {
            DataAccess da = new DataAccess();
            //da.DeletePur(((Purchase)param).Id);
            PurchaseList.Remove((Purchase)param);
        }
    }
}
