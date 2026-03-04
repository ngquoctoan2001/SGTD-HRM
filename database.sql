-- =============================
-- USER
-- =============================
CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    Name NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    Bio NVARCHAR(MAX) NOT NULL,
    AvatarUrl NVARCHAR(500) NOT NULL,

    CONSTRAINT UQ_User_Username UNIQUE (Username),
    CONSTRAINT UQ_User_Email UNIQUE (Email),
    CONSTRAINT CK_User_Role CHECK (Role IN ('Admin','User'))
);

-- =============================
-- EMPLOYEE
-- =============================
CREATE TABLE Employee (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Department NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
    Avatar NVARCHAR(500) NOT NULL,
    JoinDate DATETIME2 NOT NULL,

    CONSTRAINT UQ_Employee_Email UNIQUE (Email),
    CONSTRAINT CK_Employee_Status CHECK (Status IN ('Active','Inactive','Resigned'))
);

-- =============================
-- JOB POSTING
-- =============================
CREATE TABLE JobPosting (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Department NVARCHAR(200) NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    SalaryRange NVARCHAR(200) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Hiring',
    PostedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_JobPosting_Status CHECK (Status IN ('Hiring','Closed'))
);

-- =============================
-- CANDIDATE
-- =============================
CREATE TABLE Candidate (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    JobPostingId INT NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'New',
    AppliedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Candidate_JobPosting
        FOREIGN KEY (JobPostingId)
        REFERENCES JobPosting(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_Candidate_Status 
        CHECK (Status IN ('New','Interviewing','Hired','Rejected'))
);

CREATE INDEX IX_Candidate_JobPostingId ON Candidate(JobPostingId);

-- =============================
-- INTERVIEW SCHEDULE
-- =============================
CREATE TABLE InterviewSchedule (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CandidateId INT NOT NULL,
    InterviewDate DATETIME2 NOT NULL,
    InterviewType NVARCHAR(50) NOT NULL DEFAULT 'Online',
    LocationOrLink NVARCHAR(500) NOT NULL,
    InterviewerName NVARCHAR(200) NOT NULL,
    Notes NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Scheduled',

    CONSTRAINT FK_Interview_Candidate
        FOREIGN KEY (CandidateId)
        REFERENCES Candidate(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_Interview_Type 
        CHECK (InterviewType IN ('Online','In-Person')),

    CONSTRAINT CK_Interview_Status
        CHECK (Status IN ('Scheduled','Completed','Cancelled'))
);

CREATE INDEX IX_InterviewSchedule_CandidateId 
ON InterviewSchedule(CandidateId);

-- =============================
-- ATTENDANCE RECORD
-- =============================
CREATE TABLE AttendanceRecord (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    Date DATE NOT NULL,
    CheckInTime TIME NULL,
    CheckOutTime TIME NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Present',
    Notes NVARCHAR(MAX) NOT NULL,

    CONSTRAINT FK_Attendance_Employee
        FOREIGN KEY (EmployeeId)
        REFERENCES Employee(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_Attendance_Status 
        CHECK (Status IN ('Present','Absent','Late','Half-Day'))
);

CREATE INDEX IX_Attendance_EmployeeId ON AttendanceRecord(EmployeeId);
CREATE INDEX IX_Attendance_Date ON AttendanceRecord(Date);

-- =============================
-- LEAVE REQUEST
-- =============================
CREATE TABLE LeaveRequest (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Reason NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    RequestedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Leave_Employee
        FOREIGN KEY (EmployeeId)
        REFERENCES Employee(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_Leave_Type 
        CHECK (Type IN ('Annual','Sick','Unpaid')),

    CONSTRAINT CK_Leave_Status 
        CHECK (Status IN ('Pending','Approved','Rejected'))
);

CREATE INDEX IX_Leave_EmployeeId ON LeaveRequest(EmployeeId);

-- =============================
-- PAYROLL SLIP
-- =============================
CREATE TABLE PayrollSlip (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    MonthYear NVARCHAR(20) NOT NULL,
    BasicSalary DECIMAL(18,2) NOT NULL,
    TotalAllowances DECIMAL(18,2) NOT NULL,
    TotalDeductions DECIMAL(18,2) NOT NULL,
    NetSalary DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Payroll_Employee
        FOREIGN KEY (EmployeeId)
        REFERENCES Employee(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_Payroll_Status
        CHECK (Status IN ('Draft','Approved','Paid'))
);

CREATE INDEX IX_Payroll_EmployeeId ON PayrollSlip(EmployeeId);

-- =============================
-- PERFORMANCE REVIEW
-- =============================
CREATE TABLE PerformanceReview (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    ReviewerId INT NOT NULL,
    ReviewerName NVARCHAR(200) NOT NULL,
    ReviewDate DATE NOT NULL,
    ReviewPeriod NVARCHAR(100) NOT NULL,
    Rating INT NOT NULL,
    Goals NVARCHAR(MAX) NOT NULL,
    Feedback NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',

    CONSTRAINT FK_Review_Employee
        FOREIGN KEY (EmployeeId)
        REFERENCES Employee(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_Review_Rating 
        CHECK (Rating BETWEEN 1 AND 5),

    CONSTRAINT CK_Review_Status 
        CHECK (Status IN ('Draft','Submitted','Acknowledged'))
);

CREATE INDEX IX_PerformanceReview_EmployeeId 
ON PerformanceReview(EmployeeId);

-- =============================
-- ASSET
-- =============================
CREATE TABLE Asset (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AssetTag NVARCHAR(100) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100) NOT NULL DEFAULT 'Electronics',
    Status NVARCHAR(100) NOT NULL DEFAULT 'Available',
    PurchaseDate DATE NOT NULL,

    CONSTRAINT UQ_Asset_AssetTag UNIQUE (AssetTag),

    CONSTRAINT CK_Asset_Category 
        CHECK (Category IN ('Electronics','Furniture','Vehicle')),

    CONSTRAINT CK_Asset_Status
        CHECK (Status IN ('Available','Assigned','Maintenance','Retired'))
);

-- =============================
-- ASSET ASSIGNMENT
-- =============================
CREATE TABLE AssetAssignment (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AssetId INT NOT NULL,
    EmployeeId INT NOT NULL,
    AssignedDate DATE NOT NULL,
    ReturnedDate DATE NULL,
    Notes NVARCHAR(MAX) NOT NULL,

    CONSTRAINT FK_AssetAssignment_Asset
        FOREIGN KEY (AssetId)
        REFERENCES Asset(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_AssetAssignment_Employee
        FOREIGN KEY (EmployeeId)
        REFERENCES Employee(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_AssetAssignment_AssetId ON AssetAssignment(AssetId);
CREATE INDEX IX_AssetAssignment_EmployeeId ON AssetAssignment(EmployeeId);