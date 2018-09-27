using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ElectronicZone.Wpf.View.Sale
{
    /// <summary>
    /// Interaction logic for AddSale.xaml
    /// </summary>
    public partial class AddSale : MetroWindow
    {
        ILogger logger = new Logger(typeof(AddSale));
        private int salePersonId = 0;
        public AddSale(object[] sale)
        {
            InitializeComponent();
            cbSalesPerson.Visibility = System.Windows.Visibility.Hidden;
            LoadSaleData(sale);

            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void LoadSaleData(object[] sale)
        {
            try
            {
                this.txtStockId.Text = sale[0].ToString();
                lblProduct.Content = string.Format("{0} ({1})", sale[2].ToString(), sale[4].ToString());
                //lblBrand.Content = sale[2].ToString();
                this.lblProductCode.Content = string.Format("{0} ({1})", sale[5].ToString(), sale[6].ToString());
                //this.txtStockCode.Text = sale[4].ToString();
                //this.txtItemDesc.Text = sale[5].ToString();
                this.lblAvlQuantity.Content = sale[9].ToString();
                //this.txtQuantity.Value = int.Parse(sale[6].ToString());
                this.lblPrice.Content = sale[11].ToString();//sale[7].ToString(); sale price/purchase price
                txtQuantity.Maximum = double.Parse(sale[9].ToString());
                this.dpSaleDate.DisplayDateStart = DateTime.Parse(sale[15].ToString());
                //this.chkbPersonType.IsChecked = true;
                if (sale[14] != System.DBNull.Value)
                {
                    //Store binary data read from the database in a byte array
                    byte[] blob = (byte[])sale[14];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;

                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();

                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    imagePhoto.Source = bi;
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        /// <summary>
        /// Save Sales [consists of 5 steps]
        /// 1. Create Sale Person If New
        /// 2. Add SaleMaster
        /// 3. Add Pending Payment if any
        /// 4. Add payment transaction
        /// 5. Update Stock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validateSale())
                {
                    // add/Create Sale Person
                    if (salePersonId == 0)
                        salePersonId = createSalePersonForStockSale();
                    if (salePersonId > 0)
                    {
                        double total = (double)txtQuantity.Value * int.Parse(lblPrice.Content.ToString());
                        bool isPending = (total > double.Parse(txtAmtPaid.Text) ? true : false);
                        double pendingAmt = total - double.Parse(txtAmtPaid.Text);
                        //Add SaleMaster
                        Dictionary<string, string> saleMasterModel = new Dictionary<string, string>();
                        saleMasterModel.Add("Id", null);
                        saleMasterModel.Add("StockId", txtStockId.Text);
                        saleMasterModel.Add("SalePersonId", salePersonId.ToString());
                        saleMasterModel.Add("Quantity", this.txtQuantity.Value.ToString());
                        saleMasterModel.Add("Price", lblPrice.Content.ToString());
                        saleMasterModel.Add("Total", total.ToString());
                        saleMasterModel.Add("AmountPaid", txtAmtPaid.Text);
                        //saleMasterModel.Add("Pending", pendingAmt.ToString());
                        saleMasterModel.Add("SaleDate", (DateTime.Parse(dpSaleDate.Text).ToString("yyyy-MM-dd HH:mm:ss")));
                        saleMasterModel.Add("CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        saleMasterModel.Add("ModifiedDate", null);
                        DataAccess dataAccess = new DataAccess();
                        int rslt = dataAccess.InsertOrUpdateSaleMaster(saleMasterModel, "tblSaleMaster");
                        if (rslt > 0)
                        {
                            if (isPending)
                            {
                                #region add pending payment
                                Dictionary<string, string> pendingPaymentModel = new Dictionary<string, string>();
                                pendingPaymentModel.Add("Id", null);
                                pendingPaymentModel.Add("SaleId", rslt.ToString());
                                //pendingPaymentModel.Add("StockId", txtStockId.Text);
                                pendingPaymentModel.Add("SalePersonId", salePersonId.ToString());
                                pendingPaymentModel.Add("PendingAmount", pendingAmt.ToString());
                                pendingPaymentModel.Add("IsPaid", "0");
                                int pendingRowId = dataAccess.InsertOrUpdatePendingPayment(pendingPaymentModel, "tblPendingPayment");
                                #endregion
                            }
                            // add payment transaction
                            PaymentTransaction paymentTransaction = new PaymentTransaction();
                            bool paymentStatus = paymentTransaction.AddPaymentTransaction(Global.UserId, double.Parse(txtAmtPaid.Text), PaymentTransaction.PaymentStatus.SALE_PAYMENT, rslt);
                            if (paymentStatus)
                            {
                                #region Update Stock Quantity
                                Dictionary<string, string> stockModel = new Dictionary<string, string>();
                                stockModel.Add("Id", txtStockId.Text);
                                stockModel.Add("AvlQuantity", this.txtQuantity.Value.ToString());
                                stockModel.Add("ModifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                int isUpdated = dataAccess.UpdateStockQuantity(stockModel, "tblStockMaster");
                                #endregion
                                if (isUpdated == 1)
                                {
                                    MessageBoxResult result = MessageBox.Show("Sale Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                    this.Close();
                                }
                                else
                                {
                                    MessageBoxResult result = MessageBox.Show("Error While Updating Stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBoxResult result = MessageBox.Show("Error While Adding Payment Transaction!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Error While Adding Sale!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Invalid Data !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }

        }

        private void ResetForm()
        {

        }

        private int createSalePersonForStockSale()
        {
            //create record
            Dictionary<string, string> salePersonModel = new Dictionary<string, string>();
            salePersonModel.Add("Id", null);
            salePersonModel.Add("Title", "");
            salePersonModel.Add("Name", txtSalesName.Text.Trim());
            salePersonModel.Add("Contact", txtSalesContact.Text);
            salePersonModel.Add("AlternateContact", "");
            salePersonModel.Add("Email", "");
            salePersonModel.Add("Address", "");

            DataAccess dataAccess = new DataAccess();
            int personId = dataAccess.InsertOrUpdateSalePerson(salePersonModel, "tblSalePerson");
            if (personId > 0)
                return personId;
            else
                return 0;
        }

        /// <summary>
        /// Validate Sales Data b4 Save
        /// </summary>
        /// <returns></returns>
        private Boolean validateSale()
        {
            bool isFormValid = true;
            double total = (double)txtQuantity.Value * int.Parse(lblPrice.Content.ToString());
            //bool isPending = total != double.Parse(txtAmtPaid.Text) ? true : false;
            int avlQty = int.Parse(this.lblAvlQuantity.Content.ToString());

            if (string.IsNullOrEmpty(txtAmtPaid.Text))
                isFormValid = false;
            if (string.IsNullOrEmpty(dpSaleDate.Text))
                isFormValid = false;
            if (txtQuantity.Value > avlQty)
                isFormValid = false;
            if (!string.IsNullOrEmpty(txtAmtPaid.Text) && double.Parse(txtAmtPaid.Text) > total)
                isFormValid = false;

            return isFormValid;
        }

        private void txtQuantity_ValueDecremented(object sender, MahApps.Metro.Controls.NumericUpDownChangedRoutedEventArgs args)
        {
            this.lblTotalPurchaseAmt.Content = CalculateTotalPrice(((this.txtQuantity.Value == null ? 0 : this.txtQuantity.Value) - 1).ToString(), this.lblPrice.Content.ToString());
        }

        private void txtQuantity_ValueIncremented(object sender, MahApps.Metro.Controls.NumericUpDownChangedRoutedEventArgs args)
        {
            this.lblTotalPurchaseAmt.Content = CalculateTotalPrice(((this.txtQuantity.Value == null ? 0 : this.txtQuantity.Value) + 1).ToString(), this.lblPrice.Content.ToString());
        }

        private double CalculateTotalPrice(string qty, string price)
        {
            double total = 0;
            if (!string.IsNullOrEmpty(qty) && !string.IsNullOrEmpty(price))
                total = double.Parse(qty) * double.Parse(price);
            return total;
        }

        private void chkbPersonType_IsCheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkbPersonType.IsChecked.Value;
            EnablePersonContact(isChecked);
            cbSalesPerson.IsEnabled = !isChecked;
            if (!isChecked)
            {
                // get all sales person
                //cbSalesPerson.Items.Clear();
                loadSalesPerson();
            }
            else
            {
                salePersonId = 0;
                this.txtSalesName.Text = "";
                this.txtSalesContact.Text = "";
            }
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

        private void EnablePersonContact(bool status)
        {
            this.txtSalesName.IsEnabled = status;
            this.txtSalesContact.IsEnabled = status;
            if (status)
                cbSalesPerson.Visibility = System.Windows.Visibility.Hidden;
            else
                cbSalesPerson.Visibility = System.Windows.Visibility.Visible;
        }

        private void cbSalesPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var personObj = ((System.Data.DataRowView)(((object[])(e.AddedItems))[0])).Row.ItemArray;
                this.txtSalesName.Text = personObj[2].ToString();//Person Name
                this.txtSalesContact.Text = personObj[3].ToString();//Person Contact
                salePersonId = int.Parse(personObj[0].ToString());//PersonId
                //string text = (sender as ComboBox).SelectedItem as string;
            }
        }
    }
}
