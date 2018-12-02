using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using System;
using System.Configuration;
using System.Windows;

namespace ElectronicZone.Wpf.View.Payment
{
    /// <summary>
    /// Interaction logic for ClearPending.xaml
    /// </summary>
    public partial class ClearPending : MetroWindow
    {
        ILogger logger = new Logger(typeof(ClearPending));
        private double minAmtToAvailDiscount = 0;
        //private int pendingPaymentId = 0;
        private Model.PendingPayment _pendingPayment;


        public ClearPending(Model.PendingPayment pending)
        {
            InitializeComponent();
            //this.txtPendingAmt.IsReadOnly = true;
            this.txtPaidAmount.IsReadOnly = true;
            this._pendingPayment = pending;
            LoadPendingData(pending);
        }

        private void LoadPendingData(Model.PendingPayment obj)
        {
            try
            {
                this.lblPendingAmount.Content = obj.PendingAmount.ToString(); //pending[10].ToString();
                this.txtPaidAmount.Value = obj.Total;
                this.lblSalesPerson.Content = obj.SalePersonToDisplay;// string.Format("{0}({1})", pending[4].ToString(), pending[5].ToString());
                //this.lblSalesPerson.ToolTip = pending[7].ToString();
                //this.lblBrandProduct.Content = string.Format("{0} {1} ({2})", pending[16].ToString(), pending[15].ToString(), pending[17].ToString());
                this.lblTotal.Content = obj.Total.ToString(ConfigurationManager.AppSettings["AmountDisplayPattern"]);
                this.lblPaidAmount.Content = obj.PaidAmount.ToString(ConfigurationManager.AppSettings["AmountDisplayPattern"]);
                
                this.txtPaidAmount.Minimum = obj.MinAmountForDiscount;
                this.txtPaidAmount.Maximum = obj.PendingAmount;
                minAmtToAvailDiscount = obj.MinAmountForDiscount;
                this.dpPaymentDate.DisplayDateStart = obj.SaleDate;
                this.dpPaymentDate.SelectedDate = DateTime.Today;

                //this.lblSaleId.Content = pending[1].ToString();
                //this.lblSalePersonId.Content = pending[2].ToString();
                //pendingPaymentId = int.Parse(pending[0].ToString());
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        //private void ClearPendingPayment()
        //{
        //    using (DataAccess da = new DataAccess()) {
        //        #region add pending payment
        //        Dictionary<string, string> pendingPaymentModel = new Dictionary<string, string>();
        //        //pendingPaymentModel.Add("Id", pendingPaymentId.ToString());
        //        //pendingPaymentModel.Add("SaleId", lblSaleId.Content.ToString());
        //        //pendingPaymentModel.Add("SalePersonId", lblSalePersonId.Content.ToString());
        //        pendingPaymentModel.Add("PendingAmount", lblPendingAmount.Content.ToString());
        //        pendingPaymentModel.Add("IsPaid", "1");// true
        //        pendingPaymentModel.Add("PaidDate", (DateTime.Parse(dpPaymentDate.Text).ToString(ConfigurationManager.AppSettings["DateTimeFormat"])));
        //        pendingPaymentModel.Add("IsDiscount", chkbDiscount.IsChecked.Value == true ? "1" : "0");
        //        pendingPaymentModel.Add("PaidAmount", this.txtPaidAmount.Value.Value.ToString());//discounted payment shd more than purchase total amount
        //                                                                       //DataAccess dataAccess = new DataAccess();
        //        int pendingRowId = da.InsertOrUpdatePendingPayment(pendingPaymentModel, "tblPendingPayment");
        //        if (pendingRowId > 0)
        //        {
        //            // add payment transaction
        //            PaymentTransaction paymentTransaction = new PaymentTransaction();
        //            bool paymentStatus = paymentTransaction.AddPaymentTransaction(Global.UserId, this.txtPaidAmount.Value.Value, CommonEnum.PaymentStatus.PENDING_PAYMENT, pendingRowId, da);
        //            if (paymentStatus)
        //            {
        //                MessageBoxResult result = MessageBox.Show("Payment Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        //                this.Close();
        //            }
        //            else
        //            {
        //                MessageBoxResult result = MessageBox.Show("Error While Updating Payments!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //        else
        //        {
        //            MessageBoxResult result = MessageBox.Show("Error While Updating Pendings!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //        #endregion
        //    }
        //}

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidatePendingPayment())
                {
                    Model.PendingPayment pPayment = new Model.PendingPayment() {
                        Id = _pendingPayment.Id,
                        SaleId = _pendingPayment.SaleId,// Convert.ToInt32(lblSaleId.Content),
                        SalePersonId = _pendingPayment.SalePersonId,// Convert.ToInt32(lblSalePersonId.Content),
                        PendingAmount = Convert.ToDouble(lblPendingAmount.Content),
                        IsPaid = true,
                        PaidDate = DateTime.Parse(dpPaymentDate.Text),
                        IsDiscount = chkbDiscount.IsChecked.Value,
                        PaidAmount = this.txtPaidAmount.Value.Value
                    };

                    SaleManager sm = new SaleManager();
                    sm.ClearPendingPayment(pPayment);
                    // ClearPendingPayment();
                    this.Close();
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
        /// <summary>
        /// Validate Pending Payment : Paid Amount , PaymentDate
        /// </summary>
        /// <returns></returns>
        private bool ValidatePendingPayment()
        {
            if (!txtPaidAmount.Value.HasValue)
                return false;
            else if (!dpPaymentDate.SelectedDate.HasValue)
                return false;
            else if (txtPaidAmount.Value.Value <= minAmtToAvailDiscount)
                return false;
            else
                return true;
        }

        private void chkbDiscount_Checked(object sender, RoutedEventArgs e)
        {
            ToggleAvailDiscountOption(chkbDiscount.IsChecked.Value);
        }

        /// <summary>
        /// Active/Deactive Discount
        /// </summary>
        /// <param name="status"></param>
        private void ToggleAvailDiscountOption(bool status)
        {
            this.txtPaidAmount.IsReadOnly = !status;
        }

        private void chkbDiscount_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleAvailDiscountOption(chkbDiscount.IsChecked.Value);
        }
    }
}
