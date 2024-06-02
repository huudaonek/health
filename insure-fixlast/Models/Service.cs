    namespace insure_fixlast.Models
    {
        public class Service
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public decimal Claim { get; set; }
            public int Time { get; set; }
            public ICollection<OrderDetail> OrderDetails { get; set; }
        }
    }
