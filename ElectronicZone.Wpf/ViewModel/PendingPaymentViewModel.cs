using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace ElectronicZone.Wpf.ViewModel
{
    public class PendingPaymentViewModel : ViewModelBase
    {
        #region Properties
        ILogger logger = new Logger(typeof(PendingPaymentViewModel));
        private IDialogCoordinator _dialogCoordinator;
        public ObservableCollection<Model.PendingPayment> PendingPaymentList { get; set; }
        public ObservableCollection<Contact> ContactList { get; set; }
        #endregion

        #region UI Models
        private Contact _sContact;
        public Contact SContact
        {
            get { return _sContact; }
            set { _sContact = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand SearchPendingPaymentCmd { get; set; }
        public ICommand ResetSearchPendingPaymentCmd { get; set; }
        public ICommand ClearPendingPaymentCmd { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instance"></param>
        public PendingPaymentViewModel(IDialogCoordinator instance)
        {
            try
            {
                this._dialogCoordinator = instance;
                this.PendingPaymentList = new ObservableCollection<PendingPayment>();
                this.ContactList = new ObservableCollection<Contact>();

                this.SearchPendingPaymentCmd = new CommandHandler(SearchPendingPayment, CanExecuteSearchPendingPayment);
                this.ResetSearchPendingPaymentCmd = new CommandHandler(ResetSearchPendingPayment, CanExecuteSearchPendingPayment);
                this.ClearPendingPaymentCmd = new CommandHandler(ClearPendingPayment, CanExecuteSearchPendingPayment);

                LoadSalesPerson();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        /// <summary>
        /// Load All Contacts
        /// </summary>
        private void LoadSalesPerson()
        {
            DataTable dtProduct = new DataTable();
            using (DataAccess da = new DataAccess())
            {
                dtProduct = da.GetAllSalesPerson();
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (DataRow row in dtProduct.Rows)
                {
                    this.ContactList.Add(new Contact()
                    {
                        Id = int.Parse(row["Id"].ToString()),
                        Name = Convert.ToString(row["Name"]),
                        PrimaryContact = Convert.ToString(row["Contact"])
                    });
                }
            });
        }

        private void ClearPendingPayment(object obj)
        {
            try
            {
                var item = (Model.PendingPayment)obj;

                //open modal for sale Item
                View.Payment.ClearPending saleScreen = new View.Payment.ClearPending(item);
                saleScreen.ShowDialog();
                //refresh Pending Payment data
                LoadPendingPayments();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void ResetSearchPendingPayment(object obj)
        {
            ResetPendingPayment();
        }

        private void ResetPendingPayment()
        {
            try
            {
                this.SContact = null;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void SearchPendingPayment(object obj)
        {
            // Load Pending Payments
            LoadPendingPayments();
        }

        /// <summary>
        /// Load Pending Payment
        /// </summary>
        private void LoadPendingPayments()
        {
            try
            {
                DataTable dtPendingPayment = new DataTable();
                using (DataAccess da = new DataAccess()) {
                    dtPendingPayment = da.SearchPendingPayment(null, null, string.Empty, string.Empty
                        , (this.SContact == null ? string.Empty : this.SContact.Id.ToString()), 0);
                }
                this.PendingPaymentList.Clear();
                if (dtPendingPayment.Rows.Count > 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (DataRow row in dtPendingPayment.Rows)
                        {
                            this.PendingPaymentList.Add(new PendingPayment()
                            {
                                Id = int.Parse(row["PendingPaymentId"].ToString()),
                                Name = Convert.ToString(row["Name"]),
                                PrimaryContact = Convert.ToString(row["Contact"]),
                                Total = Convert.ToDouble(row["Total"]),
                                PaidAmount = Convert.ToDouble(row["AmountPaid"]),
                                PendingAmount = Convert.ToDouble(row["PendingAmount"]),
                                SaleDate = Convert.ToDateTime(row["SaleDate"]),

                                SaleId = int.Parse(row["SaleId"].ToString()),

                                MinAmountForDiscount = Convert.ToDouble(row["MinAmtToAvailDiscount"]),
                                SalePersonId = int.Parse(row["PendingPaymentId"].ToString()),
                                SalePersonToDisplay = Convert.ToString(row["Name"]),
                                ProductToDisplay = Convert.ToString(row["Product"]),
                                ProductCodeToDisplay = Convert.ToString(row["ProductCode"]),
                            });
                        }
                    });
                }
                else {
                    MessageBoxResult result = MessageBox.Show("No Data Found!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private bool CanExecuteSearchPendingPayment(object arg)
        {
            return true;
        }
    }
}
