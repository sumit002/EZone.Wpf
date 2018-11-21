using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ElectronicZone.Wpf.Utility
{
    public class SaleManager
    {
        ILogger logger = new Logger(typeof(SaleManager));
        private int salePersonId = 0;
        public SaleManager()
        {

        }

        /// <summary>
        /// Create Sales Order : 
        /// 1. Create Sale Person If New
        /// 2. Add SaleMaster
        /// 3. Add Sales Invoice
        /// 4. Add Pending Payment if any
        /// 5. Add payment transaction
        /// 6. Update Stock
        /// </summary>
        /// <returns></returns>
        public int CreateSalesOrder(Sale _sale) {
            int salesOrderId = 0;
            using (DataAccess da = new DataAccess())
            {
                try
                {
                    // add Sale Contact
                    if (salePersonId == 0)
                        salePersonId = CreateSalePersonForSale(da, _sale.Contact);
                    if (salePersonId > 0) {
                        double total = _sale.Quantity * _sale.Price;
                        bool isPending = (total > _sale.AmountPaid ? true : false);
                        double pendingAmt = total - _sale.AmountPaid;
                        #region Create Sales Master Object
                        //Add SaleMaster
                        Dictionary<string, string> saleMasterModel = new Dictionary<string, string>();
                        saleMasterModel.Add("Id", _sale.Id == 0 ? null : _sale.Id.ToString());
                        saleMasterModel.Add("StockId", _sale.StockId.ToString());
                        saleMasterModel.Add("SalePersonId", salePersonId.ToString());
                        saleMasterModel.Add("Quantity", _sale.Quantity.ToString());
                        saleMasterModel.Add("Price", _sale.Price.ToString());
                        saleMasterModel.Add("Total", _sale.Total.ToString());
                        saleMasterModel.Add("AmountPaid", _sale.AmountPaid.ToString());
                        //saleMasterModel.Add("Pending", pendingAmt.ToString());
                        saleMasterModel.Add("SaleDate", _sale.SaleDate.ToString(ConfigurationManager.AppSettings["DateOnly"]));
                        saleMasterModel.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"])); 
                        #endregion
                        int rslt = salesOrderId = da.InsertOrUpdateSaleMaster(saleMasterModel, "tblSaleMaster");
                        if (rslt > 0)
                        {
                            #region Add Sales Invoice
                            // Adding Sales Invoice
                            Dictionary<string, string> saleInvoiceMasterModel = new Dictionary<string, string>();
                            saleInvoiceMasterModel.Add("Id", null);
                            saleInvoiceMasterModel.Add("SalesId", salesOrderId.ToString());
                            saleInvoiceMasterModel.Add("InvoiceNumber", CommonMethods.GenerateInvoice(salesOrderId, _sale.SaleDate));
                            da.InsertOrUpdateInvoiceMaster(saleMasterModel, "tblInvoiceMaster"); 
                            #endregion

                            #region Add Pending Payment
                            if (isPending) {
                                PendingPayment pendingPayment = new PendingPayment() {
                                    SaleId = salesOrderId, SalePersonId = salePersonId, PendingAmount = pendingAmt
                                };
                                CreatePendingPayment(pendingPayment, da);
                            }
                            #endregion

                            #region Add Payment Transaction
                            // add payment transaction
                            PaymentTransaction paymentTransaction = new PaymentTransaction();
                            bool paymentStatus = paymentTransaction.AddPaymentTransaction(Global.UserId, _sale.AmountPaid, CommonEnum.PaymentStatus.SALE_PAYMENT, rslt, da);
                            if (paymentStatus)
                            {
                                #region Update Stock Quantity
                                Dictionary<string, string> stockModel = new Dictionary<string, string>();
                                stockModel.Add("Id", _sale.StockId.ToString());
                                stockModel.Add("AvlQuantity", _sale.Quantity.ToString());
                                stockModel.Add("ModifiedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
                                int isUpdated = da.UpdateStockQuantity(stockModel, "tblStockMaster");
                                #endregion
                                if (isUpdated == 1)
                                {
                                    MessageBoxResult result = MessageBox.Show("Sale Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                    //this.Close();
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
                            #endregion
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Error While Adding Sale!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        //return rslt;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                    da.RollbackTransaction();
                }
            }
            return salesOrderId;
        }
        private int CreateSalePersonForSale(DataAccess da, Contact contact)
        {
            //create record
            Dictionary<string, string> spModel = new Dictionary<string, string>();
            spModel.Add("Id", null);
            spModel.Add("Title", "");
            spModel.Add("Name", contact.Name);
            spModel.Add("Contact", contact.PrimaryContact);
            spModel.Add("AlternateContact", "");
            spModel.Add("Email", "");
            spModel.Add("Address", "");
            //using (DataAccess dataAccess = new DataAccess())
            int personId = da.InsertOrUpdateSalePerson(spModel, "tblSalePerson");
            if (personId > 0)
                return personId;
            else
                return 0;
        }
        
        /// <summary>
        /// Reverse SalesOrder
        /// ---------------------------------
        /// 1. Remove Sale from SaleMaster
        /// 2. Reverse Pending Payment if any
        /// 3. Reverse payment transaction
        /// 4. Reverse Stock
        /// </summary>
        /// <returns></returns>
        public bool ReverseSalesOrder(Sale Obj) {
            using (DataAccess da = new DataAccess()) {
                try {
                    // Remove Sale from SaleMaster
                    int stockId = da.DeleteSalesOrder(Obj.Id);
                    // Reverse payment transaction
                    PaymentTransaction paymentTransaction = new PaymentTransaction();
                    bool isReversed = paymentTransaction.ReversePaymentTransaction(Global.UserId, Obj.AmountPaid, CommonEnum.PaymentStatus.SALEREVERSAL_PAYMENT, Obj.Id, da);
                    // Reverse Stock
                    Dictionary<string, string> stockModel = new Dictionary<string, string>();
                    stockModel.Add("Id", Obj.StockId.ToString());
                    stockModel.Add("AvlQuantity", Obj.Quantity.ToString());
                    stockModel.Add("ModifiedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
                    int isUpdated = da.UpdateStockQuantity(stockModel, "tblStockMaster", true);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                    da.RollbackTransaction();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Create Pending Payment If Any
        /// </summary>
        /// <param name="pendingPayment"></param>
        /// <param name="da"></param>
        /// <returns></returns>
        private int CreatePendingPayment(PendingPayment pendingPayment, DataAccess da)
        {
            Dictionary<string, string> pendingPaymentModel = new Dictionary<string, string>();
            pendingPaymentModel.Add("Id", null);
            pendingPaymentModel.Add("SaleId", pendingPayment.SaleId.ToString());
            pendingPaymentModel.Add("SalePersonId", pendingPayment.SalePersonId.ToString());
            pendingPaymentModel.Add("PendingAmount", pendingPayment.PendingAmount.ToString());
            //pendingPaymentModel.Add("IsPaid", "0");
            int pendingPaymentId = da.InsertOrUpdatePendingPayment(pendingPaymentModel, "tblPendingPayment");
            return pendingPaymentId;
        }

        /// <summary>
        /// Clear Pending Payment :
        /// 1. Update Pending Payment
        /// 2. Update Sale Amount
        /// 3. Add Payment Transaction
        /// </summary>
        /// <param name="payment"></param>
        private void ClearPendingPayment(PendingPayment payment)
        {
            using (DataAccess da = new DataAccess())
            {
                #region Update pending payment
                Dictionary<string, string> pendingPaymentModel = new Dictionary<string, string>();
                pendingPaymentModel.Add("Id", payment.Id.ToString());
                pendingPaymentModel.Add("SaleId", payment.SaleId.ToString());
                pendingPaymentModel.Add("SalePersonId", payment.SalePersonId.ToString());
                pendingPaymentModel.Add("PendingAmount", payment.PendingAmount.ToString());
                pendingPaymentModel.Add("IsPaid", payment.IsPaid.ToString());// true
                pendingPaymentModel.Add("PaidDate", payment.PaidDate.ToString(ConfigurationManager.AppSettings["DateOnly"]));
                pendingPaymentModel.Add("IsDiscount", payment.IsDiscount.ToString());
                //Note : Discounted payment shd more than purchase total amount
                pendingPaymentModel.Add("PaidAmount", payment.PaidAmount.ToString());
                int pendingPaymentId = da.InsertOrUpdatePendingPayment(pendingPaymentModel, "tblPendingPayment");
                if (pendingPaymentId > 0)
                {
                    #region Update Sale PaidAmount
                    // update Sale
                    Dictionary<string, string> smModel = new Dictionary<string, string>();
                    smModel.Add("Id", payment.SaleId.ToString());
                    smModel.Add("AmountPaid", payment.PendingAmount.ToString());
                    smModel.Add("ModifiedDate", payment.PaidDate.ToString(ConfigurationManager.AppSettings["DateOnly"]));
                    da.InsertOrUpdateSaleMaster(smModel, "tblSaleMaster");
                    #endregion

                    #region Add Payment Transaction
                    // add payment transaction
                    PaymentTransaction pt = new PaymentTransaction();
                    bool paymentStatus = pt.AddPaymentTransaction(Global.UserId, payment.PaidAmount, CommonEnum.PaymentStatus.PENDING_PAYMENT, pendingPaymentId, da);
                    if (paymentStatus)
                    {
                        MessageBoxResult result = MessageBox.Show("Payment Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        //this.Close();
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Error While Updating Payments!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    #endregion
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Error While Updating Pendings!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                #endregion
            }
        }
    }
}
