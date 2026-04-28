using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1293481_TravelerManagementByMarufa.ViewModels
{
    public class TravelerInfoViewModel
    {
        public int TravelerId { get; set; }
        public string TravelerName { get; set; }
        public string MobileNo { get; set; }
        public string NID { get; set; }
        public string IsRegular { get; set; } 
        public string DepartureFrom { get; set; }
        public int PassengerNo { get; set; }
        public DateTime TripStartDate { get; set; }
        public DateTime TripEndDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ImageUrl { get; set; }
        public byte[] ImageBinary { get; set; } 

        public int TripPackageId { get; set; }
        public string TripPackageName { get; set; }
        public decimal BookingAmount { get; set; }

       
        public int TravelPlanId { get; set; }
        public string TouristSpots { get; set; } 
        public string TravelMode { get; set; }
        public int EstimatedHour { get; set; }
    }
}
