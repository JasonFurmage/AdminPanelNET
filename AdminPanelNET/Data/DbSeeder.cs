using AdminPanelNET.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelNET.Data
{
    // Class that contains sample data for database.
    public static class DbSeeder
    {
        // Synchronous seed method (required by EF Core tools).
        public static void Seed(ApplicationDbContext context)
        {
            // If there are no companies, add 3 default companies.
            if (!context.Companies.Any())
            {
                context.Companies.AddRange(
                    new Company
                    {
                        Name = "Acme Digital",
                        Email = "hello@acmedigital.co.uk",
                        Website = "https://www.acmedigital.co.uk",
                        Logo = null
                    },
                    new Company
                    {
                        Name = "Bright Solutions",
                        Email = "contact@brightsolutions.co.uk",
                        Website = "https://www.brightsolutions.co.uk",
                        Logo = null
                    },
                    new Company
                    {
                        Name = "Northwind Creative",
                        Email = "team@northwindcreative.co.uk",
                        Website = "https://www.northwindcreative.co.uk",
                        Logo = null
                    }
                );

                // Save so companies get database-generated IDs.
                context.SaveChanges();
            }

            // If there are no employees, add 3 default employees.
            if (!context.Employees.Any())
            {
                // Get seeded companies so employees can be added to them.
                var acmeDigital = context.Companies.FirstOrDefault(c => c.Name == "Acme Digital");
                var brightSolutions = context.Companies.FirstOrDefault(c => c.Name == "Bright Solutions");
                var northwindCreative = context.Companies.FirstOrDefault(c => c.Name == "Northwind Creative");

                // Stop if any companies are missing.
                if (acmeDigital == null || brightSolutions == null || northwindCreative == null)
                {
                    return;
                }

                context.Employees.AddRange(
                    new Employee
                    {
                        FirstName = "Alice",
                        LastName = "Johnson",
                        Email = "alice.johnson@acmedigital.co.uk",
                        PhoneNumber = "01234 567890",
                        CompanyId = acmeDigital.Id
                    },
                    new Employee
                    {
                        FirstName = "Ben",
                        LastName = "Carter",
                        Email = "ben.carter@brightsolutions.co.uk",
                        PhoneNumber = "01632 960123",
                        CompanyId = brightSolutions.Id
                    },
                    new Employee
                    {
                        FirstName = "Sophie",
                        LastName = "Williams",
                        Email = "sophie.williams@northwindcreative.co.uk",
                        PhoneNumber = "0207 123 4567",
                        CompanyId = northwindCreative.Id
                    }
                );

                // Save so employees get database-generated IDs.
                context.SaveChanges();
            }
        }

        // Async version of seed method.
        public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
        {
            if (!await context.Companies.AnyAsync(cancellationToken))
            {
                await context.Companies.AddRangeAsync(
                    new[]
                    {
                        new Company
                        {
                            Name = "Acme Digital",
                            Email = "hello@acmedigital.co.uk",
                            Website = "https://www.acmedigital.co.uk",
                            Logo = null
                        },
                        new Company
                        {
                            Name = "Bright Solutions",
                            Email = "contact@brightsolutions.co.uk",
                            Website = "https://www.brightsolutions.co.uk",
                            Logo = null
                        },
                        new Company
                        {
                            Name = "Northwind Creative",
                            Email = "team@northwindcreative.co.uk",
                            Website = "https://www.northwindcreative.co.uk",
                            Logo = null
                        }
                    },
                    cancellationToken
                );

                await context.SaveChangesAsync(cancellationToken);
            }

            if (!await context.Employees.AnyAsync(cancellationToken))
            {
                var acmeDigital = await context.Companies.FirstOrDefaultAsync(
                    c => c.Name == "Acme Digital", cancellationToken);

                var brightSolutions = await context.Companies.FirstOrDefaultAsync(
                    c => c.Name == "Bright Solutions", cancellationToken);

                var northwindCreative = await context.Companies.FirstOrDefaultAsync(
                    c => c.Name == "Northwind Creative", cancellationToken);

                if (acmeDigital == null || brightSolutions == null || northwindCreative == null)
                {
                    return;
                }

                await context.Employees.AddRangeAsync(
                    new[]
                    {
                        new Employee
                        {
                            FirstName = "Alice",
                            LastName = "Johnson",
                            Email = "alice.johnson@acmedigital.co.uk",
                            PhoneNumber = "01234 567890",
                            CompanyId = acmeDigital.Id
                        },
                        new Employee
                        {
                            FirstName = "Ben",
                            LastName = "Carter",
                            Email = "ben.carter@brightsolutions.co.uk",
                            PhoneNumber = "01632 960123",
                            CompanyId = brightSolutions.Id
                        },
                        new Employee
                        {
                            FirstName = "Sophie",
                            LastName = "Williams",
                            Email = "sophie.williams@northwindcreative.co.uk",
                            PhoneNumber = "0207 123 4567",
                            CompanyId = northwindCreative.Id
                        }
                    },
                    cancellationToken
                );

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}