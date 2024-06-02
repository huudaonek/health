namespace insure_fixlast.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Order> Orders { get; set; }
        public bool isDelete { get; set; } = false;
    }

    public class CheckoutViewModel
    {
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public List<Service> SelectedServices { get; set; }
        public Dictionary<int, int> ServiceQuantities { get; set; }
    }
}
