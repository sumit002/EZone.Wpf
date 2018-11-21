using System;

namespace ElectronicZone.Wpf.Model
{
    public class FirebaseProduct
    {
        private int _id;
        private string _name;
        private string _description;
        private double _price;
        private string _serial;
        //private DateTime _createdOn;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public double Price { get => _price; set => _price = value; }
        public string Serial { get => _serial; set => _serial = value; }
    }
}
