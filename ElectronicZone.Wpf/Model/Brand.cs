using ElectronicZone.Wpf.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicZone.Wpf.Model
{
    public class Brand : ViewModelBase
    {
        private int _id;
        private string _name;
        private string _description;
        private bool _isActive;
        private DateTime _createdDate;
        private DateTime? _modifiedDate;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }
        public DateTime? ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }


        //public int Id
        //{
        //    get { return _id; }
        //    set
        //    {
        //        _id = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string Name
        //{
        //    get { return _name; }
        //    set
        //    {
        //        _name = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string Description
        //{
        //    get { return _description; }
        //    set
        //    {
        //        _description = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public bool IsActive
        //{
        //    get { return _isActive; }
        //    set
        //    {
        //        _isActive = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public DateTime CreatedDate
        //{
        //    get { return _createdDate; }
        //    set
        //    {
        //        _createdDate = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public DateTime? ModifiedDate
        //{
        //    get { return _modifiedDate.Value; }
        //    set
        //    {
        //        _modifiedDate = value;
        //        OnPropertyChanged();
        //    }
        //}
    }
}
