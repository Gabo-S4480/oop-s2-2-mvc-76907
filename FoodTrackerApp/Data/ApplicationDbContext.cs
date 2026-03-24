using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodTracker.Domain; // Add reference to your Domain layer

namespace FoodTrackerApp.Data;

// Purpose: Connects the Domain models to the Database via Entity Framework.
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Registering the 3 required entities for the project 
    public DbSet<Premises> Premises { get; set; }
    public DbSet<Inspection> Inspections { get; set; }
    public DbSet<FollowUp> FollowUps { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Purpose: Define relationships between entities 

        // Premises 1-* Inspection 
        builder.Entity<Inspection>()
            .HasOne(i => i.Premises)
            .WithMany(p => p.Inspections)
            .HasForeignKey(i => i.PremisesId);

        // Inspection 1-* FollowUp 
        builder.Entity<FollowUp>()
            .HasOne(f => f.Inspection)
            .WithMany(i => i.FollowUps)
            .HasForeignKey(f => f.InspectionId);
    }
}