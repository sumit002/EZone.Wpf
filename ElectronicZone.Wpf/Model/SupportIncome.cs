using System;

namespace ElectronicZone.Wpf.Model
{
    public class SupportIncome
    {
        #region Properties
        private int _id;
        private string _description;
        private double _amount;
        private DateTime _supportDate;
        private string _remarks;
        private bool _isActive;

        public int Id { get => _id; set => _id = value; }
        public string Description { get => _description; set => _description = value; }
        public double Amount { get => _amount; set => _amount = value; }
        public DateTime SupportDate { get => _supportDate; set => _supportDate = value; }
        public string Remarks { get => _remarks; set => _remarks = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; } 
        #endregion
    }
}
