using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1293481_TravelerManagementByMarufa.Entities
{
    public class TravelPlan
    {
        public int TravelPlanId { get; set; }
        public int TravelerId { get; set; } 
        public int TripPackageId { get; set; }
        public string TripPackageName { get; set; }
        public string DesiredPlacesToVisit { get; set; }
        public string TravelMode { get; set; }
        public int EstimatedHour { get; set; }
    }
}
