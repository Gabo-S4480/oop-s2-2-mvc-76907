using FoodTrackerApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog; // Required for logging 

// 1. Initial Serilog Configuration 
// Purpose: Configure logging before the app starts to catch startup errors.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Requirement: Log at appropriate levels 
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "FoodSafetyTracker") // Requirement: Enriched properties 
    .WriteTo.Console() // Requirement: Console sink 
    .WriteTo.File("logs/food-safety-.txt", rollingInterval: RollingInterval.Day) // Requirement: Rolling file sink 
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // 2. Integration: Use Serilog instead of default logger
    builder.Host.UseSerilog();

    // 3. Database Connection (EF Core + SQL Server/SQLite) 
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    // 4. Identity & Roles 
    // Note: We add .AddRoles to ensure we can use Admin, Inspector, and Viewer roles later.
    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    // 5. Global Error Handling & Middleware 
    // Purpose: Ensure "friendly failures" for users and log errors for developers. 
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error"); // Friendly error page 
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication(); // Required for Identity 
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    // Purpose: Initialize the database with seed data if it's empty.
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        await context.Database.MigrateAsync(); // Apply pending migrations
        await DbSeeder.SeedRolesAndUsersAsync(roleManager, userManager); // Set up security
        await DbSeeder.SeedAsync(context); // Set up test data
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly"); // Requirement: Log caught exceptions 
}
finally
{
    Log.CloseAndFlush(); // Ensure all logs are written before closing
}