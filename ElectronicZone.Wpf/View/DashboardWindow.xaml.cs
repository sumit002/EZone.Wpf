using System;
using System.Windows;
using MahApps.Metro.Controls;
using ElectronicZone.Wpf.Utility;
using LiveCharts;
using System.Data;
using System.Globalization;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using ElectronicZone.Wpf.View.Sale;
using ElectronicZone.Wpf.View.Master;
using ElectronicZone.Wpf.View.Report;
using ElectronicZone.Wpf.View.Payment;
using ElectronicZone.Wpf.View.Common;
using ElectronicZone.Wpf.DataAccessLayer;

namespace ElectronicZone.Wpf.View
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : MetroWindow
    {
        #region properties
        string fromDate = string.Empty;
        string toDate = string.Empty;
        ILogger logger = new Logger(typeof(DashboardWindow));

        public Func<ChartPoint, string> PointLabel { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollectionDonut { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        // public object totalPayment = { async = null, c = null  };
        private decimal _totalPurchasePayment, _totalSaleIncome, _totalSupportIncome, _totalIncome = 0;
        #endregion

        public DashboardWindow()
        {
            InitializeComponent();
            InitializeSettings();
            //set dates for dashboard result
            DateTimeUtility dtUtility = new DateTimeUtility();
            dpDashboardFromDate.SelectedDate = dtUtility.getMonthStartDate();
            dpDashboardToDate.SelectedDate = DateTime.Now;

            SetDate();

            loadDashboardComponents();

            // InitializeCharts();
        }

        #region Functions & Methods
        private void SetDate()
        {
            fromDate = string.IsNullOrEmpty(dpDashboardFromDate.Text) ? "" : (DateTime.Parse(dpDashboardFromDate.Text).ToString("yyyy-MM-dd HH:mm:ss"));
            toDate = string.IsNullOrEmpty(dpDashboardToDate.Text) ? "" : (DateTime.Parse(dpDashboardToDate.Text).ToString("yyyy-MM-dd HH:mm:ss"));
        }
        private void loadDashboardComponents()
        {
            try
            {
                getPaymentIncome();
                getPaymentInvest();

                // load top 5 Pending Payments
                loadTopPendingPayment();

                // load top 5 Sales
                loadTopSales();

                // load top 5 Purchases
                loadTopPurchases();

                // load top 5 Sellers
                loadTopSellers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.LogException(ex);
            }
        }

        private void getPaymentInvest()
        {
            DataAccess da = new DataAccess();
            DataTable dtPurchaseInvest = da.SearchPaymentInvest(fromDate, toDate, PaymentTransaction.PaymentStatus.PURCHASE_PAYMENT.ToString());

            _totalPurchasePayment = CommonMethods.GetSum(dtPurchaseInvest, "Amount");
            tbTotalPurchasePayment.Text = _totalPurchasePayment.ToString("F", CultureInfo.InvariantCulture);
        }

        private void getPaymentIncome()
        {
            DataAccess da = new DataAccess();
            DataTable dtSupportIncome = da.SearchPaymentIncome(fromDate, toDate, PaymentTransaction.PaymentStatus.SUPPORT_PAYMENT.ToString());

            _totalSupportIncome = CommonMethods.GetSum(dtSupportIncome, "Amount");
            tbTotalSupportIncome.Text = _totalSupportIncome.ToString("F", CultureInfo.InvariantCulture);

            DataTable dtSaleIncome = da.SearchPaymentIncome(fromDate, toDate, PaymentTransaction.PaymentStatus.SALE_PAYMENT.ToString());

            _totalSaleIncome = CommonMethods.GetSum(dtSaleIncome, "Amount");
            tbTotalSaleIncome.Text = _totalSaleIncome.ToString("F", CultureInfo.InvariantCulture);

            _totalIncome = CommonMethods.GetSum(dtSupportIncome, "Amount") + CommonMethods.GetSum(dtSaleIncome, "Amount");
            tbTotalIncome.Text = _totalIncome.ToString("F", CultureInfo.InvariantCulture);
        }

        private void loadTopPendingPayment()
        {
            DataTable dtPending = new DataTable();
            DataAccess da = new DataAccess();
            dtPending = da.SearchPendingPayment(null, null, fromDate, toDate, string.Empty, 0);
            // soprt Column and Select top 5
            dtPending = CommonMethods.SortTable(dtPending, "PendingAmount", true);
            dtPending = CommonMethods.GetTopRow(dtPending);
            // bind to dataGrid
            dataGridPendingPayment.ItemsSource = dtPending.DefaultView;
        }

        private void loadTopSales()
        {
            DataTable dtSales = new DataTable();
            DataAccess da = new DataAccess();
            dtSales = da.SearchSales(string.Empty, string.Empty, string.Empty, string.Empty, null, null, fromDate, toDate, string.Empty);
            // sort Column and Select top 5
            dtSales = CommonMethods.SortTable(dtSales, "Total", true);
            dtSales = CommonMethods.GetTopRow(dtSales);
            // bind to dataGrid
            dataGridTopSales.ItemsSource = dtSales.DefaultView;
        }

        private void loadTopPurchases()
        {
            DataTable dtPurchase = new DataTable();
            DataAccess da = new DataAccess();
            dtPurchase = da.SearchStocks(string.Empty, string.Empty, string.Empty, string.Empty, null, null, fromDate, toDate);
            // sort Column and Select top 5
            dtPurchase = CommonMethods.SortTable(dtPurchase, "PurchasePrice", true);
            dtPurchase = CommonMethods.GetTopRow(dtPurchase);
            // bind to dataGrid
            dataGridTopPurchases.ItemsSource = dtPurchase.DefaultView;
        }

        private void loadTopSellers()
        {
            // ToDO : 
        }

        private void validateCalendarMinMaxDate()
        {
            try
            {
                this.dpDashboardFromDate.DisplayDateEnd = string.IsNullOrEmpty(dpDashboardToDate.Text) ? (DateTime?)null : DateTime.Parse(dpDashboardToDate.Text);
                this.dpDashboardToDate.DisplayDateStart = string.IsNullOrEmpty(dpDashboardFromDate.Text) ? (DateTime?)null : DateTime.Parse(dpDashboardFromDate.Text);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void InitializeSettings()
        {
            menuReports.Visibility = chkbShowReports.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void InitializeCharts()
        {
            PointLabel = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            pieChart.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Sale Income",
                    Values = new ChartValues<decimal> {_totalSaleIncome},
                    // PushOut = 10,
                    DataLabels = true,
                    LabelPoint = PointLabel
                },
                new PieSeries
                {
                    Title = "Support Income",
                    Values = new ChartValues<decimal> {_totalSupportIncome},
                    DataLabels = true,
                    LabelPoint = PointLabel
                }
            };

            SeriesCollectionDonut = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Chrome",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(8) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Mozilla",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Opera",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Explorer",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(4) },
                    DataLabels = true
                }
            };

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2015",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "2016",
                Values = new ChartValues<double> { 11, 56, 42, 48 }
            });
            //also adding values updates and animates the chart automatically
            //SeriesCollection[1].Values.Add(48d);

            Labels = new[] { "Maria", "Susan", "Charles", "Frida" };
            Formatter = value => value.ToString("N");

            DataContext = this;
        }
        #endregion

        #region events

        private void btnDashboardResult_Click(object sender, RoutedEventArgs e)
        {
            loadDashboardComponents();
            InitializeCharts();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            //SoundPlayer asd = new SoundPlayer(@"e:\3D_drums.wav");// it requires wmv files only
            //asd.Play();
            Info info = new Info();
            info.ShowDialog();
        }

        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            AboutUs about = new AboutUs();
            about.ShowDialog();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
        }

        private void RegisterProduct_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Registration will be available soon!", "Coming Soon!", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Update will be available soon!", "Coming Soon!", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void TechnicalSupport_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Technical Support will be available soon!", "Coming Soon!", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void Invoice_Click(object sender, RoutedEventArgs e)
        {
            SaleInvoice saleInvoice = new SaleInvoice();
            saleInvoice.ShowDialog();
        }

        private void InvoiceMaster_Click(object sender, RoutedEventArgs e)
        {
            InvoiceMaster invMaster = new InvoiceMaster();
            invMaster.ShowDialog();
        }

        


        // Masters Section
        private void ProductMaster_Click(object sender, RoutedEventArgs e)
        {
            ProductMaster product = new ProductMaster();
            product.ShowDialog();
        }

        private void BrandMaster_Click(object sender, RoutedEventArgs e)
        {
            BrandMaster brand = new BrandMaster();
            brand.ShowDialog();
        }

        private void StockMaster_Click(object sender, RoutedEventArgs e)
        {
            PurchaseMaster purchaseMaster = new PurchaseMaster();
            purchaseMaster.ShowDialog();
        }

        private void Sale_Click(object sender, RoutedEventArgs e)
        {
            SaleMaster sale = new SaleMaster();
            sale.ShowDialog();
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            SupportIncomeMaster supportPayment = new SupportIncomeMaster();
            supportPayment.ShowDialog();
        }

        // Reports Section
        private void SalesReport_Click(object sender, RoutedEventArgs e)
        {
            SalesReport salesReport = new SalesReport();
            salesReport.ShowDialog();
        }

        private void PurchaseReport_Click(object sender, RoutedEventArgs e)
        {
            PurchaseReport purchaseReport = new PurchaseReport();
            purchaseReport.ShowDialog();
        }

        private void SupportPaymentReport_Click(object sender, RoutedEventArgs e)
        {
            SupportPaymentReport supportPaymentReport = new SupportPaymentReport();
            supportPaymentReport.ShowDialog();
        }

        private void ContactReport_Click(object sender, RoutedEventArgs e)
        {
            ContactReport contactReport = new ContactReport();
            contactReport.ShowDialog();
        }

        private void ContactsReport_Click(object sender, RoutedEventArgs e)
        {
            ContactReport contactReport = new ContactReport();
            contactReport.ShowDialog();
        }

        private void PendingPaymentReport_Click(object sender, RoutedEventArgs e)
        {
            PendingPaymentReport pendingPaymentReport = new PendingPaymentReport();
            pendingPaymentReport.ShowDialog();
        }

        private void Contact_Click(object sender, RoutedEventArgs e)
        {
            ContactMaster contactMaster = new ContactMaster();
            contactMaster.ShowDialog();
        }

        private void dpDashboardFromDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            validateCalendarMinMaxDate();
            SetDate();
        }

        private void dpDashboardToDate_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            validateCalendarMinMaxDate();
            SetDate();
        }

        private void PendingPayment_Click(object sender, RoutedEventArgs e)
        {
            PendingPayment pendingPayment = new PendingPayment();
            pendingPayment.Show();
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            InitializeSettings();
        }
        #endregion
    }
}
