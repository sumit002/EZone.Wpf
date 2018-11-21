using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElectronicZone.Wpf.DataAccessLayer;
using static ElectronicZone.Wpf.Utility.CommonEnum;
using System.Configuration;

namespace ElectronicZone.Wpf.Utility
{
    /// <summary>
    /// Payment Transaction Handler Class
    /// </summary>
    public class PaymentTransaction
    {
        /// <summary>
        /// Adds Payment Transaction
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <param name="paymentStatus"></param>
        /// <param name="transactionId"></param>
        /// <param name="da"></param>
        /// <returns></returns>
        public bool AddPaymentTransaction(int userId, double amount, PaymentStatus paymentStatus, int transactionId, DataAccess da)
        {
            // using (DataAccess da = new DataAccess()) {
            Dictionary<string, string> paymentModel = new Dictionary<string, string>();
            //paymentM.Add("Id", null);
            paymentModel.Add("UserId", userId.ToString());
            paymentModel.Add("Cr", (paymentStatus != PaymentStatus.PURCHASE_PAYMENT ? amount.ToString() : "0"));// include pending, sale and support payments
            paymentModel.Add("Dr", (paymentStatus == PaymentStatus.PURCHASE_PAYMENT ? amount.ToString() : "0"));// include Purchase payment
            paymentModel.Add("Status", paymentStatus.ToString());
            paymentModel.Add("Description", GetPaymentDescription(transactionId, paymentStatus));
            paymentModel.Add("Remarks", "");
            // ToDo : Check Created date for Purchase order create shd be Purchase Date instead of today's date
            paymentModel.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
            int status = da.InsertPaymentMaster(paymentModel, "tblPaymentMaster");
            if (status == 1)
                return true;
            else
                return false;
            // }
        }

        /// <summary>
        /// Reverses Payment Transaction
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <param name="paymentStatus"></param>
        /// <param name="transactionId"></param>
        /// <param name="da"></param>
        /// <returns></returns>
        public bool ReversePaymentTransaction(int userId, double amount, PaymentStatus paymentStatus, int transactionId, DataAccess da)
        {
            //using (DataAccess da = new DataAccess()) {
            Dictionary<string, string> paymentModel = new Dictionary<string, string>();
            //paymentM.Add("Id", null);
            paymentModel.Add("UserId", userId.ToString());
            paymentModel.Add("Cr", (paymentStatus == PaymentStatus.PURCHASEREVERSAL_PAYMENT ? amount.ToString() : "0"));// Credit For Purchase
            paymentModel.Add("Dr", (paymentStatus != PaymentStatus.PURCHASEREVERSAL_PAYMENT ? amount.ToString() : "0"));// Debit For Sale,Support payment Reversal
            paymentModel.Add("Status", paymentStatus.ToString());
            paymentModel.Add("Description", GetPaymentDescription(transactionId, paymentStatus));
            paymentModel.Add("Remarks", "");
            paymentModel.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
            int status = da.InsertPaymentMaster(paymentModel, "tblPaymentMaster");
            if (status == 1)
                return true;
            else
                return false;
            //}
        }

        /// <summary>
        /// Constructs Payment Description
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        private string GetPaymentDescription(int transId, PaymentStatus ps)
        {
            string desc = string.Empty;
            string paymentType = string.Empty;
            if(ps.ToString().Equals("PENDING_PAYMENT"))
                paymentType = "Pending";
            else if(ps.ToString().Equals("PURCHASE_PAYMENT"))
                paymentType = "Purchase";
            else if(ps.ToString().Equals("SALE_PAYMENT"))
                paymentType = "Sale";
            else
                paymentType = "";
            // construct the description string
            desc = string.Format($"{paymentType} from transId : {transId.ToString()}");
            return desc;
        }
    }
}
