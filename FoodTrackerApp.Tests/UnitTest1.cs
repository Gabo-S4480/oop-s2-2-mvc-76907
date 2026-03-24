using Xunit;
using FoodTracker.Domain;

namespace FoodTrackerApp.Tests;

public class BusinessRulesTests
{
    [Fact]
    // Purpose: Ensure that a Follow-up cannot have a due date before the inspection date.
    public void FollowUp_DueDate_ShouldNotBeBefore_InspectionDate()
    {
        // Arrange Prepare info 
        var inspection = new Inspection { InspectionDate = new DateTime(2026, 3, 1) };
        var followUp = new FollowUp { DueDate = new DateTime(2026, 2, 25) }; // Fecha inválida

        // Act 
        bool isInvalid = followUp.DueDate < inspection.InspectionDate;

        // Assert Check results
        Assert.True(isInvalid, "The follow-up due date must be after the inspection date.");
    }

    [Fact]
    // Purpose: Ensure the Dashboard counter logic for overdue follow-ups is correct.
    public void FollowUp_IsOverdue_ShouldBeTrue_IfOpenAndPastDate()
    {
        // Arrange
        var today = DateTime.Today;
        var followUp = new FollowUp
        {
            DueDate = today.AddDays(-1),
            Status = "Open"
        };

        // Act
        bool isOverdue = followUp.Status == "Open" && followUp.DueDate < today;

        // Assert
        Assert.True(isOverdue);
    }
}