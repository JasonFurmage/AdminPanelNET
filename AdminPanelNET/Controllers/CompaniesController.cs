using AdminPanelNET.Data;
using AdminPanelNET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;

namespace AdminPanelNET.Controllers
{
    // Handles all CRUD actions for Company records.
    public class CompaniesController : Controller
    {
        // Database context injected by ASP.NET Core.
        private readonly ApplicationDbContext _context;

        // Gives access to wwwroot so uploaded logo files can be saved there.
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Allowed logo file extensions.
        private static readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        // Max logo file size: 2 MB
        private const long MaxLogoFileSize = 2 * 1024 * 1024;

        // Minimum logo dimensions.
        private const int MinLogoWidth = 100;
        private const int MinLogoHeight = 100;

        public CompaniesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Website")] Company company, IFormFile? logoFile)
        {
            // If logo file was uploaded, validate and save it.
            if (logoFile != null && logoFile.Length > 0)
            {
                company.Logo = await SaveLogoFileAsync(logoFile);
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Website")] Company company, IFormFile? logoFile)
        {
            // Protect against a mismatch between route ID and submitted model ID.
            if (id != company.Id)
            {
                return NotFound();
            }

            // Load existing company from database to preserve current logo if no new file is uploaded.
            var existingCompany = await _context.Companies.FindAsync(id);

            if (existingCompany == null)
            {
                return NotFound();
            }

            // Update normal text fields.
            existingCompany.Name = company.Name;
            existingCompany.Email = company.Email;
            existingCompany.Website = company.Website;

            // If new logo file was uploaded, validate it and replace old one.
            if (logoFile != null && logoFile.Length > 0)
            {
                existingCompany.Logo = await SaveLogoFileAsync(logoFile, existingCompany.Logo);
            }

            if (ModelState.IsValid)
            {
                try
                {
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

            // If validation failed, keep existing logo path so view can still show it.
            company.Logo = existingCompany.Logo;
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
                // Keep a copy of logo path so file can be deleted from disk after database record is removed.
                var logoPath = company.Logo;

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                DeleteLogoFile(logoPath);
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper method to check whether a company exists.
        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }

        // Helper method to validate and save logo file.
        // Returns relative path to be stored in database.
        private async Task<string?> SaveLogoFileAsync(IFormFile logoFile, string? existingLogoPath = null)
        {
            // Basic size check.
            if (logoFile.Length <= 0)
            {
                ModelState.AddModelError("logoFile", "Please choose a logo file.");
                return existingLogoPath;
            }

            if (logoFile.Length > MaxLogoFileSize)
            {
                ModelState.AddModelError("logoFile", "Logo file size must be 2 MB or smaller.");
                return existingLogoPath;
            }

            // Check file extension.
            var originalFileName = Path.GetFileName(logoFile.FileName);
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("logoFile", "Only JPG, JPEG, PNG, GIF, and WEBP files are allowed.");
                return existingLogoPath;
            }

            // Check image dimensions.
            try
            {
                await using var readStream = logoFile.OpenReadStream();
                var imageInfo = await Image.IdentifyAsync(readStream);

                if (imageInfo == null)
                {
                    ModelState.AddModelError("logoFile", "The uploaded file is not a valid image.");
                    return existingLogoPath;
                }

                if (imageInfo.Width < MinLogoWidth || imageInfo.Height < MinLogoHeight)
                {
                    ModelState.AddModelError("logoFile", "Logo must be at least 100x100 pixels.");
                    return existingLogoPath;
                }
            }
            catch
            {
                ModelState.AddModelError("logoFile", "The uploaded file could not be read as an image.");
                return existingLogoPath;
            }

            // Make sure uploads/logos folder exists inside wwwroot.
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(uploadsFolder);

            // Generate safe random filename.
            var safeFileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{extension}";
            var physicalPath = Path.Combine(uploadsFolder, safeFileName);

            // Save uploaded file to disk.
            await using (var fileStream = new FileStream(physicalPath, FileMode.Create))
            {
                await logoFile.CopyToAsync(fileStream);
            }

            // If new file was saved successfully, delete old logo file.
            DeleteLogoFile(existingLogoPath);

            // Return relative path to store in database.
            return $"/uploads/logos/{safeFileName}";
        }

        // Helper method to delete old logo file from disk.
        private void DeleteLogoFile(string? logoPath)
        {
            if (string.IsNullOrWhiteSpace(logoPath))
            {
                return;
            }

            // Convert web path like "/uploads/logos/file.png" to physical file path.
            var trimmedPath = logoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, trimmedPath);

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }
    }
}