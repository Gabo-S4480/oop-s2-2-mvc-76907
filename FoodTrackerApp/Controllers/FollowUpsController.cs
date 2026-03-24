using FoodTracker.Domain;
using FoodTrackerApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Serilog; 

namespace FoodTrackerApp.Controllers
{
    public class FollowUpsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FollowUpsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FollowUps
        public async Task<IActionResult> Index()
        {
            // Include related Inspection data for display in the view.
            var applicationDbContext = _context.FollowUps.Include(f => f.Inspection);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FollowUp followUp)
        {
            // 1. search the related dates
            var inspection = await _context.Inspections.FindAsync(followUp.InspectionId);

            if (inspection != null)
            {
                // 2. Rule bussiness 
                if (followUp.DueDate < inspection.InspectionDate)
                {
                    // Serilog log for business rule violation
                    Log.Warning("Business Rule Violation: FollowUp DueDate {DueDate} is before InspectionDate {InspectionDate}",
                        followUp.DueDate, inspection.InspectionDate);

                    // Show error to the user
                    ModelState.AddModelError("DueDate", "Follow-up date cannot be before the inspection date.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(followUp);
                await _context.SaveChangesAsync();

                Log.Information("Follow-up created for Inspection {InspectionId}", followUp.InspectionId); // Log info
                return RedirectToAction("Index", "Dashboard");
            }

            return View(followUp);
        }
    }
}