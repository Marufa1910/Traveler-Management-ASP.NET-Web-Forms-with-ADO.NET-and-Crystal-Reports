-- 1. Trip Package
CREATE TABLE [dbo].[TripPackage] (
    [TripPackageId] INT PRIMARY KEY IDENTITY(1,1),
    [TripPackageName] VARCHAR(50) NOT NULL,
    [BookingAmount] DECIMAL(18,2) NOT NULL DEFAULT 0
);

-- 2. Traveler
CREATE TABLE [dbo].[Traveler] (
    [TravelerId] INT PRIMARY KEY IDENTITY(1,1),
    [TravelerName] VARCHAR(50) NOT NULL,
    [MobileNo] VARCHAR(20) NOT NULL,
    [IsRegular] BIT DEFAULT 0,
    [NID] VARCHAR(20) UNIQUE,
    [DepartureFrom] VARCHAR(50) NOT NULL,
    [NoOfPersonsToGo] INT NOT NULL CHECK (NoOfPersonsToGo > 0),
    [TravelStartDate] DATE NOT NULL,
    [TravelEndDate] DATE NULL,
    [RegistrationDate] DATETIME DEFAULT GETDATE(),
    [ImageUrl] NVARCHAR(250) NULL,
    CONSTRAINT CHK_TravelDates CHECK (TravelEndDate IS NULL OR TravelEndDate >= TravelStartDate)
);

-- 3. TravelPlan 
CREATE TABLE [dbo].[TravelPlan] (
    [TravelPlanId] INT PRIMARY KEY IDENTITY(200,1),
    [TravelerId] INT NOT NULL FOREIGN KEY REFERENCES Traveler(TravelerId),
    [TripPackageId] INT NOT NULL FOREIGN KEY REFERENCES TripPackage(TripPackageId),
    [DesiredPlacesToVisit] VARCHAR(500) NOT NULL, 
    [TravelMode] VARCHAR(20) NOT NULL, 
    [EstimatedHour] INT NOT NULL CHECK (EstimatedHour > 0)
   
);

