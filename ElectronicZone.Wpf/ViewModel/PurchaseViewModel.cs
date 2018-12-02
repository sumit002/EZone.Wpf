using ElectronicZone.Wpf.DataAccessLayer;
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
using System.IO;
using System.Windows;
using System.Windows.Input;
using static ElectronicZone.Wpf.Utility.CommonEnum;

namespace ElectronicZone.Wpf.ViewModel
{
    public class PurchaseViewModel : ViewModelBase
    {
        #region Properties
        ILogger logger = new Logger(typeof(PurchaseViewModel));
        private IDialogCoordinator _dialogCoordinator;
        public ObservableCollection<Purchase> PurchaseList { get; set; }
        public ObservableCollection<Product> ProductList { get; set; }
        public ObservableCollection<Brand> BrandList { get; set; }
        public PurchaseManager _pm;
        string imageName = ""; 
        #endregion

        #region  UI Models
        private int _id;
        private int _productId;
        private int _brandId;
        private string _productCode;
        private string _stockCode;
        private string _itemDescription;
        private int _quantity;
        private int _avlQuantity;
        private double _purchasePrice;
        private double _salePrice;
        private byte[] _productImage;
        private bool _isActive;
        private DateTime _purchaseDate;
        private DateTime _createdDate;
        private DateTime _modifiedDate;
        private double _totalPurchasePrice;
        private string _tabHeaderText;
        private bool _isAddMode;
        private double purchaseProfitPercent;
        private bool _profitByPercent;
        private DateTime _todayDate;
        //private bool _isAddMoreQtyMode;
        //private bool _isQuantityEnabled;
        private bool _isSalePriceEnabled;
        //private int _oldQuantity;

        public int Id { get => _id; set { _id = value; } }
        public int ProductId { get => _productId; set { _productId = value; OnPropertyChanged(); } }
        public int BrandId { get => _brandId; set { _brandId = value; OnPropertyChanged(); } }
        public string ProductCode { get => _productCode; set { _productCode = value; OnPropertyChanged(); } }
        public string StockCode { get { return _stockCode; } set { _stockCode = value; OnPropertyChanged(); } }
        public string ItemDesc { get => _itemDescription; set { _itemDescription = value; OnPropertyChanged(); }  }
        public int Quantity { get => _quantity; set { _quantity = value; OnPropertyChanged(); CalculateTotalPurchasePrice(); } }
        public int AvlQuantity { get => _avlQuantity; set => _avlQuantity = value; }
        public double PurchasePrice { get => _purchasePrice; set { _purchasePrice = value; OnPropertyChanged(); CalculateTotalPurchasePrice(); CalculateTotalSellingPrice(); } }
        public double SalePrice { get => _salePrice; set { _salePrice = value; OnPropertyChanged(); ValidateSalesPrice(); } }
        public byte[] ProductImage { get => _productImage; set => _productImage = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public DateTime PurchaseDate { get => _purchaseDate; set { _purchaseDate = value; OnPropertyChanged(); } }
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }
        public DateTime ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }
        public double TotalPurchasePrice { get => _totalPurchasePrice; set { _totalPurchasePrice = value; OnPropertyChanged(); } }
        public string TabHeaderText { get => _tabHeaderText; set { _tabHeaderText = value; OnPropertyChanged(); } }
        public bool IsAddMode { get => _isAddMode; set { _isAddMode = value;
                //if(_isAddMode)
                OnPropertyChanged(); } }
        public double PurchaseProfitPercent { get => purchaseProfitPercent; set { purchaseProfitPercent = value; OnPropertyChanged(); CalculateTotalSellingPrice(); } }
        public bool ProfitByPercent { get => _profitByPercent; set { _profitByPercent = value; OnPropertyChanged(); } }
        public DateTime TodayDate { get => _todayDate; set => _todayDate = value; }
        //public bool IsAddMoreQtyMode { get => _isAddMoreQtyMode; set { _isAddMoreQtyMode = value; OnPropertyChanged(); } }
        //public bool IsQuantityEnabled { get => _isQuantityEnabled; set { _isQuantityEnabled = value; OnPropertyChanged(); } }
        public bool IsSalePriceEnabled { get => _isSalePriceEnabled; set { _isSalePriceEnabled = value; OnPropertyChanged(); } }
        //public int OldQuantity { get => _oldQuantity; set { _oldQuantity = value; OnPropertyChanged(); } }


        private Product _sProduct;
        public Product SProduct
        {
            get { return _sProduct; }
            set { _sProduct = value; OnPropertyChanged(); ProductId = _sProduct.Id; }
        }
        private Brand _sBrand;
        public Brand SBrand
        {
            get { return _sBrand; }
            set { _sBrand = value; OnPropertyChanged(); BrandId = _sBrand.Id; }
        }

        private int _selectedIndex;
        public int TabSelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                if (_selectedIndex == 1) { GetAllPurchases(); } /*else { loadProduct(); loadBrands(); }*/;
            }
        }
        #endregion

        #region Commands
        public ICommand AddPurchaseCommand { get; set; }
        public ICommand PurchaseResetCommand { get; set; }

        public ICommand EditPurchaseCmd { get; set; }
        public ICommand DeletePurchaseCmd { get; set; }
        public ICommand AddMorePurchaseQuantityCmd { get; set; }
        #endregion

        /// <summary>
        /// PurchaseViewModel Constructor
        /// </summary>
        public PurchaseViewModel(IDialogCoordinator instance)
        {
            try {
                this.TabSelectedIndex = 0;
                this._dialogCoordinator = instance;
                this.PurchaseDate = DateTime.Now;
                this.TabHeaderText = "Add Purchase";
                this.IsAddMode = this.ProfitByPercent /*= IsQuantityEnabled */= IsSalePriceEnabled = true;
                //this.IsAddMoreQtyMode = false;
                this.TodayDate = DateTime.Today;
                this.PurchaseProfitPercent = Convert.ToDouble(ConfigurationManager.AppSettings["PurchaseProfitPercent"]);

                this._pm = new PurchaseManager();
                this.PurchaseList = new ObservableCollection<Purchase>();
                this.ProductList = new ObservableCollection<Product>();
                this.BrandList = new ObservableCollection<Brand>();

                this.AddPurchaseCommand = new CommandHandler(AddPurchaseStock, CanExecuteAddPurchaseStock);
                this.PurchaseResetCommand = new CommandHandler(PurchaseReset, CanExecutePurchaseReset);

                this.EditPurchaseCmd = new CommandHandler(EditPurchase, CanExecuteEditPurchase);
                this.DeletePurchaseCmd = new CommandHandler(DeletePurchase, CanExecuteDeletePurchase);
                this.AddMorePurchaseQuantityCmd = new CommandHandler(AddMorePurchaseQuantity, CanExecuteAddPurchaseType);

                LoadProduct(); LoadBrands();
            }
            catch (Exception ex) {
                logger.LogException(ex);
            }
        }

        #region Methods
        /// <summary>
        /// Reset Purchase Form
        /// </summary>
        /// <param name="obj"></param>
        private void PurchaseReset(object obj)
        {
            try
            {
                this.Id = 0;
                this.ProductId = 0;
                this.BrandId = 0;
                this.ProductCode = "";
                this.StockCode = "";
                this.ItemDesc = "";
                this.PurchasePrice = 0;
                this.Quantity = 0;
                this.SalePrice = 0;
                this.PurchaseDate = DateTime.Now;
                this.SBrand = null;
                this.SProduct = null;

                this.TabHeaderText = "Add Purchase";
                this.IsAddMode = true;
                /*this.IsQuantityEnabled =*/ IsSalePriceEnabled = true;
                //this.IsAddMoreQtyMode = false;
            }
            catch (Exception ex) { logger.LogException(ex); }
        }

        /// <summary>
        /// Validate Purchase Order
        /// </summary>
        /// <returns></returns>
        private bool ValidatePurchaseForm()
        {
            if ((this.ProductId <= 0))
                return false;
            else if (this.BrandId <= 0)
                return false;
            else if (string.IsNullOrEmpty(this.ProductCode))
                return false;
            else if (this.PurchasePrice <= 0)
                return false;
            else if (this.Quantity <= 0)
                return false;
            else if (this.SalePrice <= 0)
                return false;
            else if (this.PurchasePrice <= 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Add Purchase/Stock Order
        /// </summary>
        /// <param name="obj"></param>
        private async void AddPurchaseStock(object obj)
        { // byte[] imgByteArr = null;
            if (ValidatePurchaseForm()) {
                using (DataAccess da = new DataAccess()) {
                    try {
                        var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", "Please wait for a while...");
                        controller.SetIndeterminate();

                        Purchase order = new Purchase() {
                            Id = this.Id,
                            ProductId = this.ProductId,
                            BrandId = this.BrandId,
                            ProductCode = this.ProductCode,
                            StockCode = this.StockCode,
                            ItemDesc = this.ItemDesc,
                            Quantity = this.Quantity,
                            AvlQuantity = (IsAddMode == true ? this.Quantity : this.AvlQuantity),
                            PurchasePrice = this.PurchasePrice,
                            SalePrice = this.SalePrice,
                            PurchaseDate = this.PurchaseDate,
                            //IsAddMoreQtyMode = this.IsAddMoreQtyMode,
                            //OldQuantity = this.OldQuantity,//Old Qty to Update
                        };

                        //PurchaseManager pm = new PurchaseManager();
                        int id = _pm.CreateOrUpdatePurchaseOrder(order);
                        if(id > 0)
                            PurchaseReset(new object());
                        await controller.CloseAsync();
                    }
                    catch (Exception ex) {
                        logger.LogException(ex);
                        da.RollbackTransaction();
                    }
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Invalid Data ! Please check the fields entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPurchase(object obj)
        {
            var item = (Purchase)obj;
            //if (item.Quantity != item.AvlQuantity)
            //    MessageBoxResult result = MessageBox.Show("Since Some of the Items are sold, So dont change the sale price!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            // Code Price Qty
            AssignPurchaseModelProperties(item);
            this.TabHeaderText = "Edit Purchase";
            this.IsAddMode /*= this.IsAddMoreQtyMode = IsQuantityEnabled*/ = ProfitByPercent = false;
            // Edit Sale Price if not sold any of the item [i.e. AvlQty=Qty]
            this.IsSalePriceEnabled = (item.Quantity == item.AvlQuantity ? true : false);
            this.TabSelectedIndex = 0;
        }

        /// <summary>
        /// Assign Purchase Model Properties
        /// </summary>
        /// <param name="purchase"></param>
        private void AssignPurchaseModelProperties(Purchase purchase)
        {
            this.Id = purchase.Id;
            this.ProductId = purchase.ProductId;
            this.BrandId = purchase.BrandId;
            this.ProductCode = purchase.ProductCode;
            this.StockCode = purchase.StockCode;
            this.ItemDesc = purchase.ItemDesc;
            this.PurchasePrice = purchase.PurchasePrice;
            this.Quantity = purchase.Quantity;
            this.AvlQuantity = purchase.AvlQuantity;
            this.SalePrice = purchase.SalePrice;
            this.PurchaseDate = purchase.PurchaseDate;

            //IEnumerable<Brand> obsCollection = (IEnumerable<Brand>)BrandList;
            //var asd = (IEnumerable<Brand>)BrandList;

            var bList = new List<Brand>((IEnumerable<Brand>)BrandList);
            var pList = new List<Product>((IEnumerable<Product>)ProductList);
            
            SBrand = bList.Find(x => x.Id == purchase.BrandId);
            SProduct = pList.Find(x => x.Id == purchase.ProductId);
        }

        /// <summary>
        /// Add More Quantity to Purchase Order
        /// </summary>
        /// <param name="obj"></param>
        private void AddMorePurchaseQuantity(object obj)
        {
            var item = (Purchase)obj;
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.ProductCode = item.ProductCode;
                this.StockCode = item.StockCode;
                this.ItemDesc = item.ItemDesc;
                this.PurchasePrice = item.PurchasePrice;
                //this.OldQuantity = item.Quantity;
                //this.AvlQuantity = item.AvlQuantity;
                this.Quantity = 0;
                this.SalePrice = item.SalePrice;
                this.PurchaseDate = DateTime.Now;

                var bList = new List<Brand>((IEnumerable<Brand>)BrandList);
                var pList = new List<Product>((IEnumerable<Product>)ProductList);
                this.SBrand = bList.Find(x => x.Id == item.BrandId);
                this.SProduct = pList.Find(x => x.Id == item.ProductId);

                this.IsAddMode = ProfitByPercent = true;
            });
            this.TabHeaderText = "Add Purchase";
            // this.IsAddMoreQtyMode = IsQuantityEnabled = true;
            this.IsSalePriceEnabled = true;
            this.TabSelectedIndex = 0;
        }

        /// <summary>
        /// Delete Purchase/Stock
        /// </summary>
        /// <param name="obj"></param>
        private void DeletePurchase(object obj)
        {
            var item = (Purchase)obj;
            // Checking if any of the item has not sell 
            if (item.Quantity == item.AvlQuantity) {
                if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    //PurchaseManager pm = new PurchaseManager();
                    _pm.DeletePurchaseOrder(item);
                    PurchaseList.Remove(item);
                }
            }
            else { MessageBoxResult result = MessageBox.Show("Stock Cannot be deleted!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private bool CanExecuteEditPurchase(object arg)
        {
            return true;
        }

        private bool CanExecuteAddPurchaseType(object arg)
        {
            return true;
        }

        private bool CanExecuteDeletePurchase(object arg)
        {
            return true;
        }

        private bool CanExecuteAddPurchaseStock(object arg)
        {
            if (string.IsNullOrEmpty(this.ProductCode.Trim()))
                return false;
            else if (string.IsNullOrEmpty(this.StockCode.Trim()))
                return false;
            else
                return true;
        }

        private bool CanExecutePurchaseReset(object arg)
        {
            return true;
        }

        /// <summary>
        /// Get All Purchases/Stocks Async
        /// </summary>
        private async void GetAllPurchases()
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", "Please wait for a while...");
            controller.SetIndeterminate();

            this.PurchaseList.Clear();
            foreach (DataRow row in _pm.GetAllStocks().Rows)
            {
                PurchaseList.Add(new Purchase()
                {
                    Id = int.Parse(row["StockId"].ToString()),
                    Product = Convert.ToString(row["Product"]),
                    ProductId = int.Parse(row["ProductId"].ToString()),
                    Brand = Convert.ToString(row["Brand"]),
                    BrandId = int.Parse(row["BrandId"].ToString()),
                    ProductCode = (string)row["ProductCode"],
                    StockCode = Convert.ToString(row["StockCode"]),
                    ItemDesc = Convert.ToString(row["ItemDesc"]),

                    Quantity = int.Parse(row["Quantity"].ToString()),
                    AvlQuantity = int.Parse(row["AvlQuantity"].ToString()),

                    PurchasePrice = Convert.ToDouble(row["PurchasePrice"]),
                    SalePrice = Convert.ToDouble(row["SalePrice"]),
                    TotalPurchasePrice = Convert.ToDouble(row["Total"]),

                    PurchaseDate = Convert.ToDateTime(row["PurchaseDate"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }

            await controller.CloseAsync();
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
            using (DataAccess da = new DataAccess()) {
                // dtBrand = da.GetAllBrands();
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

        private void CalculateTotalPurchasePrice()
        {
            // double total = 0;
            //if (!string.IsNullOrEmpty(qty) && !string.IsNullOrEmpty(price))
            //    total = double.Parse(qty) * double.Parse(price);
            this.TotalPurchasePrice = this.Quantity * this.PurchasePrice;
        }

        private void CalculateTotalSellingPrice()
        {
            if(ProfitByPercent)
                this.SalePrice = this.PurchasePrice + (this.PurchasePrice * this.PurchaseProfitPercent)/100;
        }

        private void ValidateSalesPrice() {
            if (this.SalePrice > 0 && this.SalePrice < this.PurchasePrice)
            {
                MessageBoxResult result = MessageBox.Show("Sale Price should be grater than or equal to purchase price!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.SalePrice = this.PurchasePrice;
            }
        }

        #endregion
    }
}
