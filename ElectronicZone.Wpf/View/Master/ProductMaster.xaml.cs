using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using System;
using System.Windows;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;

namespace ElectronicZone.Wpf.View.Master
{
    /// <summary>
    /// Interaction logic for ProductMaster.xaml
    /// </summary>
    public partial class ProductMaster : MetroWindow
    {
        ILogger logger = new Logger(typeof(ProductMaster));
        DateTimeUtility dtUtils = new DateTimeUtility();
        public ProductMaster()
        {
            InitializeComponent();
            this.txtProdName.Focus();
            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        # region events
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validateForm())
                {
                    //create record
                    Dictionary<string, string> folderFields = new Dictionary<string, string>();
                    folderFields.Add("Id", null);
                    folderFields.Add("Name", txtProdName.Text);
                    folderFields.Add("Description", txtProdDesc.Text);
                    folderFields.Add("CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    folderFields.Add("ModifiedDate", null);

                    DataAccess dataAccess = new DataAccess();
                    int status = dataAccess.InsertOrUpdateProductMaster(folderFields, "tblProductMaster");
                    //check if it is insert/updated
                    if (status == 1)
                    {
                        MessageBoxResult result = MessageBox.Show("Product Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ResetForm();
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
            }
        }

        private bool validateForm()
        {
            DataAccess da = new DataAccess();
            if (string.IsNullOrEmpty(txtProdName.Text.Trim()))
            {
                txtProdName.Focus();
                return false;
            }
            else if (da.IfExistsValue("tblProductMaster", "Name", txtProdName.Text.Trim()))
            {
                MessageBoxResult result = MessageBox.Show("Name already exists!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
                return true;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }
        # endregion
        /// <summary>
        /// Load Products
        /// </summary>
        private void loadProducts()
        {
            DataTable dtProducts = new DataTable();
            DataAccess da = new DataAccess();

            dtProducts = da.GetAllProducts();
            datagridProducts.ItemsSource = dtProducts.DefaultView;
            //IEnumerable<DataRow> sequence = dtProducts.AsEnumerable();
            //List<DataRow> list = dt.AsEnumerable().ToList();
        }

        /// <summary>
        /// Reset Form Fields
        /// </summary>
        private void ResetForm()
        {
            this.txtProdName.Text = "";
            this.txtProdDesc.Text = "";
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem item = sender as TabItem;
            if (e.Source is TabControl)
            {
                //bind product list on demand
                if (tabControl1.SelectedIndex != 0)
                    loadProducts();
            }
        }

    }
}
