using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using ElectronicZone.Wpf.View.Sale;
using ElectronicZone.Wpf.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectronicZone.Wpf.View.Master
{
    /// <summary>
    /// Interaction logic for SaleMaster.xaml
    /// </summary>
    public partial class SaleMaster : MetroWindow
    {
        SaleViewModel vm = new SaleViewModel(DialogCoordinator.Instance);
        //ILogger logger = new Logger(typeof(SaleMaster));
        public SaleMaster()
        {
            InitializeComponent();
            this.DataContext = vm;
            //this.txtPriceTo.Minimum = (double) (this.txtPriceFrom.Value == null ? 1 : this.txtPriceFrom.Value);

            // on esc close
            //this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        //private void HandleEsc(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //        Close();
        //}

        //private void btnSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        SearchSale();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogException(ex);
        //    }
        //}

        //private void SearchSale()
        //{
        //    DataTable dtProduct = new DataTable();
        //    using (DataAccess da = new DataAccess()) {
        //        dtProduct = da.SearchStocks((this.cbProduct.SelectedValue == null ? "" : this.cbProduct.SelectedValue.ToString()), (this.cbBrandCompany.SelectedValue == null ? "" : this.cbBrandCompany.SelectedValue.ToString()), this.txtProdCode.Text, this.txtStockCode.Text, (int?)this.txtPriceFrom.Value, (int?)this.txtPriceTo.Value, string.Empty, string.Empty, true);
        //    }
        //    if (dtProduct.Rows.Count > 0)
        //        dataGridPurchase.ItemsSource = dtProduct.DefaultView;
        //    else
        //    {
        //        dataGridPurchase.ItemsSource = null;
        //        MessageBoxResult result = MessageBox.Show("No Data Found", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //}

        //private void btnReset_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        ResetForm();
        //        dataGridPurchase.ItemsSource = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogException(ex);
        //    }
        //}

        ///// <summary>
        ///// Load All Active Products
        ///// </summary>
        //private void loadProduct()
        //{
        //    DataTable dtProduct = new DataTable();
        //    using (DataAccess da = new DataAccess()) {
        //        dtProduct = da.GetAllProducts();
        //    }
        //    // bind to combobox
        //    cbProduct.ItemsSource = dtProduct.DefaultView;
        //}
        ///// <summary>
        ///// Load All Active Brands
        ///// </summary>
        //private void LoadBrands()
        //{
        //    DataTable dtBrand = new DataTable();
        //    using (DataAccess da = new DataAccess()) {
        //        dtBrand = da.GetAllBrands();
        //    }
        //    // bind to combobox
        //    cbBrandCompany.ItemsSource = dtBrand.DefaultView;
        //}

        //private void LoadSales()
        //{
        //    DataTable dtSales = new DataTable();
        //    using (DataAccess da = new DataAccess())
        //    {
        //        dtSales = da.GetAllSales(string.Empty);
        //    }

        //    datagridSales.ItemsSource = dtSales.DefaultView;
        //    //datagridStocks.UpdateLayout();
        //}
        ///// <summary>
        ///// Reset Form Fields
        ///// </summary>
        //private void ResetForm()
        //{
        //    // combobox
        //    this.cbProduct.SelectedIndex = -1;
        //    this.cbBrandCompany.SelectedIndex = -1;
        //    // textbox
        //    this.txtProdCode.Text = "";
        //    this.txtStockCode.Text = "";
        //    this.txtPriceFrom.Value = null;
        //    this.txtPriceTo.Value = null;
        //}

        //private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.Source is TabControl)
        //    {
        //        //bind Sales list on demand
        //        if (tabControl1.SelectedIndex == 0)
        //        {
        //            loadProduct();
        //            LoadBrands();
        //        }
        //        else
        //        {
        //            LoadSales();
        //        }
        //    }
        //}

        //private void dataGridPurchase_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        //{
        //    //Get the newly selected cells
        //    IList<DataGridCellInfo> selectedcells = e.AddedCells;

        //    DataRowView drv = (DataRowView)dataGridPurchase.SelectedItem;
        //    if (drv != null)
        //    {
        //        var selectedRow = drv.Row.ItemArray;

        //        //open modal for sale Item
        //        AddSale saleScreen = new AddSale(selectedRow);
        //        saleScreen.ShowDialog();
        //        //refresh sale data
        //        SearchSale();
        //    }

        //}
    }
}
