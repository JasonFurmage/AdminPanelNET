using AdminPanelNET.Data;
using AdminPanelNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelNET.Controllers
{
    // Handles all CRUD actions for Employee records.
    public class EmployeesController : Controller
    {
        // Database context injected by ASP.NET Core.
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        // Show all employees, including their related company data.
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees
                .Include(e => e.Company)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        // Show details for one employee, including company data.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        // Show create form and populate company dropdown.
        public IActionResult Create()
        {
            PopulateCompaniesDropDownList();
            return View();
        }

        // POST: Employees/Create
        // Save new employee to database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,CompanyId,Email,PhoneNumber")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, repopulate dropdown before returning view.
            PopulateCompaniesDropDownList(employee.CompanyId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        // Show edit form with current employee values.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            PopulateCompaniesDropDownList(employee.CompanyId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // Save edits to existing employee.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,CompanyId,Email,PhoneNumber")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
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

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateCompaniesDropDownList(employee.CompanyId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        // Show confirmation page before deleting.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        // Delete employee after confirmation.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method to fill company dropdown.
        private void PopulateCompaniesDropDownList(object? selectedCompany = null)
        {
            var companiesQuery = _context.Companies
                .OrderBy(c => c.Name)
                .ToList();

            ViewData["CompanyId"] = new SelectList(companiesQuery, "Id", "Name", selectedCompany);
        }

        // Helper method to check if employee exists.
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}