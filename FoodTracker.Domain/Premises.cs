using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTracker.Domain
{
    // Purpose: Represents a food business location to be inspected. 
    public class Premises
    {
        public int Id { get; set; } // Unique identifier 
        public string Name { get; set; } 
        public string Address { get; set; } 
        public string Town { get; set; } // Used for dashboard filtering
        public string RiskRating { get; set; } // Options: Low, Medium, High 

        // Relationship: One premises can have many inspections. 
        public List<Inspection> Inspections { get; set; } = new();
    }
}
