using System;
using System.Net;
using System.Net.Mail;

namespace ElectronicZone.Wpf.Utility
{
    /// <summary>
    /// Reference : Not In Use 
    /// https://ithoughthecamewithyou.com/post/sending-email-via-gmail-in-cnet-using-smtpclient
    /// https://www.smarterasp.net/support/kb/a1546/send-email-from-gmail-with-smtp-authentication-but-got-5_5_1-authentication-required-error.aspx
    /// </summary>
    public class MailService
    {
        #region Properties
        private readonly string _userName = "info.ezone2016@gmail.com";
        private readonly string _password = "!nf0.ez0ne2o|6";//abcd1234!
        private readonly string _fromEmailDisplayName = "eZone";
        ILogger logger = new Logger(typeof(MailService));
        #endregion

        /// <summary>
        /// Send Email to Recipent
        /// Allow less secure apps : ON for gmail A/C (https://myaccount.google.com/lesssecureapps)
        /// </summary>
        /// <param name="toEmailAddress"></param>
        public void SendMail(string toEmailAddress) {
            using (SmtpClient smtpGmailClient = new SmtpClient())
            {
                smtpGmailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpGmailClient.UseDefaultCredentials = false;
                smtpGmailClient.EnableSsl = true;
                smtpGmailClient.Host = "smtp.gmail.com";
                smtpGmailClient.Port = 587;
                smtpGmailClient.Credentials = new NetworkCredential(_userName, _password);
                try
                {
                    MailAddress _fromAddress = new MailAddress(_userName, _fromEmailDisplayName);
                    MailAddress _toAddress = new MailAddress(toEmailAddress, "");
                    MailMessage message = new MailMessage(_fromAddress, _toAddress);
                    message.Body = "This is a test e-mail message sent using gmail as a relay server ";
                    message.Subject = "Gmail test email with SSL and Credentials";
                    // send the email
                    smtpGmailClient.Send(message);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                }
            }
        }
    }
}
