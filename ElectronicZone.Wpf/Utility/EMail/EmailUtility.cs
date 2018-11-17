using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ElectronicZone.Wpf.Utility.EMail
{
    //ILogger logger = new Logger(typeof(EmailUtility));
    public enum SendEmailType
    {
        SupportIncomeMail,
        PurchaseOrderMail,
        SalesOrderMail,
        ResetPasswordMail,
    }

    public static class EmailUtility
    {
        
        public static void SendMail(EmailParams mailParams, bool failOnError = false)
        {
            SendMail(mailParams.ReplyMail, mailParams.ReplyName, mailParams.ToMail, mailParams.Subject, mailParams.MailBody, mailParams.CcMail, mailParams.BccMail, mailParams.MailAttachemt, failOnError: failOnError);
        }

        public static void SendMail(string replyMail, string replyName, string toMail, string subject, string mailBody, string ccMail, string bccMail, string mailAttachments, bool isBodyHtml = true, bool failOnError = false)
        {
            RemoveDuplicateEmails(ref toMail, ref ccMail);

            //Logger.Info(string.Format("Sendig Email ({0}): (ToMail: {1},Subject: {2},CcMail: {3} )", MethodBase.GetCurrentMethod().Name, toMail, subject, ccMail));
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mail = new MailMessage();
            try
            {
                string FromEMailName = EmailConfig.ReadConfiguration().FromEMailDisplayName;// FromEMailName;
                string SMTPServer = EmailConfig.ReadConfiguration().MailServer;
                string SMTPServerPort = EmailConfig.ReadConfiguration().MailServerPort;
                string FromEMailAddress = EmailConfig.ReadConfiguration().MailFromAddress;
                string UserName = EmailConfig.ReadConfiguration().UserName;
                bool EnableSsl = EmailConfig.ReadConfiguration().EnableSsl;
                string FromEMailPassword = EmailConfig.ReadConfiguration().MailFromPassword;
                MailAddress myAddress = new MailAddress(FromEMailAddress, FromEMailName);

                smtpClient.Host = SMTPServer;
                smtpClient.Port = Convert.ToInt32(SMTPServerPort);
                smtpClient.EnableSsl = EnableSsl;
                NetworkCredential networkCredential = new NetworkCredential(UserName, FromEMailPassword);


                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = networkCredential;
                if (!string.IsNullOrEmpty(replyMail))
                {
                    MailAddress replyToAddress = new MailAddress(replyMail, replyName);
                    mail.ReplyToList.Add(replyToAddress);
                }
                mail.From = myAddress;
                mail.To.Add(toMail);
                mail.Subject = subject;
                if (!string.IsNullOrEmpty(ccMail))
                    mail.CC.Add(ccMail);
                if (!string.IsNullOrEmpty(bccMail))
                    mail.Bcc.Add(bccMail);
                if (!string.IsNullOrEmpty(mailAttachments))
                {
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(mailAttachments);
                    mail.Attachments.Add(attachment);
                }

                if (!isBodyHtml)
                {
                    mail.IsBodyHtml = false;
                }
                else
                {
                    mail.IsBodyHtml = true;
                }
                mail.Body = mailBody;
                smtpClient.Send(mail);
                //Logger.Info("Email Sent");
            }
            catch (Exception ex)
            {
                //Logger.Error(string.Format("({0}) Error Sending Email", MethodBase.GetCurrentMethod().Name), ex);
                if (failOnError)
                    throw ex;
            }
        }

        private static void RemoveDuplicateEmails(ref string toMail, ref string ccMail)
        {

            List<string> toMailList = toMail.Split(',').Distinct().ToList();
            List<string> ccMailList = ccMail?.Split(',')?.Distinct()?.ToList() ?? new List<string>();
            ccMailList.RemoveAll(a => toMailList.Contains(a));

            /*m_toMail = */
            toMail = string.Join(",", toMailList.ToArray());
            /*m_ccMail = */
            ccMail = string.Join(",", ccMailList.ToArray());
        }

        #region Mail messages
        public static string GetEmailSubject(SendEmailType emailType, Dictionary<string, string> placeHolders)
        {
            var emailTypeConfig = EmailConfig.ReadConfiguration().EmailTypes.FirstOrDefault(m => m.Name == emailType.ToString());
            return ReplaceMessagePlaceHolders(emailTypeConfig.Subject, placeHolders);
        }

        public static string GetEmailMessage(SendEmailType emailType, Dictionary<string, string> placeHolders)
        {
            var emailTypeConfig = EmailConfig.ReadConfiguration().EmailTypes.FirstOrDefault(m => m.Name == emailType.ToString());

            string mailbody = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, emailTypeConfig.HtmlContentFile)))
            {
                mailbody = reader.ReadToEnd();
            }
            return ReplaceMessagePlaceHolders(mailbody, placeHolders);
        }

        #region GetURL from config
        //public static string GetURLFromConfig()
        //{
        //    var url = EmailConfig.ReadConfiguration().URL;
        //    return url;
        //}
        public static string GetSenderNameFromConfig()
        {
            var fromName = EmailConfig.ReadConfiguration().FromEMailDisplayName;
            return fromName;
        }
        #endregion GetURL from config

        public static string ReplaceMessagePlaceHolders(string message, Dictionary<string, string> placeHolders)
        {
            if (placeHolders != null)
            {
                StringBuilder sb = new StringBuilder(message);
                foreach (var kv in placeHolders)
                {
                    sb.Replace(kv.Key, kv.Value);
                }
                message = sb.ToString();
            }
            return message;
        }
        #endregion Mail and SMS messages
    }

    public class EmailParams
    {
        public EmailParams(string toMail, SendEmailType emailType, Dictionary<string, string> placeHolders, string ccMail = null, string mailAttachments = null)
            : this(toMail, ccMail, null, EmailUtility.GetEmailSubject(emailType, placeHolders), EmailUtility.GetEmailMessage(emailType, placeHolders), null, null, mailAttachments)
        { }

        public EmailParams(string toMail, string ccMail, string bccMail, string subject, string mailBody, string replyMail, string replyName, string mailAttachments)
        {
            ToMail = toMail;
            CcMail = ccMail;
            BccMail = bccMail;
            Subject = subject;
            MailBody = mailBody;
            ReplyMail = replyMail;
            ReplyName = replyName;
            MailAttachemt = mailAttachments;
        }

        public string ReplyMail { get; private set; }
        public string ReplyName { get; private set; }
        public string ToMail { get; private set; }
        public string Subject { get; private set; }
        public string MailBody { get; private set; }
        public string CcMail { get; private set; }
        public string BccMail { get; private set; }
        public string MailAttachemt { get; private set; }
    }
}
