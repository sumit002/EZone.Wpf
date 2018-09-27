using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Helper;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ElectronicZone.Wpf.ViewModel
{
    public class BrandViewModel : ViewModelBase
    {
        public ObservableCollection<Brand> BrandList { get; set; }
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

        // Actions/Commands
        public ICommand DeleteBrandCmd { get; set; }

        public BrandViewModel()
        {
            this.BrandList = new ObservableCollection<Brand>();
            DeleteBrandCmd = new CommandHandler(DeleteBrand, CanExecuteDeleteBrandCmd);
            GetAllBrands();
        }

        private void DeleteBrand(object param)
        {
            DataAccess da = new DataAccess();
            da.DeleteBrand(((Brand)param).Id);
            BrandList.Remove((Brand)param);
        }

        private bool CanExecuteDeleteBrandCmd(object parameter)
        {
            return SelectedResult != null;
        }

        private void GetAllBrands() {
            DataTable dtBrands = new DataTable();
            DataAccess da = new DataAccess();

            dtBrands = da.GetAllBrands();
            //List<BrandModel> Brands = new List<BrandModel>();
            //Brands = CommonMethods.ConvertDataTable<BrandModel>(dtBrands);

            foreach (DataRow row in dtBrands.Rows)
            {
                this.BrandList.Add(new Brand()
                {
                    Id = int.Parse(row["Id"].ToString()),
                    Name = (string)row["Name"],
                    Description = (string)row["Description"],
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifiedDate = string.IsNullOrEmpty(row["ModifiedDate"].ToString()) ? (DateTime?)null : DateTime.Parse(row["ModifiedDate"].ToString())// Convert.ToDateTime(row["ModifiedDate"])
                });
            }
        }
    }
}
