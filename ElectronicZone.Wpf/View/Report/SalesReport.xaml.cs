using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectronicZone.Wpf.View.Report
{
    /// <summary>
    /// Interaction logic for SalesReport.xaml
    /// </summary>
    public partial class SalesReport : MetroWindow
    {
        ILogger logger = new Logger(typeof(SalesReport));
        public SalesReport()
        {
            InitializeComponent();
            loadProduct();
            loadBrands();
            loadSalesPerson();

            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
            dataGridSales.ItemsSource = null;
        }

        private void ResetForm()
        {
            // combobox
            this.cbProduct.SelectedIndex = -1;
            this.cbBrandCompany.SelectedIndex = -1;
            this.cbSalesPerson.SelectedIndex = -1;
            // textbox
            this.txtProdCode.Text = "";
            this.txtStockCode.Text = "";
            this.txtPriceFrom.Value = null;
            this.txtPriceTo.Value = null;
            // date picker
            this.fromDate.Text = "";
            this.toDate.Text = "";
            // hide export button
            btnExport.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dtSales = new DataTable();
                DataAccess da = new DataAccess();
                dtSales = da.SearchSales((this.cbProduct.SelectedValue == null ? "" : this.cbProduct.SelectedValue.ToString()), (this.cbBrandCompany.SelectedValue == null ? "" : this.cbBrandCompany.SelectedValue.ToString()), this.txtProdCode.Text, this.txtStockCode.Text, (int?)this.txtPriceFrom.Value, (int?)this.txtPriceTo.Value,
                    string.IsNullOrEmpty(fromDate.Text) ? "" : (DateTime.Parse(fromDate.Text).ToString(ConfigurationManager.AppSettings["DateTimeFormat"])),
                    string.IsNullOrEmpty(toDate.Text) ? "" : (DateTime.Parse(toDate.Text).ToString(ConfigurationManager.AppSettings["DateTimeFormat"])),
                    (this.cbSalesPerson.SelectedValue == null ? string.Empty : this.cbSalesPerson.SelectedValue.ToString()));

                if (dtSales.Rows.Count > 0)
                {
                    btnExport.Visibility = System.Windows.Visibility.Visible;
                    dataGridSales.ItemsSource = dtSales.DefaultView;
                }
                else
                {
                    btnExport.Visibility = System.Windows.Visibility.Hidden;
                    dataGridSales.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }

        }

        /// <summary>
        /// Load All Active Products
        /// </summary>
        private void loadProduct()
        {
            DataTable dtProduct = new DataTable();
            DataAccess da = new DataAccess();
            dtProduct = da.GetAllProducts();
            // bind to combobox
            cbProduct.ItemsSource = dtProduct.DefaultView;
        }
        /// <summary>
        /// Load All Active Brands
        /// </summary>
        private void loadBrands()
        {
            DataTable dtBrand = new DataTable();
            DataAccess da = new DataAccess();
            dtBrand = da.GetAllBrands();
            // bind to combobox
            cbBrandCompany.ItemsSource = dtBrand.DefaultView;
        }
        /// <summary>
        /// 
        /// </summary>
        private void loadSalesPerson()
        {
            DataTable dtProduct = new DataTable();
            DataAccess da = new DataAccess();
            dtProduct = da.GetAllSalesPerson();
            // bind to combobox
            cbSalesPerson.ItemsSource = dtProduct.DefaultView;
            cbSalesPerson.SelectedItem = null;
            //cbSalesPerson.SelectedIndex = -1;
        }

        private void fromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            validateCalendarMinMaxDate();
        }

        private void toDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            validateCalendarMinMaxDate();
        }

        private void validateCalendarMinMaxDate()
        {
            try
            {
                this.fromDate.DisplayDateEnd = string.IsNullOrEmpty(toDate.Text) ? (DateTime?)null : DateTime.Parse(toDate.Text);
                this.toDate.DisplayDateStart = string.IsNullOrEmpty(fromDate.Text) ? (DateTime?)null : DateTime.Parse(fromDate.Text);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            goExcelOut goExcelOut = new goExcelOut();
            bool result = goExcelOut.generateExcel(dataGridSales, "SalesReport");
            if (result)
                MessageBox.Show("File exported successfully.");
        }
    }
}