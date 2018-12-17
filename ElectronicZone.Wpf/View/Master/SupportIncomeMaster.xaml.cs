using ElectronicZone.Wpf.CustomControls;
using ElectronicZone.Wpf.Events;
using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Data;
using System.Windows.Controls;

namespace ElectronicZone.Wpf.View.Master
{
    /// <summary>
    /// Interaction logic for SupportIncomeMaster.xaml
    /// </summary>
    public partial class SupportIncomeMaster : MetroWindow
    {
        SupportIncomeViewModel vm = new SupportIncomeViewModel(DialogCoordinator.Instance);
        private CalenderBackground background;

        //ILogger logger = new Logger(typeof(SupportIncomeMaster));
        //public ICollectionView SupportPayment { get; private set; }
        public SupportIncomeMaster()
        {
            InitializeComponent();
            this.DataContext = vm;

            //dpSupportDate.DisplayDateEnd = DateTime.Today;
            //AddSelectedDates();
            //PostIncomeAdded();
            //// on esc close
            //this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            SupportIncomeCalendar.IsTodayHighlighted = false;
            SupportIncomeCalendar.DisplayDateEnd = DateTime.Now;
            vm.IncomeAdded += Vm_IncomeAdded;

            InitilizeCalendar();
        }

        private void InitilizeCalendar()
        {
            #region CustomCalendar
            SupportIncomeCalendar.IsTodayHighlighted = false;
            //Kalender.FirstDayOfWeek = DayOfWeek.Sunday;
            //background.ClearDates();
            background = new CalenderBackground(SupportIncomeCalendar);

            //background.AddOverlay("circle", "Resources/Images/circle.png");
            background.AddOverlay("tick", "Resources/Images/tick.png");
            //background.AddOverlay("cross", "Resources/Images/cross.png");
            //background.AddOverlay("box", "Resources/Images/box.png");
            //background.AddOverlay("gray", "Resources/Images/gray.png");

            //SupportIncomeCalendar.ClearValue(Calendar.SelectedDateProperty);
            System.Data.DataTable dtIncomes = vm.GetSupportIncomes();
            foreach (DataRow row in dtIncomes.Rows)
            {
                //SupportIncomeCalendar.SelectedDates.Add(Convert.ToDateTime(row["SupportDate"]));
                background.AddDate(Convert.ToDateTime(row["SupportDate"]), "tick");
            }
            //background.grayoutweekends = "gray";
            SupportIncomeCalendar.Background = background.GetBackground();

            // Update background when changing the displayed month
            //SupportIncomeCalendar.DisplayDateChanged += SupportIncomeCalendarViewOnDisplayDateChanged;
            #endregion
        }

        private void SupportIncomeCalendarViewOnDisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            SupportIncomeCalendar.Background = background.GetBackground();
        }

        private void Vm_IncomeAdded(object source, SupportIncomeEventArgs args)
        {
            //AddSelectedDates();
            InitilizeCalendar();
        }

        //private void PostIncomeAdded()
        //{
        //    // Code   
        //    object resource = Application.Current.FindResource("cdbKey");
        //    Style calStyle = (Style)resource;

        //    // var incomeList = new List<SupportIncome>((IEnumerable<SupportIncome>)this.SupportIncomeList);
        //    //Application.Current.Dispatcher.Invoke(() =>
        //    //{
        //    //foreach (var income in incomeList) {
        //    DateTime holidayDate = DateTime.Parse("12/12/2018");
        //    //DateTime holidayDate = DateTime.Parse(income.SupportDate.ToShortDateString());/*"12/12/2018"*/
        //    DataTrigger dataTrigger = new DataTrigger() { Binding = new Binding("Date"), Value = holidayDate };
        //    dataTrigger.Setters.Add(new Setter(CalendarDayButton.BackgroundProperty, Brushes.LightCoral));
        //    //clear the triggers

        //    calStyle.Triggers.Clear();
        //    calStyle.Triggers.Add(dataTrigger);
        //    //calStyle.Triggers.Add(dataTrigger);
        //    //}
        //    //});
        //}

        //private void AddSelectedDates()
        //{
        //    SupportIncomeCalendar.ClearValue(Calendar.SelectedDateProperty);
        //    System.Data.DataTable dtIncomes = vm.GetSupportIncomes();
        //    foreach (DataRow row in dtIncomes.Rows) {
        //        SupportIncomeCalendar.SelectedDates.Add(Convert.ToDateTime(row["SupportDate"]));
        //    }
        //}

        //private void HandleEsc(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //        Close();
        //}

        //private void btnReset_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        ResetForm();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogException(ex);
        //    }
        //}

        ///// <summary>
        ///// Reset Support Form
        ///// </summary>
        //private void ResetForm()
        //{
        //    this.dpSupportDate.Text = "";
        //    this.txtAmountEarned.Value = null;
        //    this.txtSupportDesc.Text = "";
        //}

        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (validateForm())
        //        {
        //            using (DataAccess da = new DataAccess()) {
        //                //create record
        //                Dictionary<string, string> folderFields = new Dictionary<string, string>();
        //                folderFields.Add("Id", null);
        //                folderFields.Add("Description", txtSupportDesc.Text);
        //                folderFields.Add("Amount", txtAmountEarned.Value.ToString());
        //                folderFields.Add("SupportDate", (DateTime.Parse(dpSupportDate.Text).ToString(ConfigurationManager.AppSettings["DateTimeFormat"])));
        //                //folderFields.Add("Remarks", null);


        //                int rslt = da.InsertOrUpdateSupportPaymentMaster(folderFields, "tblSupportPaymentMaster");
        //                if (rslt > 0)
        //                {
        //                    PaymentTransaction paymentTransaction = new PaymentTransaction();
        //                    bool paymentStatus = paymentTransaction.AddPaymentTransaction(1, double.Parse(txtAmountEarned.Value.ToString()), CommonEnum.PaymentStatus.SUPPORT_PAYMENT, rslt, da);
        //                    if (paymentStatus)
        //                    {
        //                        MessageBoxResult result = MessageBox.Show("Payment Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //                        ResetForm();
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBoxResult result = MessageBox.Show("Error While Adding Payment!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            MessageBoxResult result = MessageBox.Show((string)Application.Current.FindResource("InvalidFormDataWarningMessage"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogException(ex);
        //    }
        //}

        //private bool validateForm()
        //{
        //    if (string.IsNullOrEmpty(dpSupportDate.Text.Trim()))
        //        return false;
        //    else if (string.IsNullOrEmpty(txtAmountEarned.Value.ToString()))
        //        return false;
        //    else if (string.IsNullOrEmpty(txtSupportDesc.Text.Trim()))
        //        return false;
        //    else
        //        return true;
        //}

        //private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.Source is TabControl)
        //    {
        //        //bind support list on demand
        //        if (tabControl1.SelectedIndex != 0)
        //            loadSupportPayment();
        //    }
        //}


        //private void loadSupportPayment()
        //{
        //    DataAccess da = new DataAccess();
        //    DataTable dtSupportPayment = da.GetAllSupportPayment(String.Empty);

        //    //List<SupportPayment> supportList = dtSupportPayment.DataTableToList<SupportPayment>();
        //    //List<SupportPayment> list = dtSupportPayment.ToList<SupportPayment>();// use this
        //    //SupportPayment = CollectionViewSource.GetDefaultView(supportList);

        //    datagridSupportPayment.ItemsSource = dtSupportPayment.DefaultView;
        //    //datagridStocks.UpdateLayout();
        //}
    }
}
