using System;

namespace ElectronicZone.Wpf.Model
{
    public class Product
    {
        #region Properties
        private int _id;
        private string _name;
        private string _description;
        private bool _isActive;
        private bool _isNotUsed;
        private DateTime _createdDate;
        private DateTime? _modifiedDate;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public bool IsNotUsed { get => _isNotUsed; set => _isNotUsed = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }
        public DateTime? ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }
        #endregion
    }
}
