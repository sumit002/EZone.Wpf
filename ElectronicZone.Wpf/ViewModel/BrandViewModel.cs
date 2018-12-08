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
    public class BrandViewModel : ViewModelBase
    {
        #region Properties
        ILogger logger = new Logger(typeof(BrandViewModel));
        private IDialogCoordinator _dialogCoordinator;
        public ObservableCollection<Brand> BrandList { get; set; }
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
                if (_selectedIndex == 1) { GetAllBrands(); };
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
        public ICommand AddOrUpdateBrandCmd { get; set; }
        public ICommand ResetBrandCmd { get; set; }

        public ICommand EditBrandCmd { get; set; }
        public ICommand DeleteBrandCmd { get; set; }
        #endregion

        public BrandViewModel(IDialogCoordinator instance)
        {
            this.TabHeaderText = "Add Brand";
            this.BrandList = new ObservableCollection<Brand>();
            this._dialogCoordinator = instance;
            this.IsAddMode = true;

            EditBrandCmd = new CommandHandler(EditBrand, CanExecuteEditBrand);
            DeleteBrandCmd = new CommandHandler(DeleteBrand, CanExecuteDeleteBrandCmd);
            AddOrUpdateBrandCmd = new CommandHandler(AddOrUpdateBrand, CanExecuteAddOrUpdateBrand);
            ResetBrandCmd = new CommandHandler(ResetBrand, CanExecuteResetBrand);
            //GetAllBrands();
        }

        private bool CanExecuteEditBrand(object arg)
        {
            return true;
        }
        private void EditBrand(object obj)
        {
            var item = (Brand)obj;
            this.Id = item.Id;
            this.Name = item.Name;
            this.Description = item.Description;

            this.TabHeaderText = "Edit Brand";
            this.IsAddMode = false;
            this.TabSelectedIndex = 0;
        }

        private bool CanExecuteResetBrand(object arg)
        {
            return true;
        }
        private void ResetBrand(object obj)
        {
            ResetBrand();
        }

        /// <summary>
        /// Reset Brand
        /// </summary>
        private void ResetBrand()
        {
            try
            {
                this.Id = 0;
                this.Name = "";
                this.Description = "";
                this.IsAddMode = true;
                this.TabHeaderText = "Add Brand";
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private bool CanExecuteAddOrUpdateBrand(object arg)
        {
            return true;
        }
        private void AddOrUpdateBrand(object obj)
        {
            //var item = (Brand)obj;
            using (DataAccess da = new DataAccess()) {
                try
                {
                    if (ValidateBrand(da))
                    {
                        //create record
                        Dictionary<string, string> folderFields = new Dictionary<string, string>();
                        folderFields.Add("Id", this.Id == 0 ? null : this.Id.ToString());
                        folderFields.Add("Name", this.Name);
                        folderFields.Add("Description", this.Description);
                        folderFields.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
                        folderFields.Add("ModifiedDate", this.Id == 0 ? null : DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));

                        int status = da.InsertOrUpdateBrandMaster(folderFields, "tblBrandMaster");
                        //check if it is insert/updated
                        if (status == 1) {
                            MessageBoxResult result = MessageBox.Show("Brand Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ResetBrand();
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Error While Adding Brand!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private bool ValidateBrand(DataAccess da)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return false;
            }
            else if (da.IfExistsValue("tblBrandMaster", "Name", this.Name) && this.IsAddMode)
            {
                MessageBoxResult result = MessageBox.Show("Name already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
                return true;
        }

        private void DeleteBrand(object param)
        {
            var item = (Brand)param;
            if (MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                using (DataAccess da = new DataAccess()) {
                    da.DeleteBrand(item.Id);
                }
                BrandList.Remove((Brand)param);
            }  
        }

        private bool CanExecuteDeleteBrandCmd(object parameter)
        {
            return SelectedResult != null;
        }

        private async void GetAllBrands() {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Loading", "Please wait for a while...");
            controller.SetIndeterminate();

            DataTable dtBrands = new DataTable();
            using (DataAccess da = new DataAccess()) {
                dtBrands = da.GetAllBrands();
            }
            //List<BrandModel> Brands = new List<BrandModel>();
            //Brands = CommonMethods.ConvertDataTable<BrandModel>(dtBrands);
            this.BrandList.Clear();
            foreach (DataRow row in dtBrands.Rows)
            {
                this.BrandList.Add(new Brand()
                {
                    Id = int.Parse(row["Id"].ToString()),
                    Name = (string)row["Name"],
                    Description = (string)row["Description"],
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedDate = string.IsNullOrEmpty(row["ModifiedDate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ModifiedDate"].ToString()),
                    IsNotUsed = !Convert.ToBoolean(row["IsUsed"])
                });
            }
            await controller.CloseAsync();
        }
    }
}
