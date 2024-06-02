using insure_fixlast.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace insure_fixlast.Data
{
    public static class SeedData
    {
        private static bool isInitialized = false; // Flag to check if data has been initialized

        public static void Initialize(IServiceProvider serviceProvider)
        {
            if (isInitialized)
            {
                return; // If data has been initialized, do not seed again
            }

            using (var context = new insure_fixlastContext(
                serviceProvider.GetRequiredService<DbContextOptions<insure_fixlastContext>>()))
            {
                // Check and seed data for various tables
                if (!context.Account.Any() || !context.Company.Any() || !context.Employee.Any() || !context.Service.Any())
                {
                    SeedAccounts(context);
                    SeedCompanies(context);
                    SeedEmployees(context);
                    SeedServices(context);
                }

                isInitialized = true;
            }
        }

        private static void SeedAccounts(insure_fixlastContext context)
        {
            if (!context.Account.Any())
            {
                context.Account.AddRange(
                    new Account { Email = "admin@example.com", Name = "Admin", Password = HashPassword("Admin123"), Role = Role.Admin },
                    new Account { Email = "user1@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user2@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user3@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user4@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user5@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user6@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user7@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user8@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user9@example.com", Name = "User1", Password = HashPassword("User1234"), Role = Role.User },
                    new Account { Email = "user10@example.com", Name = "User2", Password = HashPassword("User1234"), Role = Role.User }
                );

                context.SaveChanges();
            }
        }

        private static void SeedCompanies(insure_fixlastContext context)
        {
            if (!context.Company.Any())
            {
                var accounts = context.Account.ToList();
                context.Company.AddRange(
                    new Company
                    {
                        Name = "TechCorp",
                        Email = "contact@techcorp.com",
                        Phone = "123-456-7890",
                        Address = "123 Tech Street",
                        AccountId = accounts.FirstOrDefault(a => a.Email == "admin@example.com").Id,
                        isDelete = false
                    },
                    new Company
                    {
                        Name = "HealthInc",
                        Email = "info@healthinc.com",
                        Phone = "098-765-4321",
                        Address = "456 Health Avenue",
                        AccountId = accounts.FirstOrDefault(a => a.Email == "user1@example.com").Id,
                        isDelete = false
                    },
                     new Company
                     {
                         Name = "HealthInc",
                         Email = "info@healthinc.com",
                         Phone = "098-765-4321",
                         Address = "456 Health Avenue",
                         AccountId = accounts.FirstOrDefault(a => a.Email == "user2@example.com").Id,
                         isDelete = false
                     }
                );

                context.SaveChanges();
            }
        }

        private static void SeedEmployees(insure_fixlastContext context)
        {
            if (!context.Employee.Any())
            {
                var company = context.Company.FirstOrDefault(c => c.Name == "HealthInc");
                if (company != null)
                {
                    context.Employee.AddRange(
                        new Employee
                        {
                            Name = "John Doe",
                            Address = "789 Employee Road",
                            Phone = "111-222-3333",
                            Level = Level.manager,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "Jane Smith",
                            Address = "321 Worker Lane",
                            Phone = "444-555-6666",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "Alice Johnson",
                            Address = "123 Main Street",
                            Phone = "777-888-9999",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "Bob Brown",
                            Address = "456 Elm Street",
                            Phone = "333-444-5555",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "Charlie Davis",
                            Address = "789 Oak Avenue",
                            Phone = "666-777-8888",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "Diana Evans",
                            Address = "101 Pine Road",
                            Phone = "222-333-4444",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "Edward Foster",
                            Address = "202 Maple Street",
                            Phone = "555-666-7777",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "Fiona Green",
                            Address = "303 Birch Lane",
                            Phone = "888-999-0000",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "George Harris",
                            Address = "404 Cedar Boulevard",
                            Phone = "999-000-1111",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        },
                        new Employee
                        {
                            Name = "Hannah King",
                            Address = "505 Redwood Drive",
                            Phone = "000-111-2222",
                            Level = Level.employee,
                            CompanyId = company.Id,
                            isDelete = false
                        }
                    );

                    context.SaveChanges();
                }
            }
        }

        private static void SeedServices(insure_fixlastContext context)
        {
            if (!context.Service.Any())
            {
                context.Service.AddRange(
                    new Service { Name = "Basic Health Plan", Description = "Basic health insurance plan", Price = 100, Claim = 10, Time = 1 },
                    new Service { Name = "Standard Health Plan", Description = "Standard health insurance plan", Price = 200, Claim = 20, Time = 2 },
                    new Service { Name = "Premium Health Plan", Description = "Premium health insurance plan", Price = 300, Claim = 30, Time = 3 },
                    new Service { Name = "Family Health Plan", Description = "Health insurance plan for families", Price = 400, Claim = 40, Time = 4 },
                    new Service { Name = "Senior Health Plan", Description = "Health insurance plan for seniors", Price = 500, Claim = 50, Time = 5 },
                    new Service { Name = "Student Health Plan", Description = "Health insurance plan for students", Price = 150, Claim = 15, Time = 1 },
                    new Service { Name = "Corporate Health Plan", Description = "Health insurance plan for corporate employees", Price = 600, Claim = 60, Time = 6 },
                    new Service { Name = "Individual Health Plan", Description = "Health insurance plan for individuals", Price = 250, Claim = 25, Time = 2 },
                    new Service { Name = "Maternity Health Plan", Description = "Health insurance plan for maternity care", Price = 350, Claim = 35, Time = 3 },
                    new Service { Name = "Dental Health Plan", Description = "Health insurance plan for dental care", Price = 200, Claim = 20, Time = 2 }
                );

                context.SaveChanges();
            }
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
