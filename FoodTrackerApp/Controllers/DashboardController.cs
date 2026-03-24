using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodTrackerApp.Data;
using FoodTrackerApp.Models;
using Serilog; 

namespace FoodTrackerApp.Controllers;

// Purpose: Controller to handle the Dashboard logic and reporting queries. 
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Dashboard
    public async Task<IActionResult> Index(string? town, string? risk)
    {
        // Purpose: Log dashboard access with Serilog enrichment. 
        Log.Information("User accessed Dashboard. Filters: Town={Town}, Risk={Risk}", town ?? "All", risk ?? "All");

        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        // 1. Base query for Premises to apply filters. 
        var premisesQuery = _context.Premises.AsQueryable();

        if (!string.IsNullOrEmpty(town))
        {
            premisesQuery = premisesQuery.Where(p => p.Town == town);
        }

        if (!string.IsNullOrEmpty(risk))
        {
            premisesQuery = premisesQuery.Where(p => p.RiskRating == risk);
        }

        // 2. Aggregate Data for the ViewModel. 
        var viewModel = new DashboardViewModel
        {
            SelectedTown = town,
            SelectedRisk = risk,
            // Get distinct towns for the filter dropdown. 
            Towns = await _context.Premises.Select(p => p.Town).Distinct().ToListAsync(),

            // Count inspections this month for filtered premises. 
            MonthlyInspectionsCount = await _context.Inspections
                .Where(i => premisesQuery.Any(p => p.Id == i.PremisesId))
                .CountAsync(i => i.InspectionDate >= firstDayOfMonth),

            // Count failed inspections this month. 
            MonthlyFailedCount = await _context.Inspections
                .Where(i => premisesQuery.Any(p => p.Id == i.PremisesId))
                .CountAsync(i => i.InspectionDate >= firstDayOfMonth && i.Outcome == "Fail"),

            // Count overdue follow-ups: DueDate < Today AND Status Open. 
            OverdueFollowUpsCount = await _context.FollowUps
                .Include(f => f.Inspection)
                .Where(f => premisesQuery.Any(p => p.Id == f.Inspection.PremisesId))
                .CountAsync(f => f.Status == "Open" && f.DueDate < today)
        };

        return View(viewModel);
    }
}