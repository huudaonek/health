namespace insure_fixlast.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public int Quantity { get; set; }
        public int? EmployeeId { get; set; } 
        public Employee Employee { get; set; } 
    }
    public class AssignPackageViewModel
    {
        public List<Employee> Employees { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

}
