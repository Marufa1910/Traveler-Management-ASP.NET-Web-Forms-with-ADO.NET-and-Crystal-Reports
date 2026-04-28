using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1293481_TravelerManagementByMarufa.Entities
{
    public class Traveler
    {
        public int TravelerId { get; set; }
        public string TravelerName { get; set; }
        public string MobileNo { get; set; }
        public bool IsRegular { get; set; } 
        public string NID { get; set; }
        public string DepartureFrom { get; set; }
        public int NoOfPersonsToGo { get; set; }
        public DateTime TravelStartDate { get; set; }
        public DateTime? TravelEndDate { get; set; } 
        public DateTime RegistrationDate { get; set; }

        public string ImageUrl { get; set; }
        
        public List<TravelPlan> TravelPlans { get; set; } = new List<TravelPlan>();
    }
}
