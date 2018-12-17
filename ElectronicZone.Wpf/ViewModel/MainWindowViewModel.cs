using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Utility;
using ElectronicZone.Wpf.View.Master;
using ElectronicZone.Wpf.View.Payment;
using ElectronicZone.Wpf.View.Report;
using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static ElectronicZone.Wpf.Utility.CommonEnum;

namespace ElectronicZone.Wpf.ViewModel
{
    /// <summary>
    /// ViewModel Class For MainWindow
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields/Properties
        private readonly IDialogCoordinator _dialogCoordinator;
        public string Title { get; set; }
        public int SelectedIndex { get; set; }
        public Uri[] FlipViewImages
        {
            get;
            set;
        }

        public Func<ChartPoint, string> PointLabel { get; set; }
        public SeriesCollection Series { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<Model.Purchase> PurchaseOUtofStockList { get; set; }
        public ObservableCollection<Model.Sale> SaleOnDemandList { get; set; }
        public Model.User _user { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dialogCoordinator"></param>
        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.Title = "Electronic Zone";
            this._dialogCoordinator = dialogCoordinator;
            this.FlipViewImages = new Uri[] {
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-4/landscapes/mountains/painted-desert.jpg", UriKind.Absolute),
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-3/landscapes/forest/breaking-the-clouds-on-winter-day.jpg", UriKind.Absolute),
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-4/travel/bodie/bodie-streets.jpg", UriKind.Absolute)
                             };

            this.EndDate = DateTime.Today;
            this.StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            // this.AddPurchaseCommand = new CommandHandler(AddPurchaseStock, CanExecuteAddPurchaseStock);

            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            this.Series = new SeriesCollection {};
            this.SeriesCollection = new SeriesCollection { };
            this.PurchaseOUtofStockList = new ObservableCollection<Model.Purchase>();
            this.SaleOnDemandList = new ObservableCollection<Model.Sale>();
            this._user = new Model.User() { Id = Global.UserId, Name = Global.Name, IsAdmin = Global.IsAdmin };
        }

        private int _selectedIndex;
        public int TabSelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                if (_selectedIndex == 1) {
                    LoadDasboardAnalysis();
                } else if (_selectedIndex == 2) {
                    LoadStockAnalysis();
                } else if (_selectedIndex == 3) {
                    LoadSaleAnalysis();
                };
            }
        }

        #region Commands
        private ICommand closeCmd;
        public ICommand CloseCmd
        {
            get
            {
                return this.closeCmd ?? (this.closeCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true /*this.CanCloseFlyout*/,
                    ExecuteDelegate = x => ((Flyout)x).IsOpen = false
                });
            }
        }

        private ICommand showMessageDialogCommand;
        public ICommand ShowMessageDialogCommand
        {
            get
            {
                return this.showMessageDialogCommand ?? (this.showMessageDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => ShowMessage((string)x)
                });
            }
        }

        private ICommand saveSettingsCommand;
        public ICommand SaveSettingsCommand
        {
            get
            {
                return this.saveSettingsCommand ?? (this.saveSettingsCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => ShowMessage("Save Settings")
                });
            }
        }
        private ICommand restoreDefaultSettingsCommand;
        public ICommand RestoreDefaultSettingsCommand {
            get
            {
                return this.restoreDefaultSettingsCommand ?? (this.restoreDefaultSettingsCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => ShowMessage("Restore Default Settings")
                });
            }
        }

        private void ShowMessage(string message) {
            this._dialogCoordinator.ShowMessageAsync(this, "Oops!", $"{message} will be available Soon"); 
        }

        private ICommand searchDashboardCmd;
        public ICommand SearchDashboardCmd
        {
            get
            {
                return this.searchDashboardCmd ?? (this.searchDashboardCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => LoadDasboardAnalysis()
                });
            }
        }

        private ICommand openSaleMasterCmd;
        public ICommand OpenSaleMasterCmd
        {
            get
            {
                return this.openSaleMasterCmd ?? (this.openSaleMasterCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        SaleMaster contactMaster = new SaleMaster();
                        contactMaster.ShowDialog();
                    }
                });
            }
        }
        private ICommand openPurchaseMasterCmd;
        public ICommand OpenPurchaseMasterCmd
        {
            get
            {
                return this.openPurchaseMasterCmd ?? (this.openPurchaseMasterCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        PurchaseMaster contactMaster = new PurchaseMaster();
                        contactMaster.ShowDialog();
                    }
                });
            }
        }
        private ICommand openSupportMasterCmd;
        public ICommand OpenSupportMasterCmd
        {
            get
            {
                return this.openSupportMasterCmd ?? (this.openSupportMasterCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        SupportIncomeMaster contactMaster = new SupportIncomeMaster();
                        contactMaster.ShowDialog();
                    }
                });
            }
        }
        private ICommand openPendingPaymentCmd;
        public ICommand OpenPendingPaymentCmd
        {
            get
            {
                return this.openPendingPaymentCmd ?? (this.openPendingPaymentCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        PendingPayment pendingPayment = new PendingPayment();
                        pendingPayment.ShowDialog();
                    }
                });
            }
        }

        private ICommand openSalesReportCmd;
        public ICommand OpenSalesReportCmd
        {
            get
            {
                return this.openSalesReportCmd ?? (this.openSalesReportCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        SalesReport _report = new SalesReport();
                        _report.ShowDialog();
                    }
                });
            }
        }
        private ICommand openPurchaseReportCmd;
        public ICommand OpenPurchaseReportCmd
        {
            get
            {
                return this.openPurchaseReportCmd ?? (this.openPurchaseReportCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        PurchaseReport _report = new PurchaseReport();
                        _report.ShowDialog();
                    }
                });
            }
        }
        private ICommand openSupportPaymentReportCmd;
        public ICommand OpenSupportPaymentReportCmd
        {
            get
            {
                return this.openSupportPaymentReportCmd ?? (this.openSupportPaymentReportCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        SupportPaymentReport _report = new SupportPaymentReport();
                        _report.ShowDialog();
                    }
                });
            }
        }
        private ICommand openPendingPaymentReportCmd;
        public ICommand OpenPendingPaymentReportCmd
        {
            get
            {
                return this.openPendingPaymentReportCmd ?? (this.openPendingPaymentReportCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        PendingPaymentReport _report = new PendingPaymentReport();
                        _report.ShowDialog();
                    }
                });
            }
        }
        private ICommand openContactReportCmd;
        public ICommand OpenContactReportCmd
        {
            get
            {
                return this.openContactReportCmd ?? (this.openContactReportCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        ContactReport _report = new ContactReport();
                        _report.ShowDialog();
                    }
                });
            }
        }

        private ICommand openProductMasterCmd;
        public ICommand OpenProductMasterCmd
        {
            get
            {
                return this.openProductMasterCmd ?? (this.openProductMasterCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        ProductMaster _master = new ProductMaster();
                        _master.ShowDialog();
                    }
                });
            }
        }
        private ICommand openBrandMasterCmd;
        public ICommand OpenBrandMasterCmd
        {
            get
            {
                return this.openBrandMasterCmd ?? (this.openBrandMasterCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        BrandMaster _master = new BrandMaster();
                        _master.ShowDialog();
                    }
                });
            }
        }
        private ICommand openContactMasterCmd;
        public ICommand OpenContactMasterCmd
        {
            get
            {
                return this.openContactMasterCmd ?? (this.openContactMasterCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => {
                        ContactMaster contactMaster = new ContactMaster();
                        contactMaster.ShowDialog();
                    }
                });
            }
        }
        #endregion

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

        #region IncomeDetails

        //Models
        private double _totalSupportIncome, _totalSaleIncome, _totalPurchasePayment, _totalIncome, _totalSaleAmount;
        private DateTime _startDate, _endDate;
        public double TotalSupportIncome { get => _totalSupportIncome; set { _totalSupportIncome = Math.Round(value, 2); OnPropertyChanged(); } }
        public double TotalSaleIncome { get => _totalSaleIncome; set { _totalSaleIncome = Math.Round(value, 2); OnPropertyChanged(); } }
        public double TotalIncome { get => _totalIncome; set { _totalIncome = Math.Round(value, 2); OnPropertyChanged(); } }
        public double TotalPurchasePayment { get => _totalPurchasePayment; set { _totalPurchasePayment = Math.Round(value, 2); OnPropertyChanged(); } }
        public double TotalSaleAmount { get => _totalSaleAmount; set { _totalSaleAmount = Math.Round(value, 2); OnPropertyChanged(); } }
        
        public DateTime StartDate { get => _startDate; set { _startDate = value; OnPropertyChanged(); } }
        public DateTime EndDate { get => _endDate; set { _endDate = value; OnPropertyChanged();} }

        /// <summary>
        /// Get Payment In Out & Charts
        /// </summary>
        private async void LoadDasboardAnalysis()
        {
            ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", (string)Application.Current.FindResource("LoadingInfoMessage"));
            controller.SetIndeterminate();

            SaleManager _sm = new SaleManager();
            DataTable dtPaymentIncome, dtPurchaseInvest, dtSales;
            using (DataAccess da = new DataAccess()) {
                dtPaymentIncome = da.SearchPaymentIncome(StartDate.ToString(ConfigurationManager.AppSettings["DateOnly"]), EndDate.ToString(ConfigurationManager.AppSettings["DateOnly"]), null);
                dtPurchaseInvest = da.SearchPaymentInvest(StartDate.ToString(ConfigurationManager.AppSettings["DateOnly"]), EndDate.ToString(ConfigurationManager.AppSettings["DateOnly"]), PaymentStatus.PURCHASE_PAYMENT.ToString());
                dtSales = _sm.SearchSales(string.Empty, string.Empty, string.Empty, string.Empty, null, null, StartDate.ToString(ConfigurationManager.AppSettings["DateOnly"]), EndDate.ToString(ConfigurationManager.AppSettings["DateOnly"]), string.Empty);
            }
            this.TotalSupportIncome = dtPaymentIncome.AsEnumerable().Where(x => x.Field<string>("PaymentType") == PaymentStatus.SUPPORT_PAYMENT.ToString()).Select(a => a.Field<double>("Amount")).Sum();
            this.TotalSaleIncome = dtPaymentIncome.AsEnumerable().Where(x => x.Field<string>("PaymentType") == PaymentStatus.SALE_PAYMENT.ToString()).Select(a => a.Field<double>("Amount")).Sum();
            this.TotalIncome = TotalSaleIncome + TotalSupportIncome;
            this.TotalPurchasePayment = dtPurchaseInvest.AsEnumerable().Select(x => x.Field<double>("Amount") ).Sum();
            this.TotalSaleAmount = dtSales.AsEnumerable().Select(x => x.Field<double>("AmountPaid")).Sum();
            //Load Charts
            InitializeCharts();
            await controller.CloseAsync();
        }

        private void InitializeCharts()
        {
            this.Series.Clear();
            this.Series.Add(new PieSeries {
                Title = "Sale Income",
                Values = new ChartValues<double> { TotalSaleIncome },
                // PushOut = 10,
                DataLabels = true,
                LabelPoint = PointLabel
            });
            this.Series.Add(new PieSeries {
                Title = "Support Income",
                Values = new ChartValues<double> { TotalSupportIncome },
                DataLabels = true,
                LabelPoint = PointLabel
            });

            this.SeriesCollection.Clear();
            //adding series will update and animate the chart automatically
            SeriesCollection.Add(new ColumnSeries {
                Title = "Sale Income",
                Values = new ChartValues<double> { TotalSaleIncome }
            });
            this.SeriesCollection.Add(new ColumnSeries {
                Title = "Support Income",
                Values = new ChartValues<double> { TotalSupportIncome }
            });
            this.SeriesCollection.Add(new ColumnSeries {
                Title = "Total Income",
                Values = new ChartValues<double> { TotalIncome }
            });

            this.SeriesCollection.Add(new ColumnSeries {
                Title = "Total Sale",
                Values = new ChartValues<double> { TotalSaleAmount }
            });
            this.SeriesCollection.Add(new ColumnSeries {
                Title = "Total Purchase",
                Values = new ChartValues<double> { TotalPurchasePayment }
            });

            //Labels = new[] { "Maria", "Susan", "Charles", "Frida" };
            //Formatter = value => value.ToString("N");
            //DataContext = this;
        }
        #endregion

        #region Stock Region
        /// <summary>
        /// Load Stock Analys
        /// </summary>
        private async void LoadStockAnalysis()
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", (string)Application.Current.FindResource("LoadingInfoMessage"));
            controller.SetIndeterminate();

            PurchaseManager _pm = new PurchaseManager();
            this.PurchaseOUtofStockList.Clear();
            DataTable dt = _pm.GetAllOutOfStocks();
            dt = CommonMethods.SortTable(dt, "CreatedDate", true);
            foreach (DataRow row in dt.Rows) {
                PurchaseOUtofStockList.Add(new Model.Purchase()
                {
                    Id = int.Parse(row["StockId"].ToString()),
                    Product = Convert.ToString(row["Product"]),
                    ProductId = int.Parse(row["ProductId"].ToString()),
                    ProductCode = (string)row["ProductCode"],
                    StockCode = Convert.ToString(row["StockCode"]),
                    //ItemDesc = Convert.ToString(row["ItemDesc"]),

                    Quantity = int.Parse(row["Quantity"].ToString()),
                    AvlQuantity = int.Parse(row["AvlQuantity"].ToString()),

                    PurchaseDate = Convert.ToDateTime(row["PurchaseDate"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }
            await controller.CloseAsync();
        }
        #endregion

        #region Sale Region
        /// <summary>
        /// Load Sale Section
        /// </summary>
        private async void LoadSaleAnalysis()
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", (string)Application.Current.FindResource("LoadingInfoMessage"));
            controller.SetIndeterminate();

            SaleManager _sm = new SaleManager();
            this.SaleOnDemandList.Clear();
            foreach (DataRow row in _sm.GetSalesOnDemand().Rows) {
                SaleOnDemandList.Add(new Model.Sale()
                {
                    Id = int.Parse(row["SalesId"].ToString()),
                    Product = Convert.ToString(row["Product"]),
                    ProductCode = (string)row["ProductCode"],
                    SaleCount = int.Parse(row["SaleCount"].ToString())
                });
            }
            await controller.CloseAsync();
        }
        #endregion
    }
}
