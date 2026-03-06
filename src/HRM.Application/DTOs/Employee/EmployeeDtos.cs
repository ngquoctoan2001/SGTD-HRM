namespace HRM.Application.DTOs.Employee;

public class EmployeeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
}

public class CreateEmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
}

public class UpdateEmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
}

public class EmployeeProfileDto : EmployeeDto
{
    public List<ContractDto> Contracts { get; set; } = new();
    public List<DisciplineRewardDto> DisciplineRewards { get; set; } = new();
    public List<LeaveRequestDto> LeaveRequests { get; set; } = new();
    public List<AttendanceRecordDto> AttendanceRecords { get; set; } = new();
    public List<PerformanceReviewDto> PerformanceReviews { get; set; } = new();
}
