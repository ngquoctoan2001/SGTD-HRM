namespace HRM.Application.DTOs.Dashboard;

public class DashboardOverviewDto
{
    public int TotalEmployees { get; set; }
    public double MonthlyAttendancePercentage { get; set; }
    public int PendingLeaveRequests { get; set; }
    public int UpcomingInterviewsCount { get; set; }
    public List<RecruitmentTrendDto> RecruitmentTrend { get; set; } = new();
    public List<RecentLeaveRequestDto> RecentLeaveRequests { get; set; } = new();
    public List<UpcomingInterviewDto> UpcomingInterviews { get; set; } = new();
}

public class RecruitmentTrendDto
{
    public string Month { get; set; } = string.Empty;
    public int Hired { get; set; }
    public int Applications { get; set; }
}

public class RecentLeaveRequestDto
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeAvatar { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class UpcomingInterviewDto
{
    public int Id { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string Time { get; set; } = string.Empty;
}
