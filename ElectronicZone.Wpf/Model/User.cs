using System;

namespace ElectronicZone.Wpf.Model
{
    public sealed class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
