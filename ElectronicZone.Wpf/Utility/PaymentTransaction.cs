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
        private static string GetPaymentDescription(int transId, PaymentStatus ps)
        {
            string _paymentType = string.Empty;
            if(ps.Equals(PaymentStatus.PENDING_PAYMENT))
                _paymentType = "Pending";
            else if(ps.Equals(PaymentStatus.PURCHASE_PAYMENT) || ps.Equals(PaymentStatus.PURCHASEREVERSAL_PAYMENT))
                _paymentType = "Purchase";
            else if(ps.Equals(PaymentStatus.SALE_PAYMENT) || ps.Equals(PaymentStatus.SALEREVERSAL_PAYMENT))
                _paymentType = "Sale";
            else if (ps.Equals(PaymentStatus.SUPPORT_PAYMENT) || ps.Equals(PaymentStatus.SUPPORTREVERSAL_PAYMENT))
                _paymentType = "Support";
            else
                _paymentType = "";
            return $"{_paymentType} from transId : {Convert.ToString(transId)}";
        }
    }
}
