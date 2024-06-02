namespace insure_fixlast.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal PriceTotal { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public ICollection<OrderDetail> Details { get; set; }
        public Payment Payment { get; set; }
    }

    public enum Payment
    {
        momo,
        paypal
    }
}
