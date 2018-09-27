using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElectronicZone.Wpf.DataAccessLayer;

namespace ElectronicZone.Wpf.Utility
{
    public class PaymentTransaction
    {
        public enum PaymentStatus
        {
            PENDING_PAYMENT,
            PURCHASE_PAYMENT,
            SALE_PAYMENT,
            SUPPORT_PAYMENT,
            SALEREVERSAL_PAYMENT
        }

        public bool AddPaymentTransaction(int userId, double amount, PaymentStatus paymentStatus, int transactionId)
        {
            Dictionary<string, string> paymentModel = new Dictionary<string, string>();
            //paymentM.Add("Id", null);
            paymentModel.Add("UserId", userId.ToString());
            paymentModel.Add("Cr", (paymentStatus != PaymentStatus.PURCHASE_PAYMENT ? amount.ToString() : "0"));// include pending, sale and support payments
            paymentModel.Add("Dr", (paymentStatus == PaymentStatus.PURCHASE_PAYMENT ? amount.ToString() : "0"));// include Purchase payment
            paymentModel.Add("Status", paymentStatus.ToString());
            paymentModel.Add("Description", GetPaymentDescription(transactionId, paymentStatus));
            paymentModel.Add("Remarks", "");
            paymentModel.Add("CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            DataAccess dataAccess = new DataAccess();
            int status = dataAccess.InsertPaymentMaster(paymentModel, "tblPaymentMaster");
            if (status == 1)
                return true;
            else
                return false;
        }

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
            desc = string.Format("{0} from transactionId : {1}", paymentType, transId.ToString());
            return desc;
        }
    }
}
