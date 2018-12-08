using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility.EMail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace ElectronicZone.Wpf.Utility
{
    public class SaleManager
    {
        ILogger logger = new Logger(typeof(SaleManager));
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
                    if (_sale.Contact.Id == 0)
                        _sale.Contact.Id = CreateSalePersonForSale(da, _sale.Contact);
                    if (_sale.Contact.Id > 0) {
                        double total = _sale.Quantity * _sale.Price;
                        bool isPending = (total > _sale.AmountPaid ? true : false);
                        double pendingAmt = total - _sale.AmountPaid;
                        #region Create Sales Master Object
                        //Add SaleMaster
                        Dictionary<string, string> saleMaster = new Dictionary<string, string>();
                        saleMaster.Add("Id", _sale.Id == 0 ? null : _sale.Id.ToString());
                        saleMaster.Add("StockId", _sale.StockId.ToString());
                        saleMaster.Add("SalePersonId", _sale.Contact.Id.ToString());
                        saleMaster.Add("Quantity", _sale.Quantity.ToString());
                        saleMaster.Add("Price", _sale.Price.ToString());
                        saleMaster.Add("Total", _sale.Total.ToString());
                        saleMaster.Add("AmountPaid", _sale.AmountPaid.ToString());
                        //saleMasterModel.Add("Pending", pendingAmt.ToString());
                        saleMaster.Add("SaleDate", _sale.SaleDate.ToString(ConfigurationManager.AppSettings["DateOnly"]));
                        saleMaster.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"])); 
                        #endregion
                        salesOrderId = da.InsertOrUpdateSaleMaster(saleMaster, "tblSaleMaster");
                        if (salesOrderId > 0 && _sale.Id == 0) {
                            #region Add Sales Invoice
                            // Adding Sales Invoice
                            Dictionary<string, string> invoice = new Dictionary<string, string>();
                            invoice.Add("Id", null);
                            invoice.Add("SalesId", salesOrderId.ToString());
                            invoice.Add("InvoiceNumber", CommonMethods.GenerateInvoice(salesOrderId, _sale.SaleDate));
                            da.InsertOrUpdateInvoiceMaster(invoice, "tblInvoiceMaster"); 
                            #endregion

                            #region Add Pending Payment
                            if (isPending) {
                                PendingPayment pendingPayment = new PendingPayment() {
                                    SaleId = salesOrderId, SalePersonId = _sale.Contact.Id, PendingAmount = pendingAmt
                                    , IsDiscount = _sale.IsDiscounted
                                };
                                CreatePendingPayment(pendingPayment, da);
                            }
                            #endregion

                            #region Add Payment Transaction
                            // add payment transaction
                            PaymentTransaction paymentTransaction = new PaymentTransaction();
                            bool paymentStatus = paymentTransaction.AddPaymentTransaction(Global.UserId, _sale.AmountPaid, CommonEnum.PaymentStatus.SALE_PAYMENT, salesOrderId, da);
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
                                    SendMailService ms = new SendMailService();
                                    ms.SendSalesOrderCreateMail(_sale);

                                    MessageBoxResult result = MessageBox.Show("Sale Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            Dictionary<string, string> spModel = new Dictionary<string, string>();
            spModel.Add("Id", null);
            spModel.Add("Title", "");
            spModel.Add("Name", contact.Name);
            spModel.Add("Contact", contact.PrimaryContact);
            spModel.Add("AlternateContact", "");
            spModel.Add("Email", contact.Email);
            spModel.Add("Address", "");
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
            Dictionary<string, string> pendingPmt = new Dictionary<string, string>();
            pendingPmt.Add("Id", null);
            pendingPmt.Add("SaleId", pendingPayment.SaleId.ToString());
            pendingPmt.Add("SalePersonId", pendingPayment.SalePersonId.ToString());
            pendingPmt.Add("PendingAmount", pendingPayment.PendingAmount.ToString());
            // If Discounted Payment Then Paid the amt with IsDiscount=true, PaidAmt
            if (pendingPayment.IsDiscount) {
                pendingPmt.Add("IsPaid", "1"); pendingPmt.Add("IsDiscount", pendingPayment.IsDiscount.ToString());
                pendingPmt.Add("PaidAmount", "0"/*pendingPayment.PaidAmount.ToString()*/);
                pendingPmt.Add("PaidDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateOnly"]));
                //ConfigurationManager.AppSettings["DateOnly"]
            }
            int pendingPaymentId = da.InsertOrUpdatePendingPayment(pendingPmt, "tblPendingPayment");
            return pendingPaymentId;
        }

        /// <summary>
        /// Clear Pending Payment :
        /// 1. Update Pending Payment
        /// 2. Update Sale Amount
        /// 3. Add Payment Transaction
        /// </summary>
        /// <param name="payment"></param>
        public void ClearPendingPayment(PendingPayment payment)
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
                    smModel.Add("AmountPaid", payment.PaidAmount.ToString());
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
