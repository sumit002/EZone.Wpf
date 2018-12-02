using ElectronicZone.Wpf.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace ElectronicZone.Wpf.Utility.EMail
{
    public class SendMailService
    {
        public void SendSupportIncomeMail(SupportIncome obj)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmailForSupportIncome"])) {
                // Placeholders for sending emails
                Dictionary<string, string> placeHolders = new Dictionary<string, string>();
                placeHolders.Add("{AmountEarned}", obj.Amount.ToString("00.00", CultureInfo.InvariantCulture));
                placeHolders.Add("{SupportIncomeDate}", obj.SupportDate.ToString(ConfigurationManager.AppSettings["DateDisplay"]));
                placeHolders.Add("{Description}", obj.Description);

                EmailParams emailParams = new EmailParams(ConfigurationManager.AppSettings["AdminEmail"], SendEmailType.SupportIncomeMail, placeHolders, ccMail: string.Empty, mailAttachments: string.Empty);
                EmailUtility.SendMail(emailParams);
            }
        }

        public void SendPurchaseOrderCreateMail(Purchase obj)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmailForPurchaseOrder"]))
            {
                // Placeholders for sending emails
                Dictionary<string, string> placeHolders = new Dictionary<string, string>();
                placeHolders.Add("{UserLoginName}", Global.Name);
                placeHolders.Add("{PurchaseOrderId}", obj.Id.ToString(ConfigurationManager.AppSettings["InvoiceIdPattern"]));
                placeHolders.Add("{TotalPurchasePrice}", obj.TotalPurchasePrice.ToString(ConfigurationManager.AppSettings["AmountDisplayPattern"]));
                placeHolders.Add("{PurchaseDate}", obj.PurchaseDate.ToString(ConfigurationManager.AppSettings["DateDisplay"]));
                placeHolders.Add("{TollFreeContactNumber}", ConfigurationManager.AppSettings["TollFreeContactNumber"]);

                EmailParams emailParams = new EmailParams(ConfigurationManager.AppSettings["AdminEmail"], SendEmailType.PurchaseOrderMail, placeHolders, ccMail: string.Empty, mailAttachments: string.Empty);
                EmailUtility.SendMail(emailParams);
            }
        }

        public void SendSalesOrderCreateMail(Sale obj)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmailForSalesOrder"]) && obj.Contact != null && !string.IsNullOrEmpty(obj.Contact.Email))
            {
                // Placeholders for sending emails
                Dictionary<string, string> placeHolders = new Dictionary<string, string>();
                placeHolders.Add("{UserLoginName}", Global.Name);
                placeHolders.Add("{SalesOrderId}", obj.Id.ToString(ConfigurationManager.AppSettings["InvoiceIdPattern"]));
                placeHolders.Add("{TollFreeContactNumber}", ConfigurationManager.AppSettings["TollFreeContactNumber"]);

                EmailParams emailParams = new EmailParams(obj.Contact.Email, SendEmailType.SalesOrderMail, placeHolders, ccMail: string.Empty, mailAttachments: string.Empty);
                EmailUtility.SendMail(emailParams);
            }
        }
    }
}
