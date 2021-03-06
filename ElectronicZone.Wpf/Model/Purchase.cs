﻿using System;

namespace ElectronicZone.Wpf.Model
{
    public class Purchase
    {
        private int _id;
        private int _productId;
        private int _brandId;
        private string _productCode;
        private string _stockCode;
        private string _itemDescription;
        private int _quantity;
        private int _avlQuantity;
        private double _purchasePrice;
        private double _salePrice;
        private byte[] _productImage;
        private bool _isActive;
        private DateTime _purchaseDate;
        private DateTime _createdDate;
        private DateTime _modifiedDate;

        private string _product;
        private string _brand;
        private double _totalPurchasePrice;
        //private bool _isAddMoreQtyMode;
        //private int _oldQuantity;


        public int Id { get => _id; set => _id = value; }
        public int ProductId { get => _productId; set => _productId = value; }
        public int BrandId { get => _brandId; set => _brandId = value; }
        public string ProductCode { get => _productCode; set => _productCode = value; }
        public string StockCode { get => _stockCode; set => _stockCode = value; }
        public string ItemDesc { get => _itemDescription; set => _itemDescription = value; }
        public int Quantity { get => _quantity; set => _quantity = value; }
        public int AvlQuantity { get => _avlQuantity; set => _avlQuantity = value; }
        public double PurchasePrice { get => _purchasePrice; set => _purchasePrice = value; }
        public double SalePrice { get => _salePrice; set => _salePrice = value; }
        public byte[] ProductImage { get => _productImage; set => _productImage = value; }
        public bool IsActive { get => _isActive; set => _isActive = value; }
        public DateTime PurchaseDate { get => _purchaseDate; set => _purchaseDate = value; }
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }
        public DateTime ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }
        public string Product { get => _product; set => _product = value; }
        public string Brand { get => _brand; set => _brand = value; }
        public double TotalPurchasePrice { get => _totalPurchasePrice; set => _totalPurchasePrice = value; }
        //public bool IsAddMoreQtyMode { get => _isAddMoreQtyMode; set => _isAddMoreQtyMode = value; }
        //public int OldQuantity { get => _oldQuantity; set => _oldQuantity = value; }
    }
}
