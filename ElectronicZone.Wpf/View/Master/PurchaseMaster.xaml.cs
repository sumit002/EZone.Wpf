using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ElectronicZone.Wpf.View.Master
{
    public partial class PurchaseMaster : MetroWindow
    {
        ILogger logger = new Logger(typeof(PurchaseMaster));
        DateTimeUtility dtUtils = new DateTimeUtility();
        string imageName = "";
        public PurchaseMaster()
        {
            InitializeComponent();
            this.cbProduct.Focus();
            this.txtQuantity.Value = 1;
            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }


        #region events
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            byte[] imgByteArr = null;
            try
            {
                if (validateForm())
                {
                    //create stock record
                    Dictionary<string, string> folderFields = new Dictionary<string, string>();
                    folderFields.Add("Id", null);
                    folderFields.Add("ProductId", cbProduct.SelectedValue.ToString());
                    folderFields.Add("BrandId", cbBrandCompany.SelectedValue.ToString());
                    folderFields.Add("ProductCode", txtProdCode.Text);
                    folderFields.Add("StockCode", txtStockCode.Text);
                    folderFields.Add("ItemDesc", txtItemDesc.Text);
                    folderFields.Add("Quantity", txtQuantity.Value.ToString());
                    folderFields.Add("AvlQuantity", txtQuantity.Value.ToString());
                    folderFields.Add("PurchasePrice", txtPurchasePrice.Text);
                    folderFields.Add("SalePrice", txtSalePrice.Text);
                    //folderFields.Add("ProductImage", System.Text.Encoding.UTF8.GetString(imgByteArr));
                    folderFields.Add("PurchaseDate", (DateTime.Parse(dpPurchaseDate.Text).ToString("yyyy-MM-dd HH:mm:ss")));
                    folderFields.Add("CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    folderFields.Add("ModifiedDate", null);

                    DataAccess dataAccess = new DataAccess();
                    int rslt = dataAccess.InsertOrUpdateStockMaster(folderFields, "tblStockMaster");
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
                                //fs.Close();
                            }
                            dataAccess.UpdateStockImage(imgByteArr, rslt, "tblStockMaster");
                        }
                        // add payment transaction
                        PaymentTransaction paymentTransaction = new PaymentTransaction();
                        bool paymentStatus = paymentTransaction.AddPaymentTransaction(1, (double.Parse(txtPurchasePrice.Text) * double.Parse(txtQuantity.Value.ToString())), PaymentTransaction.PaymentStatus.PURCHASE_PAYMENT, rslt);
                        if (paymentStatus)
                        {
                            MessageBoxResult result = MessageBox.Show("Stock Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            ResetForm();
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Error While Adding Stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if ((cbProduct.SelectedValue == null))
                return false;
            else if (cbBrandCompany.SelectedValue == null)
                return false;
            else if (string.IsNullOrEmpty(txtProdCode.Text.Trim()))
            {
                txtProdCode.Focus();
                return false;
            }
            //else if (string.IsNullOrEmpty(txtStockCode.Text.Trim())){
            //    txtStockCode.Focus();
            //    return false;
            //}
            //else if (string.IsNullOrEmpty(txtItemDesc.Text.Trim()))
            //    return false;
            else if (string.IsNullOrEmpty(txtPurchasePrice.Text.Trim()))
            {
                txtPurchasePrice.Focus();
                return false;
            }
            else if (string.IsNullOrEmpty(txtQuantity.Value.ToString()))
            {
                txtQuantity.Focus();
                return false;
            }
            else if (string.IsNullOrEmpty(txtSalePrice.Text.Trim()))
            {
                txtSalePrice.Focus();
                return false;
            }
            else if (string.IsNullOrEmpty(dpPurchaseDate.Text.Trim()))
            {
                dpPurchaseDate.Focus();
                return false;
            }
            else
                return true;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetForm();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }
        # endregion

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

        private void loadStocks()
        {
            DataTable dtStocks = new DataTable();
            DataAccess da = new DataAccess();

            dtStocks = da.GetAllStocks();
            datagridStocks.ItemsSource = dtStocks.DefaultView;
            //datagridStocks.UpdateLayout();
        }
        /// <summary>
        /// Reset Form Fields
        /// </summary>
        private void ResetForm()
        {
            // combobox
            this.cbProduct.SelectedIndex = -1;
            this.cbBrandCompany.SelectedIndex = -1;
            // textbox
            this.txtProdCode.Text = "";
            this.txtStockCode.Text = "";
            this.txtItemDesc.Text = "";
            this.txtQuantity.Value = 1;
            this.txtPurchasePrice.Text = "";
            this.txtSalePrice.Text = "";
            // datepicker
            this.dpPurchaseDate.Text = "";
            this.imagePhoto.Source = null;
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                //bind brand list on demand
                if (tabControl1.SelectedIndex == 0)
                {
                    loadProduct();
                    loadBrands();
                }
                else
                    loadStocks();
            }
        }

        private void txtPurchasePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.lblTotalPurchaseAmt.Content = CalculateTotalPrice(this.txtQuantity.Value.ToString(), this.txtPurchasePrice.Text);
        }

        //private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    this.lblTotalPurchaseAmt.Content = CalculateTotalPrice(this.txtQuantity.Value.ToString(), this.txtPurchasePrice.Text);
        //}

        private double CalculateTotalPrice(string qty, string price)
        {
            double total = 0;
            if (!string.IsNullOrEmpty(qty) && !string.IsNullOrEmpty(price))
                total = double.Parse(qty) * double.Parse(price);
            return total;
        }

        private void txtQuantity_ValueChanged(object sender, MahApps.Metro.Controls.NumericUpDownChangedRoutedEventArgs args)
        {
            this.lblTotalPurchaseAmt.Content = CalculateTotalPrice(this.txtQuantity.Value.ToString(), this.txtPurchasePrice.Text);
        }

        private void txtQuantity_ValueIncremented(object sender, MahApps.Metro.Controls.NumericUpDownChangedRoutedEventArgs args)
        {
            this.lblTotalPurchaseAmt.Content = CalculateTotalPrice((this.txtQuantity.Value + 1).ToString(), this.txtPurchasePrice.Text);
        }

        private void txtQuantity_ValueDecremented(object sender, MahApps.Metro.Controls.NumericUpDownChangedRoutedEventArgs args)
        {
            this.lblTotalPurchaseAmt.Content = CalculateTotalPrice((this.txtQuantity.Value - 1).ToString(), this.txtPurchasePrice.Text);
        }

        private void btnUploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileDialog fldlg = new OpenFileDialog();
                fldlg.Title = "Select an Image";
                fldlg.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
                fldlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                "Portable Network Graphic (*.png)|*.png";
                if (fldlg.ShowDialog() == true)
                {
                    imageName = fldlg.FileName;
                    ImageSourceConverter isc = new ImageSourceConverter();
                    imagePhoto.SetValue(Image.SourceProperty, isc.ConvertFromString(imageName));
                }
                fldlg = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                logger.LogException(ex);
            }

            //OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            //op.Title = "Select a picture";
            //op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
            //  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
            //  "Portable Network Graphic (*.png)|*.png";
            //if (op.ShowDialog() == true)
            //{
            //    imagePhoto.Source = new BitmapImage(new Uri(op.FileName));
            //}
        }

        private void txtSalePrice_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtSalePrice.Text) && double.Parse(this.txtSalePrice.Text) < double.Parse(this.txtPurchasePrice.Text))
            {
                btnSave.IsEnabled = false;
                MessageBoxResult result = MessageBox.Show("Sale Price should be grater than or equal to purchase price!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                btnSave.IsEnabled = true;
        }
    }
}
