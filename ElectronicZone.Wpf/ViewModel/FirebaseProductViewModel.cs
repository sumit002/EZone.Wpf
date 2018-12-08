//using ElectronicZone.Wpf.FireBaseModel;
using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;

namespace ElectronicZone.Wpf.ViewModel
{
    public class FirebaseProductViewModel : ViewModelBase
    {
        #region Properties
        ILogger logger = new Logger(typeof(FirebaseProductViewModel));
        //private IDialogCoordinator dialogCoordinator;
        public ObservableCollection<FirebaseProduct> FirebaseProductList { get; set; }
        //private readonly FirebaseApp _app;
        #endregion

        public FirebaseProductViewModel()
        {
            this.FirebaseProductList = new ObservableCollection<FirebaseProduct>();
            //this._app = new FirebaseApp(new Uri("https://productstore-2fdc7.firebaseio.com") /*, <auth token> */);
            //this._app = new FirebaseApp(new Uri("https://productstore-2fdc7.firebaseio.com")
            //    , "AIzaSyBumXHPSSMKY8lV2XBh--luHYI9V1DM0cc");
            
            GetData();
        }

        private void GetData()
        {
            //var productsRef = _app.Child("products");

            //productsRef.OrderByValue().LimitToLast(3)
            //.On("value", (snapshot, child, context) => {
            //    foreach (var data in snapshot.Children) {
            //        this.FirebaseProductList.Add(new FirebaseProduct() {
            //            Name = data.Value<string>()
            //        });
            //        Console.WriteLine("The {0} products\'s score is {1}",
            //                            data.Key, data.Value<int>());
            //    }
            //});
        }
    }
}
