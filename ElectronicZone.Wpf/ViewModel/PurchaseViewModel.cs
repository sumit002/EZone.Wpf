using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility;
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
        private IDialogCoordinator dialogCoordinator;
        public ObservableCollection<Purchase> PurchaseList { get; set; }
        public ObservableCollection<Product> ProductList { get; set; }
        public ObservableCollection<Brand> BrandList { get; set; }
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
        public bool IsAddMode { get => _isAddMode; set { _isAddMode = value; OnPropertyChanged(); } }
        public double PurchaseProfitPercent { get => purchaseProfitPercent; set { purchaseProfitPercent = value; OnPropertyChanged(); CalculateTotalSellingPrice(); } }
        public bool ProfitByPercent { get => _profitByPercent; set { _profitByPercent = value; OnPropertyChanged(); } }
        public DateTime TodayDate { get => _todayDate; set => _todayDate = value; }


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
        public ICommand AddPurchaseTypeCmd { get; set; }
        #endregion

        /// <summary>
        /// PurchaseViewModel Constructor
        /// </summary>
        public PurchaseViewModel(IDialogCoordinator instance)
        {
            try {
                this.TabSelectedIndex = 0;
                this.dialogCoordinator = instance;
                this.PurchaseDate = DateTime.Now;
                this.TabHeaderText = "Add Purchase";
                this.IsAddMode = this.ProfitByPercent = true;
                this.TodayDate = DateTime.Today;
                this.PurchaseProfitPercent = Convert.ToDouble(ConfigurationManager.AppSettings["PurchaseProfitPercent"]);

                this.PurchaseList = new ObservableCollection<Purchase>();
                this.ProductList = new ObservableCollection<Product>();
                this.BrandList = new ObservableCollection<Brand>();

                this.AddPurchaseCommand = new CommandHandler(AddPurchaseStock, CanExecuteAddPurchaseStock);
                this.PurchaseResetCommand = new CommandHandler(PurchaseReset, CanExecutePurchaseReset);

                this.EditPurchaseCmd = new CommandHandler(EditPurchase, CanExecuteEditPurchase);
                this.DeletePurchaseCmd = new CommandHandler(DeletePurchase, CanExecuteDeletePurchase);
                this.AddPurchaseTypeCmd = new CommandHandler(AddPurchaseType, CanExecuteAddPurchaseType);

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
            }
            catch (Exception ex) { logger.LogException(ex); }
        }

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
        private void AddPurchaseStock(object obj)
        {
            byte[] imgByteArr = null;
            if (ValidatePurchaseForm()) {
                using (DataAccess da = new DataAccess())
                {
                    try
                    {
                        //create stock record
                        System.Collections.Generic.Dictionary<string, string> folderFields = new System.Collections.Generic.Dictionary<string, string>();
                        folderFields.Add("Id", this.Id == 0 ? null : this.Id.ToString());
                        folderFields.Add("ProductId", this.ProductId.ToString());
                        folderFields.Add("BrandId", this.BrandId.ToString());
                        folderFields.Add("ProductCode", this.ProductCode);
                        folderFields.Add("StockCode", this.StockCode);
                        folderFields.Add("ItemDesc", this.ItemDesc);
                        folderFields.Add("Quantity", this.Quantity.ToString());
                        folderFields.Add("AvlQuantity", this.Quantity.ToString());
                        folderFields.Add("PurchasePrice", this.PurchasePrice.ToString());
                        folderFields.Add("SalePrice", this.SalePrice.ToString());//PurchaseProfitPercent
                        //folderFields.Add("ProductImage", System.Text.Encoding.UTF8.GetString(imgByteArr));
                        folderFields.Add("PurchaseDate", this.PurchaseDate.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
                        folderFields.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
                        folderFields.Add("ModifiedDate", this.Id == 0 ? null : DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));

                        int rslt = da.InsertOrUpdateStockMaster(folderFields, "tblStockMaster");
                        if (rslt > 0)
                        {
                            // adding Product Image
                            if (!string.IsNullOrEmpty(imageName))
                            {
                                //Initialize a file stream to read the image file
                                using (FileStream fs = new FileStream(@imageName, FileMode.Open, FileAccess.Read))
                                {
                                    //Initialize a byte array with size of stream
                                    imgByteArr = new byte[fs.Length];
                                    //Read data from the file stream and put into the byte array
                                    fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));
                                }
                                da.UpdateStockImage(imgByteArr, rslt, "tblStockMaster");
                            }
                            // add payment transaction
                            PaymentTransaction paymentTransaction = new PaymentTransaction();
                            bool paymentStatus = paymentTransaction.AddPaymentTransaction(Global.UserId, (double.Parse(this.PurchasePrice.ToString()) * double.Parse(this.Quantity.ToString())), PaymentStatus.PURCHASE_PAYMENT, rslt, da);
                            if (paymentStatus)
                            {
                                MessageBoxResult result = MessageBox.Show("Stock Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                PurchaseReset(new object());
                            }
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Error While Adding Stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
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
            bool IsFewItemsSold = false;
            // ToDo : if some Items sold dont allow to Change Selling Price
            if (item.Quantity != item.AvlQuantity) {
                IsFewItemsSold = true;
                MessageBoxResult result = MessageBox.Show("Some Items sold Already. So dont change the sale price!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            // Code Price Qty Date
            AssignPurchaseModelProperties(item);
            this.TabHeaderText = "Edit Purchase";
            this.IsAddMode = false;
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
        private void AddPurchaseType(object obj)
        {
            var item = (Purchase)obj;
            bool IsFewItemsSold = false;
            // ToDo : if some Items sold dont allow to Change Selling Price
            if (item.Quantity != item.AvlQuantity)
            {
                IsFewItemsSold = true;
                MessageBoxResult result = MessageBox.Show("Some Items sold Already. So dont change the sale price!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Id = item.Id;
                this.ProductCode = item.ProductCode;
                this.StockCode = item.StockCode;
                this.ItemDesc = item.ItemDesc;
                this.PurchasePrice = item.PurchasePrice;
                this.Quantity = item.Quantity;
                this.SalePrice = item.SalePrice;
                this.PurchaseDate = DateTime.Now;

                var bList = new List<Brand>((IEnumerable<Brand>)BrandList);
                var pList = new List<Product>((IEnumerable<Product>)ProductList);
                SBrand = bList.Find(x => x.Id == item.BrandId);
                SProduct = pList.Find(x => x.Id == item.ProductId);
            });
            this.TabHeaderText = "Add Purchase";
            this.IsAddMode = false;
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
            if (item.Quantity == item.AvlQuantity)
            {
                if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    using (DataAccess da = new DataAccess())
                    {
                        int stockId = da.DeleteStock(item.Id);
                        // Reverse Payment Transaction
                        PaymentTransaction paymentTransaction = new PaymentTransaction();
                        bool isReversed = paymentTransaction.ReversePaymentTransaction(Global.UserId, item.TotalPurchasePrice, CommonEnum.PaymentStatus.PURCHASEREVERSAL_PAYMENT, item.Id, da);
                        if (isReversed)
                        {
                            MessageBoxResult result = MessageBox.Show("Stock Deleted Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            // ResetForm();
                        }
                        PurchaseList.Remove(item);
                    }
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
        /// Get All Purchases/Stocks
        /// </summary>
        private void GetAllPurchases()
        {
            DataTable dt = new DataTable();
            using (DataAccess da = new DataAccess()) {
                dt = da.GetAllStocks();
            }

            this.PurchaseList.Clear();
            foreach (DataRow row in dt.Rows)
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
