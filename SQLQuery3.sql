SELECT 
                            t.TravelerId, 
                            t.TravelerName AS Name,
                            t.ImageUrl, 
                            t.MobileNo AS Mobile, 
                            t.NID,
                            tp.TripPackageName,
                            tp.BookingAmount,
                            t.DepartureFrom,
                            t.NoOfPersonsToGo AS PassengerCount,
                            t.TravelStartDate AS StartDate,
                            t.TravelEndDate AS EndDate,
                            CASE WHEN t.IsRegular = 1 THEN 'Regular' ELSE 'Seasonal' END AS Status                        
                         FROM Traveler t
                         LEFT JOIN TravelPlan p ON t.TravelerId = p.TravelerId
                         LEFT JOIN TripPackage tp ON p.TripPackageId = tp.TripPackageId
 