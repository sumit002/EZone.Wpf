using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Utility;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private int pendingPaymentId = 0;
        public ClearPending(object[] pending)
        {
            InitializeComponent();
            //this.txtPendingAmt.IsReadOnly = true;
            this.txtPaidAmount.IsReadOnly = true;
            LoadPendingData(pending);
        }

        private void LoadPendingData(object[] pending)
        {
            try
            {
                this.lblPendingAmount.Content = pending[10].ToString();
                this.txtPaidAmount.Text = pending[10].ToString();
                this.lblSalesPerson.Content = string.Format("{0}({1})", pending[4].ToString(), pending[5].ToString());
                this.lblSalesPerson.ToolTip = pending[7].ToString();
                this.lblBrandProduct.Content = string.Format("{0} {1} ({2})", pending[16].ToString(), pending[15].ToString(), pending[17].ToString());
                this.lblTotalAndPaid.Content = string.Format("{0}/{1}", pending[8].ToString(), pending[9].ToString());
                this.lblSaleId.Content = pending[1].ToString();
                this.lblSalePersonId.Content = pending[2].ToString();

                pendingPaymentId = int.Parse(pending[0].ToString());
                minAmtToAvailDiscount = double.Parse(pending[23].ToString());
                this.dpPaymentDate.DisplayDateStart = DateTime.Parse(pending[11].ToString());
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void ClearPendingPayment()
        {
            #region add pending payment
            Dictionary<string, string> pendingPaymentModel = new Dictionary<string, string>();
            pendingPaymentModel.Add("Id", pendingPaymentId.ToString());
            pendingPaymentModel.Add("SaleId", lblSaleId.Content.ToString());
            pendingPaymentModel.Add("SalePersonId", lblSalePersonId.Content.ToString());
            pendingPaymentModel.Add("PendingAmount", lblPendingAmount.Content.ToString());
            pendingPaymentModel.Add("IsPaid", "1");// true
            pendingPaymentModel.Add("PaidDate", (DateTime.Parse(dpPaymentDate.Text).ToString("yyyy-MM-dd HH:mm:ss")));
            pendingPaymentModel.Add("IsDiscount", chkbDiscount.IsChecked.Value == true ? "1" : "0");
            pendingPaymentModel.Add("PaidAmount", this.txtPaidAmount.Text);//discounted payment shd more than purchase total amount
            DataAccess dataAccess = new DataAccess();
            int pendingRowId = dataAccess.InsertOrUpdatePendingPayment(pendingPaymentModel, "tblPendingPayment");
            if (pendingRowId > 0)
            {
                // add payment transaction
                PaymentTransaction paymentTransaction = new PaymentTransaction();
                bool paymentStatus = paymentTransaction.AddPaymentTransaction(1, double.Parse(this.txtPaidAmount.Text), PaymentTransaction.PaymentStatus.PENDING_PAYMENT, pendingRowId);
                if (paymentStatus)
                {
                    MessageBoxResult result = MessageBox.Show("Payment Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Error While Updating Payments!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Error While Updating Pendings!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            #endregion
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validatePending())
                {
                    ClearPendingPayment();
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
        /// Mandatory validation : Paid Amount , PaymentDate
        /// </summary>
        /// <returns></returns>
        private bool validatePending()
        {
            //int avlQty = int.Parse(this.lblAvlQuantity.Content.ToString());
            if (string.IsNullOrEmpty(txtPaidAmount.Text.Trim()))
                return false;
            else if (string.IsNullOrEmpty(dpPaymentDate.Text.Trim()))
                return false;
            else if (double.Parse(txtPaidAmount.Text) <= minAmtToAvailDiscount)
                return false;
            else
                return true;
        }

        private void chkbDiscount_Checked(object sender, RoutedEventArgs e)
        {
            availDiscount(chkbDiscount.IsChecked.Value);
        }

        private void availDiscount(bool status)
        {
            this.txtPaidAmount.IsReadOnly = !status;
        }

        private void chkbDiscount_Unchecked(object sender, RoutedEventArgs e)
        {
            availDiscount(chkbDiscount.IsChecked.Value);
        }
    }
}
