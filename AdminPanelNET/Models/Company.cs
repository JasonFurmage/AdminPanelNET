using System.ComponentModel.DataAnnotations;

namespace AdminPanelNET.Models
{
    // Represents Companies table in database.
    public class Company
    {
        // Primary key.
        public int Id { get; set; }

        // Company name.
        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Company name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // Email address (optional).
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
        public string? Email { get; set; }

        // Logo file path (optional).
        [StringLength(255, ErrorMessage = "Logo path cannot be longer than 255 characters.")]
        public string? Logo { get; set; }

        // Website (optional).
        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(255, ErrorMessage = "Website URL cannot be longer than 255 characters.")]
        public string? Website { get; set; }


        // Navigation property:
        // Employees collection (one company can have many employees).
        public ICollection<Employee> Employees { get; set; } = new List<Employee>(); // Initialise to avoids null-reference issues.
    }
}