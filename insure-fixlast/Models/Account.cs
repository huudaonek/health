namespace insure_fixlast.Models
{
    public enum Role
    {
        Admin,
        User
    }

    public class Account
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }

    public class RegisterEmployeeViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string EmployeeName { get; set; }
        public Level EmployeeRank { get; set; } 
        public string EmployeeAddress { get; set; }
        public string EmployeePhone { get; set; }
        public int CompanyId { get; set; }
    }
}
