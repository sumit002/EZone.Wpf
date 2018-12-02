using System;

namespace ElectronicZone.Wpf.Model
{
    public class PendingPayment
    {
        #region Properties
        private int _id;
        private int _saleId;
        private int _salePersonId;
        private double _pendingAmount;
        private bool _isPaid;
        private DateTime _paidDate;
        private bool _isDiscount;
        private double _paidAmount;

        public int Id { get => _id; set => _id = value; }
        public int SaleId { get => _saleId; set => _saleId = value; }
        public int SalePersonId { get => _salePersonId; set => _salePersonId = value; }
        public double PendingAmount { get => _pendingAmount; set => _pendingAmount = value; }
        public bool IsPaid { get => _isPaid; set => _isPaid = value; }
        public DateTime PaidDate { get => _paidDate; set => _paidDate = value; }
        public bool IsDiscount { get => _isDiscount; set => _isDiscount = value; }
        public double PaidAmount { get => _paidAmount; set => _paidAmount = value; }

        public string Name { get => _name; set => _name = value; }
        public string PrimaryContact { get => _primaryContact; set => _primaryContact = value; }
        public double Total { get => _total; set => _total = value; }
        //public double AmountPaid { get => _amountPaid; set => _amountPaid = value; }
        public DateTime SaleDate { get => _saleDate; set => _saleDate = value; }

        
        public double MinAmountForDiscount { get ; set ; }
        public string SalePersonToDisplay { get; set; }
        public string ProductToDisplay { get; set; }
        public string ProductCodeToDisplay { get; set; }

        private string _name;
        private string _primaryContact;
        private double  _total;
        //private double _amountPaid;
        private DateTime _saleDate;
        #endregion
    }
}
