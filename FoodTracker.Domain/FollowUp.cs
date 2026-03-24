using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTracker.Domain
{
    // Purpose: Tracks required actions after an inspection 
    public class FollowUp
    {
        public int Id { get; set; } // Unique identifier 
        public int InspectionId { get; set; } // Foreign Key to Inspection
        public DateTime DueDate { get; set; } 
        public string Status { get; set; } // Open or Closed 
        public DateTime? ClosedDate { get; set; } // Nullable if still open 

        // Navigation property 
        public Inspection Inspection { get; set; } = null!;
    }
}
