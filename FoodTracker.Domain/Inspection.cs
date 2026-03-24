using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTracker.Domain
{
    // Purpose: Records the details and outcome of a specific food safety visit. 
    public class Inspection
    {
        public int Id { get; set; } // Unique identifier 
        public int PremisesId { get; set; } // Foreign Key to Premises 
        public DateTime InspectionDate { get; set; }  
        public int Score { get; set; } // Range: 0-100 
        public string Outcome { get; set; } // Pass or Fail 
        public string? Notes { get; set; } 

        // Navigation properties 
        public Premises Premises { get; set; } = null!;
        public List<FollowUp> FollowUps { get; set; } = new();
    }
}
