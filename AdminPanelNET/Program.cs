using AdminPanelNET.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register ApplicationDbContext with ASP.NET Core's dependency injection container.
// Use SQLite and get connection string called "DefaultConnection" from appsettings.json.
// Run database seeder during database creation/update in both sync and async scenarios.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSeeding((context, _) => DbSeeder.Seed((ApplicationDbContext)context))
           .UseAsyncSeeding((context, _, cancellationToken) =>
               DbSeeder.SeedAsync((ApplicationDbContext)context, cancellationToken)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
