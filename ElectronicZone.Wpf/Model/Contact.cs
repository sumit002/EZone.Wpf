using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElectronicZone.Wpf.Utility.CommonEnum;

namespace ElectronicZone.Wpf.Model
{
    public class Contact
    {
        private int _id;
        private Salutation _title;
        private string _name;
        private string _contact;
        private string _altContact;
        private string _email;
        private Uri _emailUri;
        private string _address;
        private bool _isActive;
        //private DateTime _createdDate;
        //private DateTime _modifiedDate;

        public int Id { get => _id; set => _id = value; }
        public Salutation Title { get => _title; set => _title = value; }
        public string Name { get => _name; set => _name = value; }
        public string PrimaryContact { get => _contact; set => _contact = value; }
        public string AltContact { get => _altContact; set => _altContact = value; }
        public string Email { get => _email; set => _email = value; }
        public string Address { get => _address; set => _address = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }

        public Uri EmailUri { get => _emailUri; set => _emailUri = value; }
        //public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }
        //public DateTime ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }

        //public enum Salutation
        //{
        //    // [Description("Mr")]
        //    Mr,
        //    Miss,
        //    Mrs,
        //}

        //public List<string> GetSalutationList()
        //{
        //    return Enum.GetNames(typeof(Salutation)).ToList();
        //}

        public static ObservableCollection<string> GetSalutationObservableCollection()
        {
            return new ObservableCollection<string>(Enum.GetNames(typeof(Salutation)));
        }

    }
}
