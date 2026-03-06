namespace HRM.Application.DTOs;

// ==================== DEPARTMENT ====================
public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}


// ==================== ATTENDANCE ====================
public class AttendanceRecordDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class CreateAttendanceDto
{
    public int EmployeeId { get; set; }
    public string Date { get; set; } = string.Empty;
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public string Status { get; set; } = "Present";
    public string Notes { get; set; } = string.Empty;
}

// ==================== LEAVE REQUEST ====================
public class LeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
}

public class CreateLeaveRequestDto
{
    public int EmployeeId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class UpdateLeaveStatusDto
{
    public string Status { get; set; } = string.Empty;
}

// ==================== PAYROLL ====================
public class PayrollSlipDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string MonthYear { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreatePayrollSlipDto
{
    public int EmployeeId { get; set; }
    public string MonthYear { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
}

// ==================== JOB POSTING ====================
public class JobPostingDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string SalaryRange { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }
    public int CandidateCount { get; set; }
}

public class CreateJobPostingDto
{
    public string Title { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Location { get; set; } = string.Empty;
    public string SalaryRange { get; set; } = string.Empty;
}

// ==================== CANDIDATE ====================
public class CandidateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int JobPostingId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedDate { get; set; }
}

public class CreateCandidateDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int JobPostingId { get; set; }
}

public class UpdateCandidateStatusDto
{
    public string Status { get; set; } = string.Empty;
}

// ==================== INTERVIEW SCHEDULE ====================
public class InterviewScheduleDto
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string InterviewType { get; set; } = string.Empty;
    public string LocationOrLink { get; set; } = string.Empty;
    public string InterviewerName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class CreateInterviewScheduleDto
{
    public int CandidateId { get; set; }
    public DateTime InterviewDate { get; set; }
    public string InterviewType { get; set; } = "Online";
    public string LocationOrLink { get; set; } = string.Empty;
    public string InterviewerName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

// ==================== PERFORMANCE REVIEW ====================
public class PerformanceReviewDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewDate { get; set; } = string.Empty;
    public string ReviewPeriod { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Goals { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class CreatePerformanceReviewDto
{
    public int EmployeeId { get; set; }
    public int ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewDate { get; set; } = string.Empty;
    public string ReviewPeriod { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Goals { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
}

// ==================== ASSET ====================
public class AssetDto
{
    public int Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PurchaseDate { get; set; } = string.Empty;
}

public class CreateAssetDto
{
    public string AssetTag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "Electronics";
    public string PurchaseDate { get; set; } = string.Empty;
}

// ==================== ASSET ASSIGNMENT ====================
public class AssetAssignmentDto
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string AssignedDate { get; set; } = string.Empty;
    public string? ReturnedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CreateAssetAssignmentDto
{
    public int AssetId { get; set; }
    public int EmployeeId { get; set; }
    public string AssignedDate { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
