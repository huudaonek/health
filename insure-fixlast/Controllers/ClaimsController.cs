using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using insure_fixlast.Data;
using insure_fixlast.Models;

namespace insure_fixlast.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly insure_fixlastContext _context;

        public ClaimsController(insure_fixlastContext context)
        {
            _context = context;
        }

        // GET: Claims
        public async Task<IActionResult> Index()
        {
            var insure_fixlastContext = _context.Claim.Include(c => c.Employee);
            return View(await insure_fixlastContext.ToListAsync());
        }

        // GET: Claims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim
                .Include(c => c.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // GET: Claims/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Set<Employee>(), "Id", "Id");
            return View();
        }

        // POST: Claims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Form,Hospital,amount,status,EmployeeId")] Claim claim)
        {
            if (true)
            {
                _context.Add(claim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Set<Employee>(), "Id", "Id", claim.EmployeeId);
            return View(claim);
        }

        // GET: Claims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Set<Employee>(), "Id", "Id", claim.EmployeeId);
            return View(claim);
        }

        // POST: Claims/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Form,Hospital,amount,status,EmployeeId")] Claim claim)
        {
            if (id != claim.Id)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    _context.Update(claim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimExists(claim.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Set<Employee>(), "Id", "Id", claim.EmployeeId);
            return View(claim);
        }

        // GET: Claims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim
                .Include(c => c.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim != null)
            {
                _context.Claim.Remove(claim);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaimExists(int id)
        {
            return _context.Claim.Any(e => e.Id == id);
        }
    }
}
