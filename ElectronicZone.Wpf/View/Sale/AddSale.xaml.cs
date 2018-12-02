using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectronicZone.Wpf.View.Sale
{
    /// <summary>
    /// Interaction logic for AddSale.xaml
    /// </summary>
    public partial class AddSale : MetroWindow
    {
        //SaleViewModel vm = new SaleViewModel(DialogCoordinator.Instance);
        ILogger logger = new Logger(typeof(AddSale));
        private int salePersonId, avlQuantity = 0;
        private readonly Purchase purchase;
        private string salePersonEmail = "";
        private readonly double SaleOrderOfferPercentage, SaleOrderPromotionalOfferPercentage = 0;
        DataTable dtContacts = new DataTable();
        private double totalSaleAmt = 0;

        public AddSale(Purchase obj)
        {
            InitializeComponent();
            //this.DataContext = vm;
            //cbSalesPerson.Visibility = System.Windows.Visibility.Hidden;
            this.purchase = obj;

            SaleOrderPromotionalOfferPercentage = Convert.ToDouble(ConfigurationManager.AppSettings["SaleOrderPromotionalOfferPercentage"]);
            txtDiscountPercentage.Value = this.SaleOrderOfferPercentage = Convert.ToDouble(ConfigurationManager.AppSettings["SaleOrderOfferPercentage"]);

            LoadSaleData();

            DiscountGridContainer.Visibility = ChkbDiscountedSale.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            LoadSalesPerson();


            // on esc close
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void LoadSaleData()
        {
            try
            {
                if (SaleOrderPromotionalOfferPercentage > 0) {
                    OfferRow.Height = GridLength.Auto;
                    this.lblSaleOfferDisplay.Content = SaleOrderPromotionalOfferPercentage.ToString();
                }
                this.txtStockId.Text = purchase.Id.ToString();
                //lblProduct.Content = string.Format("{0} ({1})", purchase.Product, purchase.Brand);
                //lblBrand.Content = sale[2].ToString();
                //this.lblProductCode.Content = string.Format("{0} ({1})", purchase.ProductCode, purchase.StockCode);
                //this.txtStockCode.Text = sale[4].ToString();
                //this.txtItemDesc.Text = sale[5].ToString();
                avlQuantity = purchase.AvlQuantity;
                this.lblAvlQuantity.Content = purchase.AvlQuantity.ToString();
                //this.txtQuantity.Value = int.Parse(sale[6].ToString());
                this.tbItemPrice.Text = String.Format("{0:n}", purchase.SalePrice);//sale[7].ToString(); sale price/purchase price
                txtQuantity.Maximum = double.Parse(purchase.AvlQuantity.ToString());
                this.dpSaleDate.DisplayDateStart = purchase.PurchaseDate;
                if(DateTime.Today > purchase.PurchaseDate)
                    this.dpSaleDate.SelectedDate = DateTime.Today;


                this.badgedItem.Badge = purchase.AvlQuantity.ToString();
                this.btnBadge.Content = string.Format("{0} ({1})", purchase.Product, purchase.ProductCode);
                this.sliderAmountPaid.Minimum = purchase.PurchasePrice;
                this.sliderAmountPaid.Maximum = 500;
                //this.chkbPersonType.IsChecked = true;
                //if (sale[14] != System.DBNull.Value)
                //{
                //    //Store binary data read from the database in a byte array
                //    byte[] blob = (byte[])sale[14];
                //    MemoryStream stream = new MemoryStream();
                //    stream.Write(blob, 0, blob.Length);
                //    stream.Position = 0;

                //    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                //    BitmapImage bi = new BitmapImage();
                //    bi.BeginInit();

                //    MemoryStream ms = new MemoryStream();
                //    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                //    ms.Seek(0, SeekOrigin.Begin);
                //    bi.StreamSource = ms;
                //    bi.EndInit();
                //    imagePhoto.Source = bi;
                //}
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            CreateSaleOrder();
        }

        private void CreateSaleOrder()
        {
            if (ValidateSaleForm())
            {
                // double total = (int)txtQuantity.Value * double.Parse(tbItemPrice.Text.ToString());
                ElectronicZone.Wpf.Model.Sale saleOrder = new ElectronicZone.Wpf.Model.Sale()
                {
                    StockId = Convert.ToInt32(this.txtStockId.Text),
                    Quantity = Convert.ToInt32(txtQuantity.Value.ToString()),
                    Price = Convert.ToDouble(tbItemPrice.Text.ToString()),
                    Total = totalSaleAmt,
                    AmountPaid = Convert.ToDouble(this.txtAmtPaid.Text.ToString()),
                    SaleDate = dpSaleDate.SelectedDate.Value,
                    IsDiscounted = this.ChkbDiscountedSale.IsChecked.Value,

                    Contact = new Contact() { Id = salePersonId, Name = this.txtSalesName.Text, PrimaryContact = this.txtSalesContact.Text, Email = salePersonEmail }
                };

                SaleManager sm = new SaleManager();
                sm.CreateSalesOrder(saleOrder);
                this.Close();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Invalid Data !", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                DiscountGridContainer.Visibility = ChkbDiscountedSale.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
                if (ChkbDiscountedSale.IsChecked.Value)
                {
                    sliderAmountPaid.SelectionStart = totalSaleAmt - (this.purchase.PurchasePrice * txtQuantity.Value.Value * SaleOrderOfferPercentage / 100);
                    AvailDiscountByPercentage(SaleOrderOfferPercentage);
                }
                else
                    this.txtAmtPaid.Text = this.totalSaleAmt.ToString();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private Boolean ValidateSaleForm()
        {
            bool isFormValid = true;

            if ((salePersonId == 0) && (string.IsNullOrEmpty(txtSalesName.Text) && string.IsNullOrEmpty(txtSalesContact.Text)))
                return false;
            using (DataAccess da = new DataAccess())
            {
                if (salePersonId == 0 && (!string.IsNullOrEmpty(txtSalesName.Text) && !string.IsNullOrEmpty(txtSalesContact.Text))
                    && da.IfContactExists("tblSalePerson", "Name", "Contact", txtSalesName.Text, txtSalesContact.Text))
                {
                    MessageBoxResult result = MessageBox.Show("Contact already exists! Please Select from List", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            if (txtQuantity.Value == null)
                isFormValid = false;

            // double total = (txtQuantity.Value == null ? 0 : (int)txtQuantity.Value) * double.Parse(tbItemPrice.Text.ToString());
            //bool isPending = total != double.Parse(txtAmtPaid.Text) ? true : false;
            //int avlQty = int.Parse(this.lblAvlQuantity.Content.ToString());

            if (string.IsNullOrEmpty(txtAmtPaid.Text))
                isFormValid = false;
            if (string.IsNullOrEmpty(dpSaleDate.Text))
                isFormValid = false;
            if (txtQuantity.Value > avlQuantity)
                isFormValid = false;
            if (!string.IsNullOrEmpty(txtAmtPaid.Text) && double.Parse(txtAmtPaid.Text) > totalSaleAmt)
                isFormValid = false;

            return isFormValid;
        }

        private void txtQuantity_ValueDecremented(object sender, MahApps.Metro.Controls.NumericUpDownChangedRoutedEventArgs args)
        {
            try {
                int qty = ((int)(this.txtQuantity.Value == null ? 0 : this.txtQuantity.Value) - 1);
                this.lblTotalSaleAmt.Content = totalSaleAmt = CalculateTotalPrice(qty, Convert.ToDouble(this.tbItemPrice.Text));
                this.txtAmtPaid.Text = totalSaleAmt.ToString();
                this.sliderAmountPaid.Maximum = totalSaleAmt;
                this.sliderAmountPaid.Minimum = qty * purchase.PurchasePrice;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void txtQuantity_ValueIncremented(object sender, MahApps.Metro.Controls.NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                int qty = ((int)(this.txtQuantity.Value == null ? 0 : this.txtQuantity.Value) + 1);
                this.lblTotalSaleAmt.Content = totalSaleAmt = CalculateTotalPrice(qty, Convert.ToDouble(this.tbItemPrice.Text));
                this.txtAmtPaid.Text = totalSaleAmt.ToString();
                this.sliderAmountPaid.Maximum = totalSaleAmt;
                this.sliderAmountPaid.Minimum = qty * purchase.PurchasePrice;
                // Show Avail Offer
                ChkbDiscountedSale.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private double CalculateTotalPrice(int qty, double price)
        {
            double total = 0;
            if (qty > 0 && price != 0)
                total = qty * price;
            return total;
        }

        private void chkbPersonType_IsCheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkbPersonType.IsChecked.Value;
            EnablePersonContact(isChecked);
            cbSalesPerson.IsEnabled = !isChecked;
            if (!isChecked)
            {
                LoadSalesPerson();
            }
            else
            {
                salePersonId = 0;
                this.txtSalesName.Text = this.txtSalesContact.Text = "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadSalesPerson()
        {
            //DataTable dtProduct = new DataTable();
            using (DataAccess da = new DataAccess())
            {
                dtContacts = da.GetAllSalesPerson();
            }
            // bind to combobox
            cbSalesPerson.ItemsSource = dtContacts.DefaultView;
            cbSalesPerson.SelectedItem = null;
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

        private void sliderAmountPaid_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                this.txtAmtPaid.Text = string.Format("{0:0.00}", Math.Round(sliderAmountPaid.Value));
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void txtDiscountPercentage_ValueDecremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                int discount = ((int)(this.txtDiscountPercentage.Value == null ? 0 : this.txtDiscountPercentage.Value) - 1);
                AvailDiscountByPercentage(discount);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void txtDiscountPercentage_ValueIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                int discount = ((int)(this.txtDiscountPercentage.Value == null ? 0 : this.txtDiscountPercentage.Value) + 1);
                AvailDiscountByPercentage(discount);
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void AvailDiscountByPercentage(double discountPercent)
        {
            this.txtAmtPaid.Text = (totalSaleAmt - (this.purchase.PurchasePrice * txtQuantity.Value.Value * discountPercent / 100)).ToString();
        }

        private void cbSalesPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0) {
                var personObj = ((System.Data.DataRowView)(((object[])(e.AddedItems))[0])).Row.ItemArray;
                this.txtSalesName.Text = personObj[2].ToString();//Person Name
                this.txtSalesContact.Text = personObj[3].ToString();//Person Contact
                salePersonId = int.Parse(personObj[0].ToString());//PersonId
                salePersonEmail = Convert.ToString(personObj[5]);//Sales Email
            }
        }
    }
}
