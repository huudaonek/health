using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using insure_fixlast.Data;
using insure_fixlast.Models;
using Microsoft.Identity.Client;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace insure_fixlast.Controllers
{
    public class AccountsController : Controller
    {
        private readonly insure_fixlastContext _context;

        public AccountsController(insure_fixlastContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Account.ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Name,Password,Role")] Account account)
        {
            if (ModelState.IsValid)
            {
				account.Password = HashPassword(account.Password); 
				_context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Name,Password,Role")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            return View(account);
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Account.FindAsync(id);
            if (account != null)
            {
                _context.Account.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Account.Any(e => e.Id == id);
        }

        public IActionResult Login()
        {
            return View("~/Views/Accounts/Login.cshtml");
        }
        [HttpPost]
        public IActionResult Login(Account user)
        {
            var hashedPassword = HashPassword(user.Password);
            var existingUser = _context.Account.FirstOrDefault(u => u.Email == user.Email && u.Password == hashedPassword);
            if (existingUser != null)
            {
                HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
                HttpContext.Session.SetString("Email", existingUser.Email);
                HttpContext.Session.SetString("Name", existingUser.Name);

                var company = _context.Company.Include(c => c.Account).FirstOrDefault(c => c.AccountId == existingUser.Id);
                if (company != null)
                {
                    var employees = _context.Employee.Where(e => e.CompanyId == company.Id).ToList();
                    if (employees.Any())
                    {
                        var employee = employees.First();
                        HttpContext.Session.SetString("EmployeeName", employee.Name);
                        HttpContext.Session.SetString("EmployeeLevel", employee.Level.ToString());

                    }
                }

                if (existingUser.Role == Role.Admin)
                {
                    return RedirectToAction("Index", "Admins");
                }
                else if (existingUser.Role == Role.User)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Role value is not valid.";
                    return View("Login");
                }
            }

            ViewBag.Error = "Invalid email or password";
            return View("Login");
        }

        // GET: Accounts/RegisterEmployee
        public IActionResult RegisterEmployee()
        {
            return View();
        }

        // POST: Accounts/RegisterEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmployee(RegisterEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create Account
                var account = new Account
                {
                    Email = model.Email,
                    Name = model.Name,
                    Password = HashPassword(model.Password),
                    Role = model.Role
                };

                _context.Account.Add(account);
                await _context.SaveChangesAsync();

                // Create Company
                var company = new Company
                {
                    Name = model.CompanyName,
                    Email = model.CompanyEmail,
                    Phone = model.CompanyPhone,
                    Address = model.CompanyAddress,
                    AccountId = account.Id
                };

                _context.Company.Add(company);
                await _context.SaveChangesAsync();

                // Create Employee
                var employee = new Employee
                {
                    Name = model.EmployeeName,
                    Level = model.EmployeeRank,
                    Address = model.EmployeeAddress,
                    Phone = model.EmployeePhone,
                    CompanyId = company.Id
                };

                _context.Employee.Add(employee);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
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
    }
}
