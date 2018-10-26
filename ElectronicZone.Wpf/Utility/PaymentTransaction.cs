using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ElectronicZone.Wpf.DataAccessLayer;
using static ElectronicZone.Wpf.Utility.CommonEnum;
using System.Configuration;

namespace ElectronicZone.Wpf.Utility
{
    public class PaymentTransaction
    {
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
            paymentModel.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
            DataAccess dataAccess = new DataAccess();
            int status = dataAccess.InsertPaymentMaster(paymentModel, "tblPaymentMaster");
            if (status == 1)
                return true;
            else
                return false;
        }

        public bool ReversePaymentTransaction(int userId, double amount, PaymentStatus paymentStatus, int transactionId)
        {
            Dictionary<string, string> paymentModel = new Dictionary<string, string>();
            //paymentM.Add("Id", null);
            paymentModel.Add("UserId", userId.ToString());
            paymentModel.Add("Cr", (paymentStatus == PaymentStatus.PURCHASEREVERSAL_PAYMENT ? amount.ToString() : "0"));// Credit For Purchase Reversal
            paymentModel.Add("Dr", (paymentStatus == PaymentStatus.SALEREVERSAL_PAYMENT ? amount.ToString() : "0"));// Debit For Sale Reversal
            paymentModel.Add("Status", paymentStatus.ToString());
            paymentModel.Add("Description", GetPaymentDescription(transactionId, paymentStatus));
            paymentModel.Add("Remarks", "");
            paymentModel.Add("CreatedDate", DateTime.Now.ToString(ConfigurationManager.AppSettings["DateTimeFormat"]));
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
            desc = string.Format($"{paymentType} from transactionId : {transId.ToString()}");
            return desc;
        }
    }
}
