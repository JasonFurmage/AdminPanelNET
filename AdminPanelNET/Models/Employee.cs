using System.ComponentModel.DataAnnotations;

namespace AdminPanelNET.Models
{
    // Represents Employees table in database.
    public class Employee
    {
        // Primary key.
        public int Id { get; set; }

        // First name.
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name cannot be longer than 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        // Last name.
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name cannot be longer than 100 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        // Company (foreign key).
        // Nullable so employees can exist without having a company.
        [Display(Name = "Company")]
        public int? CompanyId { get; set; }

        // Email address (optional).
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
        public string? Email { get; set; }

        // Phone number (optional).
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(50, ErrorMessage = "Phone number cannot be longer than 50 characters.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }


        // Navigation property:
        // Company (employees can have one company).
        public Company? Company { get; set; }
    }
}