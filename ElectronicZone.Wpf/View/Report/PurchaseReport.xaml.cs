using System;
using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

namespace ElectronicZone.Wpf.View.Report
{
    /// <summary>
    /// Interaction logic for PurchaseReport.xaml
    /// </summary>
    public partial class PurchaseReport : MetroWindow
    {
        ILogger logger = new Logger(typeof(PurchaseReport));

        public PurchaseReport()
        {
            InitializeComponent();
            LoadProduct();
            LoadBrands();

            DateTimeUtility dtUtility = new DateTimeUtility();
            fromDate.SelectedDate = dtUtility.GetMonthStartDate(null);
            toDate.SelectedDate = DateTime.Now;

            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dtPurchase = new DataTable();
                using (DataAccess da = new DataAccess()) {
                    dtPurchase = da.SearchStocks((this.cbProduct.SelectedValue == null ? "" : this.cbProduct.SelectedValue.ToString()), (this.cbBrandCompany.SelectedValue == null ? "" : this.cbBrandCompany.SelectedValue.ToString()), this.txtProdCode.Text, this.txtStockCode.Text, null, null,
                        string.IsNullOrEmpty(fromDate.Text) ? "" : (DateTime.Parse(fromDate.Text).ToString(ConfigurationManager.AppSettings["DateOnly"])),
                        string.IsNullOrEmpty(toDate.Text) ? "" : (DateTime.Parse(toDate.Text).ToString(ConfigurationManager.AppSettings["DateOnly"])),
                        (this.chkBoxAvailableStock.IsChecked == null ? false : this.chkBoxAvailableStock.IsChecked == true ? true : false));
                }
                if (dtPurchase.Rows.Count > 0)
                {
                    btnExport.Visibility = System.Windows.Visibility.Visible;
                    dataGridPurchase.ItemsSource = dtPurchase.DefaultView;
                }
                else
                {
                    btnExport.Visibility = System.Windows.Visibility.Hidden;
                    dataGridPurchase.ItemsSource = null;
                    MessageBoxResult result = MessageBox.Show((string)Application.Current.FindResource("NoDataFoundInfoMessage"), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetForm();
                dataGridPurchase.ItemsSource = null;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void ResetForm()
        {
            // combobox
            this.cbProduct.SelectedIndex = -1;
            this.cbBrandCompany.SelectedIndex = -1;
            // textbox
            this.txtProdCode.Text = "";
            this.txtStockCode.Text = "";
            //this.txtPriceFrom.Value = null;
            //this.txtPriceTo.Value = null;
            // date picker
            this.fromDate.Text = "";
            this.toDate.Text = "";
            // hide export button
            btnExport.Visibility = System.Windows.Visibility.Hidden;
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
            // bind to combobox
            cbProduct.ItemsSource = dtProduct.DefaultView;
        }
        /// <summary>
        /// Load All Active Brands
        /// </summary>
        private void LoadBrands()
        {
            DataTable dtBrand = new DataTable();
            using (DataAccess da = new DataAccess()) {
                dtBrand = da.GetAllBrands();
            }
            // bind to combobox
            cbBrandCompany.ItemsSource = dtBrand.DefaultView;
        }

        private void fromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateCalendarMinMaxDate();
        }

        private void toDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateCalendarMinMaxDate();
        }

        private void ValidateCalendarMinMaxDate()
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
            bool result = goExcelOut.GenerateExcelOutput(dataGridPurchase, "PurchaseReport");
            //if (result)
            //    MessageBox.Show("File exported successfully.");
        }
    }
}
