using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility;
using ElectronicZone.Wpf.View.Sale;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ElectronicZone.Wpf.ViewModel
{
    public class SaleViewModel : ViewModelBase
    {
        #region Properties
        ILogger logger = new Logger(typeof(SaleViewModel));
        private readonly IDialogCoordinator _dialogCoordinator;
        public ObservableCollection<Sale> SaleList { get; set; }
        public ObservableCollection<Purchase> PurchaseList { get; set; }
        public ObservableCollection<Product> ProductList { get; set; }
        public ObservableCollection<Brand> BrandList { get; set; }
        SaleManager _sm = null;
        //public ObservableCollection<Contact> ContactList { get; set; }
        #endregion

        #region Commands
        public ICommand SearchPurchaseResetCmd { get; set; }
        public ICommand SearchPurchaseOrderCmd { get; set; }
        public ICommand CancelSaleOrderCmd { get; set; }
        public ICommand SaleThisPurchaseOrderCmd { get; set; }
        public ICommand DownloadSaleInvoiceCmd { get; set; }
        #endregion

        #region UI Models
        private string _productCode;
        private string _stockCode;
        private double _priceFrom;
        private double _priceTo;

        public string ProductCode { get => _productCode; set { _productCode = value; OnPropertyChanged(); } }
        public string StockCode { get => _stockCode; set { _stockCode = value; OnPropertyChanged(); } }
        public double PriceFrom { get => _priceFrom; set { _priceFrom = value; OnPropertyChanged(); } }
        public double PriceTo { get => _priceTo; set { _priceTo = value; OnPropertyChanged(); } }

        //Add Sale Properties
        //private int _selectedStockId;
        //private string _selectedProduct;
        //private string _selectedProductCode;
        //private string _selectedStockCode;
        //private double _selectedProductPrice;
        //private int _selectedProductAvlQuantity;
        //// private int _selePersonId;
        //private string _selePersonName;
        //private string _selePersonContact;
        //private int _seleQuantity;
        //private double _seleTotalAmount;
        //private double _seleAmountPaid;
        //private DateTime _seleDate;
        //private bool _selectedContactType;//New|Existing

        //public int SelectedStockId { get => _selectedStockId; set { _selectedStockId = value; OnPropertyChanged(); } }
        //public string SelectedProduct { get => _selectedProduct; set { _selectedProduct = value; OnPropertyChanged(); } }
        //public string SelectedProductCode { get => _selectedProductCode; set { _selectedProductCode = value; OnPropertyChanged(); } }
        //public string SelectedStockCode { get => _selectedStockCode; set { _selectedStockCode = value; OnPropertyChanged(); } }
        //public double SelectedProductPrice { get => _selectedProductPrice; set { _selectedProductPrice = value; OnPropertyChanged(); } }
        //public int SelectedProductAvlQuantity { get => _selectedProductAvlQuantity; set { _selectedProductAvlQuantity = value; OnPropertyChanged(); } }
        //public string SelePersonName { get => _selePersonName; set { _selePersonName = value; OnPropertyChanged(); } }
        //public string SelePersonContact { get => _selePersonContact; set { _selePersonContact = value; OnPropertyChanged(); } }
        //public int SeleQuantity { get => _seleQuantity; set { _seleQuantity = value; OnPropertyChanged(); CalculateTotalSaleAmount(); } }
        //public double SeleTotalAmount { get => _seleTotalAmount; set { _seleTotalAmount = value; OnPropertyChanged(); } }
        //public double SeleAmountPaid { get => _seleAmountPaid; set { _seleAmountPaid = value; OnPropertyChanged(); } }
        //public DateTime SeleDate { get => _seleDate; set { _seleDate = value; OnPropertyChanged(); } }
        //public bool IsSaleToExistingContact { get => _selectedContactType; set { _selectedContactType = value; OnPropertyChanged(); OnSaleContactTypeChanged(); } }

        //private void OnSaleContactTypeChanged()
        //{
        //    if (!IsSaleToExistingContact) {
        //        this.SelePersonName = "";
        //        this.SelePersonContact = "";
        //    }
        //}

        private DateTime _todayDate;
        public DateTime TodayDate { get => _todayDate; set => _todayDate = value; }
        private int _selectedIndex;
        public int TabSelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                if (_selectedIndex == 1) { GetAllSalesAsync(); } /*else { loadProduct(); loadBrands(); }*/;
            }
        }
        private Product _sProduct;
        public Product SProduct
        {
            get { return _sProduct; }
            set { _sProduct = value; OnPropertyChanged(); }
        }
        private Brand _sBrand;
        public Brand SBrand
        {
            get { return _sBrand; }
            set { _sBrand = value; OnPropertyChanged(); }
        }
        //private Contact _sContact;
        //public Contact SContact
        //{
        //    get { return _sContact; }
        //    set { _sContact = value; OnPropertyChanged(); OnContactSelected(); }
        //}

        //private void OnContactSelected()
        //{
        //    if(SContact!= null)
        //    {
        //        this.SelePersonName = SContact.Name;
        //        this.SelePersonContact = SContact.PrimaryContact;
        //    }
        //}

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instance"></param>
        public SaleViewModel(IDialogCoordinator instance)
        {
            try {
                this.TabSelectedIndex = 0;
                this._dialogCoordinator = instance;
                //this.PurchaseDate = DateTime.Now;
                //this.TabHeaderText = "Add Purchase";
                this.TodayDate = DateTime.Today;
                //this.SelectedProduct = "asdasd";
                //this.IsSaleToExistingContact = true;
                this._sm = new SaleManager();

                this.SaleList = new ObservableCollection<Sale>();
                this.PurchaseList = new ObservableCollection<Purchase>();
                this.ProductList = new ObservableCollection<Product>();
                this.BrandList = new ObservableCollection<Brand>();
                //this.ContactList = new ObservableCollection<Contact>();

                this.CancelSaleOrderCmd = new CommandHandler(CancelSaleOrder, CanExecuteCancelSaleOrder);
                this.SearchPurchaseResetCmd = new CommandHandler(SearchPurchaseReset, CanExecuteSearchPurchaseReset);
                this.SearchPurchaseOrderCmd = new CommandHandler(SearchPurchaseOrder, CanExecuteSearchPurchaseOrder);
                this.SaleThisPurchaseOrderCmd = new CommandHandler(SaleThisPurchaseOrder, CanExecuteSaleThisPurchaseOrder);
                this.DownloadSaleInvoiceCmd = new CommandHandler(DownloadSaleInvoice, CanExecuteCancelSaleOrder);

                LoadProduct(); LoadBrands();

                //this.AddSaleOrderCmd = new CommandHandler(AddSaleOrder, CanExecuteAddSaleOrder);
                //this.CloseAddSaleOrderCmd = new CommandHandler(CloseAddSaleOrder, CanExecuteCloseAddSaleOrder);
                //LoadSalesPerson();
            }
            catch (Exception ex) {
                logger.LogException(ex);
            }
        }

        private async void DownloadSaleInvoice(object obj)
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", "Please wait for a while...");
            controller.SetIndeterminate();
            DownloadSaleInvoice((Sale)obj);
            await controller.CloseAsync();
        }

        private async void GetAllSalesAsync()
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", "Please wait for a while...");
            controller.SetIndeterminate();

            //await System.Threading.Tasks.Task.Delay(1000);
            GetAllSales();

            await controller.CloseAsync();
        }
        private void GetAllSales()
        {
            this.SaleList.Clear();
            foreach (DataRow row in _sm.SearchSales(string.Empty, string.Empty, string.Empty, string.Empty,null,null, string.Empty, string.Empty, string.Empty).Rows)
            {
                this.SaleList.Add(new Sale()
                {
                    Id = int.Parse(row["SalesId"].ToString()),
                    StockId = int.Parse(row["StockId"].ToString()),
                    Product = Convert.ToString(row["Product"]),
                    Brand = Convert.ToString(row["Brand"]),
                    ProductCode = Convert.ToString(row["ProductCode"]),
                    StockCode = Convert.ToString(row["StockCode"]),
                    SaleTo = Convert.ToString(row["SaleTo"]),
                    SaleContact = Convert.ToString(row["SaleContact"]),
                    AmountPaid = Convert.ToDouble(row["AmountPaid"]),
                    Quantity = int.Parse(row["Quantity"].ToString()),
                    Price = Convert.ToDouble(row["SalePrice"]),
                    Total = Convert.ToDouble(row["Total"]),
                    Pending = Convert.ToDouble(row["Pending"]),
                    SaleDate = Convert.ToDateTime(row["SaleDate"]),
                    //IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    CanCancel = Convert.ToBoolean(row["CanDelete"]),
                    //ModifiedDate = Convert.ToDateTime(row["ModifiedDate"])
                });
            }
        }

        /// <summary>
        /// Download Invoice For Sale
        /// </summary>
        /// <param name="_sale"></param>
        private void DownloadSaleInvoice(Sale _sale)
        {
            try
            {
                goPDFOut invoice = new goPDFOut(new int[] { _sale.Id });
                invoice.GeneratePDF();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        //private void CloseAddSaleOrder(object obj)
        //{
        //    AddSale window = (AddSale)obj;
        //    window.Close();
        //}


        //private Boolean ValidateSaleForm()
        //{
        //    bool isFormValid = true;
        //    //double total = this.SeleQuantity * this.SelectedProductPrice;
        //    if(this.SelectedStockId <= 0)
        //        isFormValid = false;
        //    if (this.SeleAmountPaid <= 0)
        //        isFormValid = false;
        //    //if (string.IsNullOrEmpty(SeleDate))
        //    //    isFormValid = false;
        //    if (this.SeleQuantity > this.SelectedProductAvlQuantity)
        //        isFormValid = false;
        //    //if (this.SeleAmountPaid <= 0 && double.Parse(txtAmtPaid.Text) > total)
        //    //    isFormValid = false;
        //    return isFormValid;
        //}
        //private void AddSaleOrder(object obj)
        //{
        //    // Validate Sale
        //    if (ValidateSaleForm()) {
        //        Sale saleOrder = new Sale() {
        //            StockId = this.SelectedStockId,
        //            Quantity = this.SeleQuantity,
        //            Price = this.SelectedProductPrice,
        //            Total = this.SeleTotalAmount,
        //            AmountPaid = this.SeleAmountPaid,
        //            SaleDate = this.SeleDate,

        //            Contact = { Id = SContact.Id, Name = this.SelePersonName, PrimaryContact = this.SelePersonContact }
        //        };
        //        SaleManager sm = new SaleManager();
        //        sm.CreateSalesOrder(saleOrder);
        //        // Create Sale
        //        AddSale window = (AddSale)obj;
        //        window.Close();
        //    }
        //    else
        //    {
        //        MessageBoxResult result = MessageBox.Show("Invalid Data !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        //private bool CanExecuteCloseAddSaleOrder(object arg)
        //{
        //    return true;
        //}

        //private bool CanExecuteAddSaleOrder(object arg)
        //{
        //    return true;
        //}

        private void SaleThisPurchaseOrder(object obj)
        {
            var item = (Purchase)obj;
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    // stockId
            //    this.SelectedStockId = item.Id;
            //    this.SelectedProduct = item.Product;
            //    this.SelectedProductCode = item.ProductCode;
            //    this.SelectedStockCode = item.StockCode;
            //    this.SelectedProductPrice = item.SalePrice;
            //    this.SelectedProductAvlQuantity = item.AvlQuantity;
            //});
            //LoadSalesPerson();

            //open modal for sale Item
            AddSale saleScreen = new AddSale(item);
            saleScreen.ShowDialog();
            //refresh sale data
            SearchPurchaseOrder(new object());
        }

        private bool CanExecuteSaleThisPurchaseOrder(object arg)
        {
            return true;
        }

        private async void SearchPurchaseOrder(object obj)
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", "Please wait for a while...");
            controller.SetIndeterminate();

            //await System.Threading.Tasks.Task.Delay(1000);
            DataTable dt = new DataTable();
            using (DataAccess da = new DataAccess())
            {
                dt = da.SearchStocks(this.SProduct != null ? this.SProduct.Id.ToString() : "", this.SBrand != null ? this.SBrand.Id.ToString() : ""
                    , this.ProductCode, this.StockCode, this.PriceFrom == 0 ? (double?)null : this.PriceFrom, this.PriceTo == 0 ? (double?)null : this.PriceTo, string.Empty, string.Empty, true);
            }

            this.PurchaseList.Clear();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    this.PurchaseList.Add(new Purchase()
                    {
                        Id = int.Parse(row["StockId"].ToString()),
                        Product = Convert.ToString(row["Product"]),
                        Brand = Convert.ToString(row["Brand"]),
                        ProductCode = Convert.ToString(row["ProductCode"]),
                        StockCode = Convert.ToString(row["StockCode"]),
                        AvlQuantity = int.Parse(row["AvlQuantity"].ToString()),
                        Quantity = int.Parse(row["Quantity"].ToString()),
                        SalePrice = Convert.ToDouble(row["SalePrice"]),
                        PurchasePrice = Convert.ToDouble(row["PurchasePrice"]),
                        PurchaseDate = Convert.ToDateTime(row["PurchaseDate"])
                    });
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("No Results Found!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            await controller.CloseAsync();
        }

        private void SearchPurchaseReset(object obj)
        {
            ResetPurchaseSearchForm();
        }

        private void ResetPurchaseSearchForm()
        {
            try {
                this.SProduct = null;
                this.SBrand = null;
                this.ProductCode = null;
                this.StockCode = null;
                this.PriceFrom = 0;
                this.PriceTo = 0;
            }
            catch (Exception ex) {
                logger.LogException(ex);
            }
        }

        private bool CanExecuteSearchPurchaseOrder(object arg)
        {
            return true;
        }

        private bool CanExecuteSearchPurchaseReset(object arg)
        {
            return true;
        }

        private bool CanExecuteCancelSaleOrder(object arg)
        {
            return true;
        }

        /// <summary>
        /// Cancel Sale Order
        /// </summary>
        /// <param name="obj"></param>
        private void CancelSaleOrder(object obj)
        {
            var item = (Sale)obj;
            // Checking if any of the item has not sell 
            if (item.CanCancel && item.Pending == 0) {
                if (MessageBox.Show($"Are you sure want to cancel order with amount {item.Total}?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes) {
                    SaleManager sm = new SaleManager();
                    bool isCanceled = sm.ReverseSalesOrder(item);
                    if (isCanceled)
                    {
                        MessageBoxResult result = MessageBox.Show("Sale Canceled Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    this.SaleList.Remove(item);
                }
            }
            else { MessageBoxResult result = MessageBox.Show("Sale Cannot be Canceled! You might be exceed the date limit or its a pending sale.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        /// <summary>
        /// Load All Active Products
        /// </summary>
        private void LoadProduct()
        {
            DataTable dtProduct = new DataTable();
            using (DataAccess da = new DataAccess()) {
                dtProduct = da.GetAllProducts();
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (DataRow row in dtProduct.Rows)
                {
                    this.ProductList.Add(new Product()
                    {
                        Id = int.Parse(row["Id"].ToString()),
                        Name = Convert.ToString(row["Name"]),
                        //Description = Convert.ToString(row["Description"])
                        //CreatedDate = Convert.ToDateTime(row["CreatedDate"].ToString()),
                    });
                }
            });
        }

        /// <summary>
        /// Load All Active Brands
        /// </summary>
        private void LoadBrands()
        {
            // DataTable dtBrand = new DataTable();
            using (DataAccess da = new DataAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (DataRow row in da.GetAllBrands().Rows)
                    {
                        this.BrandList.Add(new Brand()
                        {
                            Id = int.Parse(row["Id"].ToString()),
                            Name = Convert.ToString(row["Name"]),
                            //Description = Convert.ToString(row["Description"]),
                            //CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        });
                    }
                });
            }
        }

        //private void LoadSalesPerson()
        //{
        //    DataTable dtProduct = new DataTable();
        //    using (DataAccess da = new DataAccess())
        //    {
        //        dtProduct = da.GetAllSalesPerson();
        //    }

        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        foreach (DataRow row in dtProduct.Rows)
        //        {
        //            this.ContactList.Add(new Contact()
        //            {
        //                Id = int.Parse(row["Id"].ToString()),
        //                Name = Convert.ToString(row["Name"]),
        //                PrimaryContact = Convert.ToString(row["Contact"])
        //            });
        //        }
        //    });
        //}

        //private void CalculateTotalSaleAmount()
        //{
        //    this.SeleTotalAmount = SeleQuantity * SelectedProductPrice;
        //}

        //private ICommand showCustomDialogCommand;

        //public ICommand ShowCustomDialogCommand
        //{
        //    get
        //    {
        //        return this.showCustomDialogCommand ?? (this.showCustomDialogCommand = new SimpleCommand
        //        {
        //            CanExecuteDelegate = x => true,
        //            ExecuteDelegate = x => RunCustomFromVm()
        //        });
        //    }
        //}

        //private async void RunCustomFromVm()
        //{
        //    var customDialog = new CustomDialog() { Title = "Custom Dialog" };

        //    var dataContext = new CustomDialogExampleContent(instance =>
        //    {
        //        _dialogCoordinator.HideMetroDialogAsync(this, customDialog);

        //        //Add Sale
        //        System.Diagnostics.Debug.WriteLine(instance.SaleDate);
        //    });
        //    customDialog.Content = new AddSaleCustomDialog { DataContext = dataContext };

        //    await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        //}
    }
}
