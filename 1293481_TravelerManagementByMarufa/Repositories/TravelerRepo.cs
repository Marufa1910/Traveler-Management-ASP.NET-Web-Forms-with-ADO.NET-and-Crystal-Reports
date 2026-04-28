using _1293481_TravelerManagementByMarufa.DAL;
using _1293481_TravelerManagementByMarufa.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _1293481_TravelerManagementByMarufa.Repositories
{
    public class TravelerRepo
    {
        TravelerGateWay dal = new TravelerGateWay();

        public DataTable GetAllTripPackages()
        {
            DataTable dt = dal.GetAllTripPackages();
            return dt;
        }

        public DataTable GetAllTravelers()
        {
            DataTable dt = dal.GetAllTravelerInfo();
            return dt;
        }
        public int SaveTraveler(Traveler traveler)
        {
            int saveCount = dal.SaveTraveler(traveler);
            return saveCount;
        }

        public DataTable GetPlansByTravelerId(int travelerId)
        {
            DataTable dt = dal.GetPlansByTravelerId(travelerId);
            return dt;
        }

        public int UpdateTraveler(Traveler traveler)
        {
            int updateCount = dal.UpdateTraveler(traveler);
            return updateCount;
        }

        public int DeleteTraveler(int travelerId)
        {
            int count = dal.DeleteTraveler(travelerId);
            return count;
        }

        public DataTable GetFullTravelerReport()
        {
            DataTable dt = dal.GetAllTravelerInfo();
            return dt;
        }

        public int DeleteTavelPlanByTravelerId(int travelerId, int travelPlanId)
        {
            int deleteResult = dal.DeleteTravelPlanByTravelerId( travelerId,travelPlanId);
            return deleteResult;
        }

        public DataTable GetTravelerById(int travelerId)
        {

            DataTable dt = new DataTable();
            dt = dal.GetTravelerById(travelerId);
            return dt;
        }

        public int DeleteTravelerInfoByTravelerId(int tId)
        {
            int deleteResult = dal.DeleteTravelerInfoByTravelerId( tId);
            return deleteResult;
        }

        public DataTable GetAllTravelerInfoForReport()
        {
        
            DataTable dt = new DataTable();
            dt = dal.GetAllTravelerInfoForReport();
            return dt;
        }
    }
    
}
