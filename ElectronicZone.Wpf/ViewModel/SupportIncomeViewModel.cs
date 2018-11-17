using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Events;
using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility;
using ElectronicZone.Wpf.Utility.EMail;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace ElectronicZone.Wpf.ViewModel
{
    public class SupportIncomeViewModel : ViewModelBase
    {
        #region Properties
        ILogger logger = new Logger(typeof(PurchaseViewModel));
        private IDialogCoordinator dialogCoordinator;
        public ObservableCollection<SupportIncome> SupportIncomeList { get; set; }


        #endregion

        #region Events
        // 1- Define a delegate
        // 2- Define a event based on the delegate
        // 3- Raise the event
        public delegate void IncomeAddedEventHandler(object source, SupportIncomeEventArgs args);
        public event IncomeAddedEventHandler IncomeAdded;
        protected virtual void OnIncomeAdded(SupportIncome obj)
        {
            SupportIncomeEventArgs evtArgs = new SupportIncomeEventArgs();
            evtArgs.SupportIncome = obj;
            IncomeAdded?.Invoke(this, evtArgs);
        }
        #endregion

        #region UI Models
        private int _id;
        private DateTime _supportIncomeDate;
        private double _amountEarned;
        private string _description;
        private string _remarks;
        private string _tabHeaderText;
        private bool _isAddMode;
        private DateTime _selectedCalendarDate;

        public int Id { get => _id; set => _id = value; }
        public DateTime SupportIncomeDate { get => _supportIncomeDate; set { _supportIncomeDate = value; OnPropertyChanged(); } }
        public double AmountEarned { get => _amountEarned; set { _amountEarned = value; OnPropertyChanged(); } }
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
        public string Remarks { get => _remarks; set { _remarks = value; OnPropertyChanged(); } }
        public string TabHeaderText { get => _tabHeaderText; set { _tabHeaderText = value; OnPropertyChanged(); } }
        public bool IsAddMode { get => _isAddMode; set { _isAddMode = value; OnPropertyChanged();
                if(_isAddMode) this.TabHeaderText = "Add Support Income"; else this.TabHeaderText = "Edit Support Income";
            } }
        public DateTime SelectedCalendarDate { get => _selectedCalendarDate; set { _selectedCalendarDate = value;
                OnPropertyChanged(); IncomeCalendarDateChanged(); } }


        private int _selectedIndex;
        public int TabSelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                if (_selectedIndex == 1) { GetAllSupportIncomes(); };
                if (_selectedIndex == 2) { InitializeSupportIncomeCalendar(); }
            }
        }
        #endregion

        #region Commands
        public ICommand AddSupportIncomeCmd { get; set; }
        public ICommand SupportIncomeResetCmd { get; set; }

        public ICommand EditSupportIncomeCmd { get; set; }
        public ICommand DeleteSupportIncomeCmd { get; set; }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="instance"></param>
        public SupportIncomeViewModel(IDialogCoordinator instance)
        {
            this.TabSelectedIndex = 0;
            this.dialogCoordinator = instance;
            this.SupportIncomeDate = DateTime.Now;
            //dpSupportDate.DisplayDateEnd = DateTime.Today;
            //this.TabHeaderText = "Add Support Income";
            this.IsAddMode = true;

            this.SupportIncomeList = new ObservableCollection<SupportIncome>();

            //InitializeSupportIncomeCalendar();

            this.AddSupportIncomeCmd = new CommandHandler(AddSupportIncome, CanExecuteAddSupportIncome);
            this.SupportIncomeResetCmd = new CommandHandler(SupportIncomeReset, CanExecuteSupportIncomeReset);
            this.EditSupportIncomeCmd = new CommandHandler(EditSupportIncome, CanExecuteEditSupportIncome);
            this.DeleteSupportIncomeCmd = new CommandHandler(DeleteSupportIncome, CanExecuteDeleteSupportIncome);

            GetAllSupportIncomes();
        }

        #region Methods
        private bool CanExecuteDeleteSupportIncome(object arg)
        {
            return true;
        }
        private void DeleteSupportIncome(object obj)
        {
            var item = (SupportIncome)obj;
            // Checking validations to delete 
            if (item.Amount >= 0){
                if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes) {
                    using (DataAccess da = new DataAccess())
                    {
                        int sPaymentId = da.DeleteSupportPayment(item.Id);
                        // Reverse Payment Transaction
                        PaymentTransaction ptrans = new PaymentTransaction();
                        bool isReversed = ptrans.ReversePaymentTransaction(Global.UserId, item.Amount, CommonEnum.PaymentStatus.SUPPORTREVERSAL_PAYMENT, item.Id, da);
                        if (isReversed) {
                            MessageBoxResult result = MessageBox.Show("Payment Deleted Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        this.SupportIncomeList.Remove(item);
                    }
                }
            }
            else { MessageBoxResult result = MessageBox.Show("Payment Cannot be deleted!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private bool CanExecuteEditSupportIncome(object arg)
        {
            return true;
        }
        private void EditSupportIncome(object obj)
        {
            try
            {
                AssignSupportIncomeModelProperties((SupportIncome)obj);
                //this.TabHeaderText = "Edit Support Income";
                this.IsAddMode = false;
                this.TabSelectedIndex = 0;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void AssignSupportIncomeModelProperties(SupportIncome obj)
        {
            this.Id = obj.Id;
            this.SupportIncomeDate = obj.SupportDate;
            this.AmountEarned = obj.Amount;
            this.Description = obj.Description;
            this.Remarks = obj.Remarks;
        }

        private bool CanExecuteSupportIncomeReset(object arg)
        {
            return true;
        }

        private void SupportIncomeReset(object obj)
        {
            ResetFeilds();
        }

        private void ResetFeilds()
        {
            try
            {
                this.Id = 0;
                this.SupportIncomeDate = DateTime.Now;
                this.AmountEarned = 0;
                this.Description = "";
                this.Remarks = "";

                //this.TabHeaderText = "Add Support Income";
                this.IsAddMode = true;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private bool CanExecuteAddSupportIncome(object arg)
        {
            return true;
        }

        private void AddSupportIncome(object obj)
        {
            if (ValidateSupportIncome())
            {
                using (DataAccess da = new DataAccess()) {
                    try
                    { 
                        //create record
                        Dictionary<string, string> folderFields = new Dictionary<string, string>();
                        folderFields.Add("Id", this.Id == 0 ? null : this.Id.ToString());
                        folderFields.Add("Description", this.Description);
                        folderFields.Add("Amount", this.AmountEarned.ToString());
                        folderFields.Add("SupportDate", (this.SupportIncomeDate.ToString(ConfigurationManager.AppSettings["DateOnly"])));
                        folderFields.Add("Remarks", this.Remarks);
                        int rslt = da.InsertOrUpdateSupportPaymentMaster(folderFields, "tblSupportPaymentMaster");
                        if (rslt > 0) {
                            if (this.IsAddMode)
                            {
                                PaymentTransaction payTrans = new PaymentTransaction();
                                bool paymentStatus = payTrans.AddPaymentTransaction(Global.UserId, this.AmountEarned, CommonEnum.PaymentStatus.SUPPORT_PAYMENT, rslt, da);
                                if (paymentStatus)
                                {
                                    MessageBoxResult result = MessageBox.Show("Support Income Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                    //ResetFeilds();
                                }
                                // Notify Calendar to Update
                                OnIncomeAdded(new SupportIncome() { Amount = this.AmountEarned, SupportDate = this.SupportIncomeDate });
                                SendIncomeEmail();
                                ResetFeilds();
                            }
                            else { MessageBoxResult result = MessageBox.Show("Support Income Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);  }
                        }
                        else {
                            MessageBoxResult result = MessageBox.Show("Error While Adding Support Income!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(ex);
                        // da.RollbackTransaction();
                    }
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Invalid Data ! Please check the fields entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendIncomeEmail()
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmailOnSupportIncome"]))
            {
                //Place holder for sending emails
                Dictionary<string, string> placeHolders = new Dictionary<string, string>();
                placeHolders.Add("{AmountEarned}", this.AmountEarned.ToString("00.00", CultureInfo.InvariantCulture));
                placeHolders.Add("{SupportIncomeDate}", this.SupportIncomeDate.ToString(ConfigurationManager.AppSettings["DateDisplay"]));
                placeHolders.Add("{Description}", this.Description);

                EmailParams emailParams = new EmailParams(ConfigurationManager.AppSettings["AdminEmail"], SendEmailType.SupportIncomeMail, placeHolders, ccMail: string.Empty, mailAttachments: string.Empty);
                EmailUtility.SendMail(emailParams);
                //MailService mailService = new MailService();
                //mailService.SendMail(ConfigurationManager.AppSettings["AdminEmail"]);
            }
        }

        private bool ValidateSupportIncome()
        {
            if (string.IsNullOrEmpty(this.SupportIncomeDate.ToShortDateString()))
                return false;
            else if (string.IsNullOrEmpty(this.AmountEarned.ToString()))
                return false;
            else if (string.IsNullOrEmpty(this.Description))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Get All Support Incomes
        /// </summary>
        private void GetAllSupportIncomes()
        {
            try
            {
                DataTable dtSupportPayment = new DataTable();
                using (DataAccess da = new DataAccess()) {
                    dtSupportPayment = da.GetAllSupportPayment(String.Empty);
                }

                this.SupportIncomeList.Clear();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (DataRow row in dtSupportPayment.Rows)
                    {
                        this.SupportIncomeList.Add(new SupportIncome()
                        {
                            Id = int.Parse(row["Id"].ToString()),
                            Description = Convert.ToString(row["Description"]),
                            Amount = Convert.ToDouble(row["Amount"].ToString()),
                            SupportDate = Convert.ToDateTime(row["SupportDate"]),
                            Remarks = Convert.ToString(row["Remarks"])
                            //IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }
        /// <summary>
        /// https://stackoverflow.com/questions/34469567/change-background-of-some-holiday-dates-in-calendar-control
        /// </summary>
        private void InitializeSupportIncomeCalendar() {
            object resource = Application.Current.FindResource("cdbKey");
            Style calStyle = (Style)resource;

            var incomeList = new List<SupportIncome>((IEnumerable<SupportIncome>)this.SupportIncomeList);
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    //foreach (var income in incomeList) {
            //    DateTime holidayDate = DateTime.Parse("12/12/2018");
            //    //DateTime holidayDate = DateTime.Parse(income.SupportDate.ToShortDateString());/*"12/12/2018"*/
            //    DataTrigger dataTrigger = new DataTrigger() { Binding = new Binding("Date"), Value = holidayDate };
            //    dataTrigger.Setters.Add(new Setter(CalendarDayButton.BackgroundProperty, Brushes.LightCoral));
            //    calStyle.Triggers.Add(dataTrigger);
            //    //}
            //});
            

        }
        // public event EventHandler<NotificationEventArgs<string>> DoSomething;

        public DataTable GetSupportIncomes() {
            DataTable dtSupportPayment = new DataTable();
            using (DataAccess da = new DataAccess())
            {
                dtSupportPayment = da.GetAllSupportPayment(String.Empty);
                return dtSupportPayment;
            }
        }

        private void IncomeCalendarDateChanged()
        {
            var incomeList = new List<SupportIncome>((IEnumerable<SupportIncome>)this.SupportIncomeList);
            // SupportIncomeCalendar.SelectedDates.Add(new DateTime(2018, 11, 5));
            if (MessageBox.Show($"Do you want to add income for {SelectedCalendarDate.ToShortDateString()}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                //MessageBoxResult result = MessageBox.Show($"Calendar Date Changed:{SelectedCalendarDate.ToShortDateString()}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //AssignSupportIncomeModelProperties((SupportIncome)obj);
                this.SupportIncomeDate = SelectedCalendarDate;
                //this.TabHeaderText = "Edit Support Income";
                this.IsAddMode = true;
                this.TabSelectedIndex = 0;
            }
        }
        #endregion
    }
}
