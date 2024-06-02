    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using insure_fixlast.Data;
    using insure_fixlast.Models;
    using System.Text;
    using System.Security.Cryptography;

    namespace insure_fixlast.Controllers
    {
        public class EmployeesController : Controller
        {
            private readonly insure_fixlastContext _context;

            public EmployeesController(insure_fixlastContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> Index()
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                    return RedirectToAction("Login", "Accounts");

                var userCompany = await _context.Company.FirstOrDefaultAsync(c => c.AccountId == int.Parse(userId));
                if (userCompany == null)
                    return RedirectToAction("Create", "Companies");

                var employees = await _context.Employee.Where(e => e.CompanyId == userCompany.Id).ToListAsync();
                return View(employees);
            }
            // GET: Employees/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employee
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var company = _context.Company.FirstOrDefault(c => c.AccountId.ToString() == userId);

            if (company == null)
            {
                return RedirectToAction("Error", "Home");
            }

            ViewData["CompanyId"] = company.Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Register model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Accounts");

            var userCompany = await _context.Company.FirstOrDefaultAsync(c => c.AccountId == int.Parse(userId));
            if (userCompany == null)
                return RedirectToAction("Create", "Companies");

            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    Email = model.Email,
                    Name = model.Name,
                    Password = HashPassword(model.Password),
                    Role = Role.User
                };

                _context.Account.Add(account);
                await _context.SaveChangesAsync();

                var employee = new Employee
                {
                    Name = model.EmployeeName,
                    Address = model.EmployeeAddress,
                    Phone = model.EmployeePhone,
                    CompanyId = userCompany.Id,
                    Level = Level.employee
                };

                _context.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        private string HashPassword(string password)
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
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employee.FindAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", employee.CompanyId);
                return View(employee);
            }

            // POST: Employees/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Phone,Role,CompanyId")] Employee employee)
            {
                if (id != employee.Id)
                {
                    return NotFound();
                }

                if (true)
                {
                    try
                    {
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmployeeExists(employee.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Id", employee.CompanyId);
                return View(employee);
            }

            // GET: Employees/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employee
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }

            // POST: Employees/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var employee = await _context.Employee.FindAsync(id);
                if (employee != null)
                {
                    _context.Employee.Remove(employee);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool EmployeeExists(int id)
            {
                return _context.Employee.Any(e => e.Id == id);
            }
        }
    }
