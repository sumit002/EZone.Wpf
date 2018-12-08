using System;

namespace ElectronicZone.Wpf.Model
{
    public class Sale
    {
        #region Properties
        private int _id;
        private int _stockId;
        private int _salePersonId;
        private int _quantity;
        private double _price;
        private double _total;
        private double _amountPaid;
        private DateTime _saleDate;
        private bool _isActive;
        private DateTime _createdDate;
        private DateTime _modifiedDate;
        private bool _isDiscounted;
        private bool _canCancel;

        public int Id { get => _id; set => _id = value; }
        public int StockId { get => _stockId; set => _stockId = value; }
        public int SalePersonId { get => _salePersonId; set => _salePersonId = value; }
        public int Quantity { get => _quantity; set => _quantity = value; }
        public double Price { get => _price; set => _price = value; }
        public double Total { get => _total; set => _total = value; }
        public double AmountPaid { get => _amountPaid; set => _amountPaid = value; }
        public DateTime SaleDate { get => _saleDate; set => _saleDate = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }
        public DateTime ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }
        public bool IsDiscounted { get => _isDiscounted; set => _isDiscounted = value; }
        public bool CanCancel { get => _canCancel; set => _canCancel = value; }

        private string _product;
        private string _brand;
        private string _productCode;
        private string _stockCode;
        private string _saleTo;
        private string _saleContact;
        private double pending;
        private string _invoiceNumber;

        public string Product { get => _product; set => _product = value; }
        public string Brand { get => _brand; set => _brand = value; }
        public string ProductCode { get => _productCode; set => _productCode = value; }
        public string StockCode { get => _stockCode; set => _stockCode = value; }
        public string SaleTo { get => _saleTo; set => _saleTo = value; }
        public string SaleContact { get => _saleContact; set => _saleContact = value; }
        public double Pending { get => pending; set => pending = value; }
        public string InvoiceNumber { get => _invoiceNumber; set => _invoiceNumber = value; }

        public int SaleCount { get; set; }

        public Contact Contact { get; set; }
        #endregion
    }
}
