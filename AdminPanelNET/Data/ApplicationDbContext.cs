using AdminPanelNET.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelNET.Data
{
    // Bridge between C# app and database.
    // Used by EF core to query data, save data, and map model classes to database tables.
    public class ApplicationDbContext : DbContext
    {
        // Recieves configuration options from ASP.NET Core's dependency injection system
        // so it knows which database provider and connection string to use.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Companies table
        public DbSet<Company> Companies { get; set; }

        // Employees table
        public DbSet<Employee> Employees { get; set; }
    }
}