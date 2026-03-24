using FoodTracker.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace FoodTrackerApp.Data;

// Purpose: Automatically populates the database with roles and test data on startup.
public static class DbSeeder
{
    // Purpose: Creates the security roles and a default admin user
    public static async Task SeedRolesAndUsersAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        // Define the 3 required roles according to the assessment 
        string[] roleNames = { "Admin", "Inspector", "Viewer" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create a default Admin user to allow initial login 
        var adminEmail = "admin@foodtracker.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Password123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // Purpose: Seeds the business data (Premises, Inspections, FollowUps)
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if database is already seeded to avoid duplicates
        if (context.Premises.Any()) return;

        // 1. Seed 12 Premises across 3 towns 
        var towns = new[] { "Dublin", "Cork", "Galway" };
        var ratings = new[] { "Low", "Medium", "High" };
        var premisesList = new List<Premises>();

        for (int i = 1; i <= 12; i++)
        {
            premisesList.Add(new Premises
            {
                Name = $"Shop {i}",
                Address = $"{i} Main St",
                Town = towns[i % 3],
                RiskRating = ratings[i % 3]
            });
        }
        context.Premises.AddRange(premisesList);
        await context.SaveChangesAsync();

        // 2. Seed 25 Inspections 
        var random = new Random();
        var inspections = new List<Inspection>();
        for (int i = 0; i < 25; i++)
        {
            var p = premisesList[random.Next(premisesList.Count)];
            inspections.Add(new Inspection
            {
                PremisesId = p.Id,
                InspectionDate = DateTime.Now.AddDays(-random.Next(1, 60)),
                Score = random.Next(40, 100),
                Outcome = i % 5 == 0 ? "Fail" : "Pass", // Every 5th fails to test dashboard
                Notes = "Routine inspection"
            });
        }
        context.Inspections.AddRange(inspections);
        await context.SaveChangesAsync();

        // 3. Seed 10 Follow-ups (some overdue, some closed) 
        var failedInspections = inspections.Where(x => x.Outcome == "Fail").ToList();
        for (int i = 0; i < 10 && i < failedInspections.Count; i++)
        {
            context.FollowUps.Add(new FollowUp
            {
                InspectionId = failedInspections[i].Id,
                // Some are overdue (DueDate < Today) to test the Dashboard requirement 
                DueDate = i < 5 ? DateTime.Now.AddDays(-5) : DateTime.Now.AddDays(10),
                Status = i % 2 == 0 ? "Open" : "Closed",
                ClosedDate = i % 2 != 0 ? DateTime.Now : null
            });
        }
        await context.SaveChangesAsync();
    }
}