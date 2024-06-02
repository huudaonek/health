using System.ComponentModel.DataAnnotations;

namespace insure_fixlast.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Level Level { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public ICollection<Claim> Claims { get; set; }
        public bool isDelete { get; set; } = false;
        public ICollection<OrderDetail> OrderDetails { get; set; } 
    }
    public class Register
    {
        // Account Information
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Employee Information
        [Required]
        public string EmployeeName { get; set; }

        [Required]
        public Level Level { get; set; }

        [Required]
        public string EmployeeAddress { get; set; }

        [Required]
        [Phone]
        public string EmployeePhone { get; set; }
    }

    public enum Level
    {
        manager,
        admin,
        employee
    }

}
    