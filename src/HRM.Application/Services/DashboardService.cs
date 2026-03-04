using AutoMapper;
using HRM.Application.DTOs.Common;
using HRM.Application.DTOs.Dashboard;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using HRM.Domain.Interfaces;

namespace HRM.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IRepository<Employee> _employeeRepo;
    private readonly IRepository<AttendanceRecord> _attendanceRepo;
    private readonly IRepository<LeaveRequest> _leaveRepo;
    private readonly IRepository<InterviewSchedule> _interviewRepo;
    private readonly IRepository<Candidate> _candidateRepo;
    private readonly IMapper _mapper;

    public DashboardService(
        IRepository<Employee> employeeRepo,
        IRepository<AttendanceRecord> attendanceRepo,
        IRepository<LeaveRequest> leaveRepo,
        IRepository<InterviewSchedule> interviewRepo,
        IRepository<Candidate> candidateRepo,
        IMapper mapper)
    {
        _employeeRepo = employeeRepo;
        _attendanceRepo = attendanceRepo;
        _leaveRepo = leaveRepo;
        _interviewRepo = interviewRepo;
        _candidateRepo = candidateRepo;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DashboardOverviewDto>> GetOverviewAsync()
    {
        // Total Employees
        var totalEmployees = await _employeeRepo.CountAsync(e => e.Status == EmployeeStatus.Active);

        // Monthly Attendance %
        var now = DateTime.UtcNow;
        var firstOfMonth = new DateOnly(now.Year, now.Month, 1);
        var totalAttendance = await _attendanceRepo.CountAsync(a => a.Date >= firstOfMonth);
        var presentAttendance = await _attendanceRepo.CountAsync(a => a.Date >= firstOfMonth && a.Status == AttendanceStatus.Present);
        var attendancePercent = totalAttendance > 0 ? Math.Round((double)presentAttendance / totalAttendance * 100, 1) : 0;

        // Pending Leave Requests
        var pendingLeaves = await _leaveRepo.CountAsync(l => l.Status == LeaveStatus.Pending);

        // Upcoming Interviews (next 30 days)
        var upcomingDate = DateTime.UtcNow;
        var upcomingInterviews = await _interviewRepo.FindAsync(
            i => i.InterviewDate >= upcomingDate && i.Status == InterviewStatus.Scheduled,
            i => i.Candidate, i => i.Candidate.JobPosting);
        var upcomingList = upcomingInterviews.OrderBy(i => i.InterviewDate).Take(5).ToList();

        // Recruitment Trend (last 6 months)
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        var allCandidates = await _candidateRepo.FindAsync(c => c.AppliedDate >= sixMonthsAgo);
        var recruitmentTrend = Enumerable.Range(0, 6).Select(i =>
        {
            var monthDate = DateTime.UtcNow.AddMonths(-5 + i);
            var monthCandidates = allCandidates.Where(c =>
                c.AppliedDate.Month == monthDate.Month && c.AppliedDate.Year == monthDate.Year);
            return new RecruitmentTrendDto
            {
                Month = monthDate.ToString("MMM"),
                Applications = monthCandidates.Count(),
                Hired = monthCandidates.Count(c => c.Status == CandidateStatus.Hired)
            };
        }).ToList();

        // Recent Leave Requests (latest 5)
        var (recentLeaves, _) = await _leaveRepo.GetPagedAsync(1, 5,
            null, null, l => l.Employee);
        var recentLeavesDtos = _mapper.Map<List<RecentLeaveRequestDto>>(recentLeaves);

        // Map upcoming interviews
        var upcomingDtos = _mapper.Map<List<UpcomingInterviewDto>>(upcomingList);

        var overview = new DashboardOverviewDto
        {
            TotalEmployees = totalEmployees,
            MonthlyAttendancePercentage = attendancePercent,
            PendingLeaveRequests = pendingLeaves,
            UpcomingInterviewsCount = upcomingInterviews.Count(),
            RecruitmentTrend = recruitmentTrend,
            RecentLeaveRequests = recentLeavesDtos,
            UpcomingInterviews = upcomingDtos
        };

        return ApiResponse<DashboardOverviewDto>.Ok(overview);
    }
}
