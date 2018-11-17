using System.Collections.Generic;
using System.Configuration;

namespace ElectronicZone.Wpf.Utility.EMail
{
    internal static class EmailConfig
    {
        private const string SectionName = "EmailConfig";

        public static EmailConfiguration ReadConfiguration()
        {
            var configSection = (EmailConfiguration)System.Configuration.ConfigurationManager.GetSection(SectionName);
            return configSection;
        }
    }

    public class EmailConfiguration : ConfigurationSection
    {

        [ConfigurationProperty("fromEMailName", IsRequired = true)]
        public string FromEMailName
        {
            get { return (string)this["fromEMailName"]; }
            set { this["fromEMailName"] = value; }
        }

        [ConfigurationProperty("fromEMailDisplayName")]
        public string FromEMailDisplayName
        {
            get { return (string)this["fromEMailDisplayName"]; }
            set { this["fromEMailDisplayName"] = value; }
        }

        [ConfigurationProperty("mailServer", IsRequired = true)]
        public string MailServer
        {
            get { return (string)this["mailServer"]; }
            set { this["mailServer"] = value; }
        }

        [ConfigurationProperty("mailServerPort", IsRequired = true)]
        public string MailServerPort
        {
            get { return (string)this["mailServerPort"]; }
            set { this["mailServerPort"] = value; }
        }

        [ConfigurationProperty("mailFromAddress", IsRequired = true)]
        public string MailFromAddress
        {
            get { return (string)this["mailFromAddress"]; }
            set { this["mailFromAddress"] = value; }
        }

        [ConfigurationProperty("userName", IsRequired = true)]
        public string UserName
        {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }


        [ConfigurationProperty("enableSsl", IsRequired = true)]
        public bool EnableSsl
        {
            get { return (bool)this["enableSsl"]; }
            set { this["enableSsl"] = value; }
        }

        [ConfigurationProperty("mailFromPassword", IsRequired = true)]
        public string MailFromPassword
        {
            get { return (string)this["mailFromPassword"]; }
            set { this["mailFromPassword"] = value; }
        }

        [ConfigurationProperty("EmailTypes", IsRequired = true)]
        public EmailTypeCollection EmailTypes
        {
            get { return this["EmailTypes"] as EmailTypeCollection; }
        }

        //[ConfigurationProperty("siteURL", IsRequired = true)]
        //public string URL
        //{
        //    get { return (string)this["siteURL"]; }
        //    set { this["siteURL"] = value; }
        //}

    }

    [ConfigurationCollection(typeof(EmailType), AddItemName = "EmailType")]
    public class EmailTypeCollection : ConfigurationElementCollection, IEnumerable<EmailType>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EmailType();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var emailType = element as EmailType;
            if (emailType != null)
                return emailType.Name;
            else
                return null;
        }

        public EmailType this[int index]
        {
            get
            {
                return BaseGet(index) as EmailType;
            }
        }

        public new IEnumerator<EmailType> GetEnumerator()
        {
            for (int i = 0; i < base.Count; i++)
            {
                yield return this[i];
            }
        }
    }

    public class EmailType : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("subject", IsRequired = true)]
        public string Subject
        {
            get { return (string)this["subject"]; }
            set { this["subject"] = value; }
        }

        [ConfigurationProperty("htmlContentFile", IsRequired = true)]
        public string HtmlContentFile
        {
            get { return (string)this["htmlContentFile"]; }
            set { this["htmlContentFile"] = value; }
        }

    }
}
