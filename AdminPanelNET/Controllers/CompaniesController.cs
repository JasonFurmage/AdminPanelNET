using AdminPanelNET.Data;
using AdminPanelNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelNET.Controllers
{
    // Handles all CRUD actions for Company records.
    public class CompaniesController : Controller
    {
        // Database context.
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        // Shows list of all companies in database.
        public async Task<IActionResult> Index()
        {
            return View(await _context.Companies.ToListAsync());
        }

        // GET: Companies/Details/5
        // Shows details page for one company.
        public async Task<IActionResult> Details(int? id)
        {
            // If no ID was provided, return 404 response.
            if (id == null)
            {
                return NotFound();
            }

            // Find company with matching ID.
            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);

            // If company does not exist, return 404 response.
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        // Shows empty create form.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // Receives submitted form data and creates a new company.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Logo,Website")] Company company)
        {
            // Only save if validation passed.
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If validation failed, show form again with validation messages.
            return View(company);
        }

        // GET: Companies/Edit/5
        // Shows edit form with existing company data filled in.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Edit/5
        // Receives edited form data and updates existing company.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Logo,Website")] Company company)
        {
            // Protect against a mismatch between route ID and submitted model ID.
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If company was deleted by someone before save, return 404.
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Otherwise rethrow exception so we can see real problem.
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        // GET: Companies/Delete/5
        // Shows confirmation page before deleting a company.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        // Deletes the company after confirmation.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company != null)
            {
                _context.Companies.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method to check whether a company exists.
        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}