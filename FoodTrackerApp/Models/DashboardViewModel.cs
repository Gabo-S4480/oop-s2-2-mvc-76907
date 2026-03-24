// Purpose: A ViewModel to hold aggregated data for the Dashboard page
// Location: FoodTrackerApp/Models/DashboardViewModel.cs

namespace FoodTrackerApp.Models
{
    public class DashboardViewModel
    {
        // Total inspections for the current month
        public int MonthlyInspectionsCount { get; set; }

        // Total failed inspections for the current month
        public int MonthlyFailedCount { get; set; }

        // Logic: Count where DueDate < Today AND Status is Open
        public int OverdueFollowUpsCount { get; set; }

        // Filter: Town selection for grouped queries
        public string? SelectedTown { get; set; }

        // Filter: RiskRating selection (Low/Medium/High)
        public string? SelectedRisk { get; set; }

        // List of towns to populate the dropdown filter
        public List<string> Towns { get; set; } = new();
    }
}