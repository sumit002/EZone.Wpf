using ElectronicZone.Wpf.DataAccessLayer;
using ElectronicZone.Wpf.Model;
using ElectronicZone.Wpf.Utility.EMail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static ElectronicZone.Wpf.Utility.CommonEnum;

namespace ElectronicZone.Wpf.Utility
{
    public sealed class PurchaseManager
    {
        #region Properties
        ILogger _logger = new Logger(typeof(SaleManager));
        public int purchaseOrderId { get; private set; }
        private string _tableToAttach = "tblStockMaster";
        #endregion

        public PurchaseManager()
        { }

        /// <summary>
        /// Get All Purchases/Stocks
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllStocks() {
            using (DataAccess da = new DataAccess()) {
                return da.GetAllStocks();
            }
        }

        /// <summary>
        /// Create Purchase Order :
        /// 1. Create Purchase Order
        /// 2. Add payment transaction
        /// </summary>
        /// <param name="_purchase"></param>
        /// <returns></returns>
        public int CreateOrUpdatePurchaseOrder(Purchase _purchase) {
            using (DataAccess da = new DataAccess())
            {
                try
                {
                    System.Collections.Generic.Dictionary<string, string> fields = new System.Collections.Generic.Dictionary<string, string>();
                    fields.Add("Id", _purchase.Id == 0 ? null : _purchase.Id.ToString());
                    fields.Add("ProductId", _purchase.ProductId.ToString());
                    fields.Add("BrandId", _purchase.BrandId.ToString());
                    fields.Add("ProductCode", _purchase.ProductCode);
                    fields.Add("StockCode", _purchase.StockCode);
                    fields.Add("ItemDesc", _purchase.ItemDesc);
                    fields.Add("Quantity", Convert.ToString(_purchase.Quantity));
                    fields.Add("AvlQuantity", Convert.ToString(_purchase.AvlQuantity));
                    fields.Add("PurchasePrice", _purchase.PurchasePrice.ToString());
                    fields.Add("SalePrice", _purchase.SalePrice.ToString());//PurchaseProfitPercent //folderFields.Add("ProductImage", System.Text.Encoding.UTF8.GetString(imgByteArr));
                    fields.Add("PurchaseDate", _purchase.PurchaseDate.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
                    fields.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
                    fields.Add("ModifiedDate", _purchase.Id == 0 ? null : DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));

                    purchaseOrderId = da.InsertOrUpdateStockMaster(fields, _tableToAttach);
                    if (purchaseOrderId > 0) {
                        // adding Product Image
                        //if (!string.IsNullOrEmpty(imageName))
                        //{
                        //    //Initialize a file stream to read the image file
                        //    using (FileStream fs = new FileStream(@imageName, FileMode.Open, FileAccess.Read))
                        //    {
                        //        //Initialize a byte array with size of stream
                        //        imgByteArr = new byte[fs.Length];
                        //        //Read data from the file stream and put into the byte array
                        //        fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));
                        //    }
                        //    da.UpdateStockImage(imgByteArr, rslt, _tableToAttach);
                        //}
                        // add payment transaction
                        PaymentTransaction paymentTransaction = new PaymentTransaction();
                        bool paymentStatus = paymentTransaction.AddPaymentTransaction(Global.UserId, _purchase.PurchasePrice * _purchase.Quantity, PaymentStatus.PURCHASE_PAYMENT, purchaseOrderId, da);
                        if (paymentStatus)
                        {
                            SendMailService ms = new SendMailService();
                            ms.SendPurchaseOrderCreateMail(new Purchase() { Id = purchaseOrderId
                                , TotalPurchasePrice = _purchase.PurchasePrice * _purchase.Quantity
                                , PurchaseDate = _purchase.PurchaseDate
                            });
                            MessageBoxResult result = MessageBox.Show("Stock Updated Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else {
                        MessageBoxResult result = MessageBox.Show("Error While Adding Stock!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                    da.RollbackTransaction();
                }
            }

            return purchaseOrderId;
        }

        /// <summary>
        /// Delete Purchase Order :
        /// 1. Delete order
        /// 2. Reverse Payment
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int DeletePurchaseOrder(Purchase obj) {
            int stockId = 0;
            using (DataAccess da = new DataAccess()) {
                try {
                    stockId = da.DeleteStock(obj.Id);
                    // Reverse Payment Transaction
                    PaymentTransaction paymentTransaction = new PaymentTransaction();
                    bool isReversed = paymentTransaction.ReversePaymentTransaction(Global.UserId, obj.TotalPurchasePrice, CommonEnum.PaymentStatus.PURCHASEREVERSAL_PAYMENT, obj.Id, da);
                    if (isReversed)
                    {
                        MessageBoxResult result = MessageBox.Show("Stock Deleted Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                    da.RollbackTransaction();
                }
            }
            return stockId;
        }
    }
}
