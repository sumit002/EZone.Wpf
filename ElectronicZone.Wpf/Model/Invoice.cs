namespace ElectronicZone.Wpf.Model
{
    public class Invoice
    {
        #region Properties
        public string SrNo { get; set; }
        public string Item { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string Total { get; set; }

        public Contact Contact { get; set; }
        #endregion
    }
}
