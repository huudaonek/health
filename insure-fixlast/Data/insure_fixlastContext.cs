    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using insure_fixlast.Models;

    namespace insure_fixlast.Data
    {
        public class insure_fixlastContext : DbContext
        {
            public insure_fixlastContext(DbContextOptions<insure_fixlastContext> options)
                : base(options)
            {
            }

            public DbSet<Account> Account { get; set; } = default!;
            public DbSet<Claim> Claim { get; set; } = default!;
            public DbSet<Company> Company { get; set; } = default!;
            public DbSet<Employee> Employee { get; set; } = default!;
            public DbSet<Order> Order { get; set; } = default!;
            public DbSet<OrderDetail> OrderDetail { get; set; } = default!;
            public DbSet<Service> Service { get; set; } = default!;

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Claim>().ToTable("Claim");
                modelBuilder.Entity<Account>().ToTable("Account");
                modelBuilder.Entity<Company>().ToTable("Company");
                modelBuilder.Entity<Employee>().ToTable("Employee");
                modelBuilder.Entity<Order>().ToTable("Order");
                modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail");
                modelBuilder.Entity<Service>().ToTable("Service");

                // Configure relationships for OrderDetail
                modelBuilder.Entity<OrderDetail>()
                    .HasOne(od => od.Order)
                    .WithMany(o => o.Details)
                    .HasForeignKey(od => od.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<OrderDetail>()
                    .HasOne(od => od.Service)
                    .WithMany()
                    .HasForeignKey(od => od.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure relationships for Order
                modelBuilder.Entity<Order>()
                    .HasOne(o => o.Company)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CompanyId)
                    .IsRequired(); // Đảm bảo rằng CompanyId là bắt buộc
                modelBuilder.Entity<OrderDetail>()
                    .HasOne(od => od.Employee)
                    .WithMany(e => e.OrderDetails)  // Sử dụng navigation property tương ứng trong lớp Employee
                    .HasForeignKey(od => od.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

        }

        }
    }
