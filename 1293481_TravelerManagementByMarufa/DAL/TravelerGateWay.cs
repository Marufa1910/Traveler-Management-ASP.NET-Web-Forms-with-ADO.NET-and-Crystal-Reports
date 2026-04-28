using _1293481_TravelerManagementByMarufa.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _1293481_TravelerManagementByMarufa.DAL
{
    public class TravelerGateWay
    {
       
        string conStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        public DataTable GetAllTripPackages()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = "SELECT * FROM TripPackage";
                SqlDataAdapter da = new SqlDataAdapter(query, sqlcon);
                da.Fill(dt);
            }
            return dt;
        }

        public int SaveTraveler(Traveler traveler)
        {
            int travelerId = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        string travelerQuery = @"INSERT INTO Traveler (TravelerName, MobileNo, IsRegular, NID, DepartureFrom, NoOfPersonsToGo, TravelStartDate, TravelEndDate, ImageUrl) 
                                               VALUES (@Name, @Mob, @IsReg, @NID, @Dep, @Persons, @Start, @End, @Img); 
                                               SELECT SCOPE_IDENTITY();";

                        SqlCommand cmd = new SqlCommand(travelerQuery, sqlcon, tran);
                        cmd.Parameters.AddWithValue("@Name", traveler.TravelerName);
                        cmd.Parameters.AddWithValue("@Mob", traveler.MobileNo);
                        cmd.Parameters.AddWithValue("@IsReg", traveler.IsRegular);
                        cmd.Parameters.AddWithValue("@NID", traveler.NID);
                        cmd.Parameters.AddWithValue("@Dep", traveler.DepartureFrom);
                        cmd.Parameters.AddWithValue("@Persons", traveler.NoOfPersonsToGo);
                        cmd.Parameters.AddWithValue("@Start", traveler.TravelStartDate);
                        cmd.Parameters.AddWithValue("@End", (object)traveler.TravelEndDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Img", (object)traveler.ImageUrl ?? DBNull.Value);

                        travelerId = Convert.ToInt32(cmd.ExecuteScalar());

                       
                        foreach (var plan in traveler.TravelPlans)
                        {
                            string planQuery = @"INSERT INTO TravelPlan (TravelerId, TripPackageId, DesiredPlacesToVisit, TravelMode, EstimatedHour) 
                                               VALUES (@TId, @PId, @Places, @Mode, @Hour)";

                            SqlCommand pcmd = new SqlCommand(planQuery, sqlcon, tran);
                            pcmd.Parameters.AddWithValue("@TId", travelerId);
                            pcmd.Parameters.AddWithValue("@PId", plan.TripPackageId);
                            pcmd.Parameters.AddWithValue("@Places", plan.DesiredPlacesToVisit);
                            pcmd.Parameters.AddWithValue("@Mode", plan.TravelMode);
                            pcmd.Parameters.AddWithValue("@Hour", plan.EstimatedHour);
                            pcmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return travelerId;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }


        public DataTable GetAllTravelerInfo()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                
                string query = @"SELECT
                            t.TravelerId, 
                            t.TravelerName AS Name,
                            t.ImageUrl, 
                            t.MobileNo AS Mobile, 
                            t.NID,
                            t.DepartureFrom,
                            t.NoOfPersonsToGo AS PassengerCount,
                            t.TravelStartDate AS StartDate,
                            t.TravelEndDate AS EndDate,
                            CASE WHEN t.IsRegular = 1 THEN 'Regular' ELSE 'Seasonal' END AS Status
                         FROM Traveler t";

                SqlDataAdapter da = new SqlDataAdapter(query, sqlcon);
                da.Fill(dt);
            }
            return dt;
        }
        public DataTable GetPlansByTravelerId(int travelerId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = @"SELECT tp.TravelPlanId, tp.TravelerId, tp.TripPackageId, 
                               tp.DesiredPlacesToVisit, tp.TravelMode, tp.EstimatedHour 
                        FROM TravelPlan tp 
                        WHERE tp.TravelerId = @TravelerId";

                using (SqlCommand cmd = new SqlCommand(query, sqlcon))
                {
                    cmd.Parameters.AddWithValue("@TravelerId", travelerId);
                    sqlcon.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }


        public int UpdateTraveler(Traveler traveler)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        string updateSql = @"UPDATE Traveler SET TravelerName=@Name, MobileNo=@Mob, IsRegular=@IsReg, 
                                            NID=@NID, DepartureFrom=@Dep, NoOfPersonsToGo=@Persons, 
                                            TravelStartDate=@Start, TravelEndDate=@End, ImageUrl=@Img 
                                            WHERE TravelerId=@Id";

                        SqlCommand cmd = new SqlCommand(updateSql, sqlcon, tran);
                        cmd.Parameters.AddWithValue("@Id", traveler.TravelerId);
                        cmd.Parameters.AddWithValue("@Name", traveler.TravelerName);
                        cmd.Parameters.AddWithValue("@Mob", traveler.MobileNo);
                        cmd.Parameters.AddWithValue("@IsReg", traveler.IsRegular);
                        cmd.Parameters.AddWithValue("@NID", traveler.NID);
                        cmd.Parameters.AddWithValue("@Dep", traveler.DepartureFrom);
                        cmd.Parameters.AddWithValue("@Persons", traveler.NoOfPersonsToGo);
                        cmd.Parameters.AddWithValue("@Start", traveler.TravelStartDate);
                        cmd.Parameters.AddWithValue("@End", (object)traveler.TravelEndDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Img", (object)traveler.ImageUrl ?? DBNull.Value);
                        count = cmd.ExecuteNonQuery();

                        SqlCommand delCmd = new SqlCommand("DELETE FROM TravelPlan WHERE TravelerId=@Id", sqlcon, tran);
                        delCmd.Parameters.AddWithValue("@Id", traveler.TravelerId);
                        delCmd.ExecuteNonQuery();

                        foreach (var plan in traveler.TravelPlans)
                        {
                            SqlCommand pcmd = new SqlCommand(@"INSERT INTO TravelPlan (TravelerId, TripPackageId, DesiredPlacesToVisit, TravelMode, EstimatedHour) 
                                                              VALUES (@TId, @PId, @Places, @Mode, @Hour)", sqlcon, tran);
                            pcmd.Parameters.AddWithValue("@TId", traveler.TravelerId);
                            pcmd.Parameters.AddWithValue("@PId", plan.TripPackageId);
                            pcmd.Parameters.AddWithValue("@Places", plan.DesiredPlacesToVisit);
                            pcmd.Parameters.AddWithValue("@Mode", plan.TravelMode);
                            pcmd.Parameters.AddWithValue("@Hour", plan.EstimatedHour);
                            pcmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                    }
                }
            }
            return count;
        }
           
        public int DeleteTraveler(int travelerId)
        {
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {

                        SqlCommand cmdTrav = new SqlCommand("DELETE FROM Traveler WHERE TravelerId=@Id", sqlcon, tran);
                        cmdTrav.Parameters.AddWithValue("@Id", travelerId);
                        int count = cmdTrav.ExecuteNonQuery();

                        tran.Commit();
                        return count;
                    }
                    catch { tran.Rollback(); return 0; }
                }
            }
        }

        public int DeleteTravelPlanByTravelerId(int travelerId, int travelPlanId)
        {
            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {

                        string sql = "DELETE FROM TravelPlan WHERE TravelerId=@TravelerId AND TravelPlanId=@TravelPlanId";

                        using (SqlCommand deletePlanCmd = new SqlCommand(sql, sqlcon, tran))
                        {
                            deletePlanCmd.CommandType = CommandType.Text;

                            // Matching parameters to your method arguments
                            deletePlanCmd.Parameters.Add("@TravelerId", SqlDbType.Int).Value = travelerId;
                            deletePlanCmd.Parameters.Add("@TravelPlanId", SqlDbType.Int).Value = travelPlanId;

                            count = deletePlanCmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }

        public DataTable GetTravelerById(int travelerId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = @"SELECT t.TravelerId, t.TravelerName, t.MobileNo, t.NID, 
                                t.DepartureFrom, t.NoOfPersonsToGo, 
                                Format(t.TravelStartDate,'yyyy-MM-dd') AS TravelStartDate,
                                Format(t.TravelEndDate,'yyyy-MM-dd') AS TravelEndDate,
                                t.ImageUrl, t.IsRegular,
                                CASE WHEN t.IsRegular = 1 THEN 'Regular' ELSE 'Seasonal' END AS Status
                         FROM Traveler t 
                         WHERE t.TravelerId = @TravelerId";

                SqlCommand cmd = new SqlCommand(query, sqlcon);
                cmd.Parameters.AddWithValue("@TravelerId", travelerId);

                sqlcon.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int DeleteTravelerInfoByTravelerId(int tId)
        {

            int count = 0;
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                using (SqlTransaction tran = sqlcon.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteTravelPlanCmd = new SqlCommand("DELETE FROM TravelPlan WHERE TravelerId=@TId ", sqlcon, tran))
                        {
                            deleteTravelPlanCmd.CommandType = CommandType.Text;
                            deleteTravelPlanCmd.Parameters.Add("@TId", SqlDbType.Int).Value = tId;

                            count = deleteTravelPlanCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }

        public DataTable GetAllTravelerInfoForReport()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                string query = @"SELECT 
                            t.TravelerId, 
                            t.TravelerName, 
                            t.MobileNo, 
                            t.NID, 
                            t.DepartureFrom, 
                            t.NoOfPersonsToGo, 
                            t.TravelStartDate, 
                            t.TravelEndDate, 
                            t.ImageUrl,
                            CASE WHEN t.IsRegular = 1 THEN 'Regular' ELSE 'Seasonal' END AS IsRegular,
                            tp.TripPackageId,
                            tp.TripPackageName, 
                            tp.BookingAmount,
                            p.TravelPlanId,
                            p.DesiredPlacesToVisit, 
                            p.TravelMode, 
                            p.EstimatedHour
                         FROM Traveler t
                         INNER JOIN TravelPlan p ON t.TravelerId = p.TravelerId
                         INNER JOIN TripPackage tp ON p.TripPackageId = tp.TripPackageId
                         ORDER BY t.TravelerId ASC";

                SqlDataAdapter da = new SqlDataAdapter(query, sqlcon);
                da.Fill(dt);
            }
            return dt;
        }
    }
}

