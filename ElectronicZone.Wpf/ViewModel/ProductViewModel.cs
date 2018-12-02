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
using System.Windows;
using System.Windows.Input;

namespace ElectronicZone.Wpf.ViewModel
{
    public class ProductViewModel : ViewModelBase
    {
        #region Properties
        ILogger logger = new Logger(typeof(ProductViewModel));
        private IDialogCoordinator _dialogCoordinator;
        public ObservableCollection<Product> ProductList { get; set; }
        #endregion

        #region UI Models
        private int _id;
        private string _name;
        private string _description;
        private string _tabHeaderText;
        private bool _isAddMode;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
        public string TabHeaderText { get => _tabHeaderText; set { _tabHeaderText = value; OnPropertyChanged(); } }
        public bool IsAddMode { get => _isAddMode; set { _isAddMode = value; OnPropertyChanged(); } }

        private int _selectedIndex;
        public int TabSelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                if (_selectedIndex == 1) { GetAllProducts(); };
            }
        }

        private Brand _selectedResult;
        public Brand SelectedResult
        {
            get { return _selectedResult; }
            set
            {
                _selectedResult = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand AddOrUpdateProductCmd { get; set; }
        public ICommand ResetProductCmd { get; set; }

        public ICommand EditProductCmd { get; set; }
        public ICommand DeleteProductCmd { get; set; }
        #endregion

        public ProductViewModel(IDialogCoordinator instance)
        {
            this.TabHeaderText = "Add Brand";
            this.ProductList = new ObservableCollection<Product>();
            this._dialogCoordinator = instance;
            this.IsAddMode = true;

            AddOrUpdateProductCmd = new CommandHandler(AddOrUpdateProduct, CanExecuteAddOrUpdateProduct);
            ResetProductCmd = new CommandHandler(ResetProduct, CanExecuteAddOrUpdateProduct);
            EditProductCmd = new CommandHandler(EditProduct, CanExecuteAddOrUpdateProduct);
            DeleteProductCmd = new CommandHandler(DeleteProduct, CanExecuteAddOrUpdateProduct);
        }

        private void DeleteProduct(object obj)
        {
            var item = (Product)obj;
            if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                using (DataAccess da = new DataAccess()) {
                    da.DeleteProduct(item.Id);
                }
                this.ProductList.Remove(item);
            }
        }

        private void EditProduct(object obj)
        {
            var item = (Product)obj;
            this.Id = item.Id;
            this.Name = item.Name;
            this.Description = item.Description;

            //this.TabHeaderText = "Edit Brand";
            this.IsAddMode = false;
            this.TabSelectedIndex = 0;
        }

        private void ResetProduct(object obj)
        {
            try
            {
                this.Id = 0;
                this.Name = "";
                this.Description = "";
                this.IsAddMode = true;
                this.TabHeaderText = "Add Product";
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void AddOrUpdateProduct(object obj)
        {
            //var item = (Brand)obj;
            using (DataAccess da = new DataAccess())
            {
                try
                {
                    if (ValidateProduct(da)) {
                        Dictionary<string, string> folderFields = new Dictionary<string, string>();
                        folderFields.Add("Id", this.Id == 0 ? null : this.Id.ToString());
                        folderFields.Add("Name", this.Name);
                        folderFields.Add("Description", this.Description);
                        folderFields.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
                        folderFields.Add("ModifiedDate", this.Id == 0 ? null : DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));

                        int status = da.InsertOrUpdateProductMaster(folderFields, "tblProductMaster");
                        if (status == 1) {
                            MessageBoxResult result = MessageBox.Show("Product Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ResetProduct(new object());
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Error While Adding Product!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Invalid Data ! Please check the fields entered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                    da.RollbackTransaction();
                }
            }
        }

        private bool ValidateProduct(DataAccess da)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return false;
            }
            else if (da.IfExistsValue("tblProductMaster", "Name", this.Name) && this.IsAddMode)
            {
                MessageBoxResult result = MessageBox.Show("Name already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
                return true;
        }

        private bool CanExecuteAddOrUpdateProduct(object arg)
        {
            return true;
        }

        /// <summary>
        /// Get All Products
        /// </summary>
        private async void GetAllProducts()
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", "Please wait for a while...");
            controller.SetIndeterminate();

            DataTable dtProducts = new DataTable();
            using (DataAccess da = new DataAccess()) {
                dtProducts = da.GetAllProducts();
            }

            this.ProductList.Clear();
            foreach (DataRow row in dtProducts.Rows)
            {
                this.ProductList.Add(new Product()
                {
                    Id = int.Parse(row["Id"].ToString()),
                    Name = (string)row["Name"],
                    Description = (string)row["Description"],
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedDate = string.IsNullOrEmpty(row["ModifiedDate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ModifiedDate"].ToString())// Convert.ToDateTime(row["ModifiedDate"])
                });
            }

            await controller.CloseAsync();
        }
    }
}
