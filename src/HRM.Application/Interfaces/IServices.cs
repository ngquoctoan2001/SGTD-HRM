using HRM.Application.DTOs.Auth;
using HRM.Application.DTOs.Common;
using HRM.Application.DTOs.Dashboard;
using HRM.Application.DTOs.Employee;
using HRM.Application.DTOs;

namespace HRM.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
}

public interface IEmployeeService
{
    Task<ApiResponse<PagedResult<EmployeeDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<EmployeeDto>> GetByIdAsync(int id);
    Task<ApiResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto dto);
    Task<ApiResponse<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IDashboardService
{
    Task<ApiResponse<DashboardOverviewDto>> GetOverviewAsync();
}

public interface IAttendanceService
{
    Task<ApiResponse<PagedResult<AttendanceRecordDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<AttendanceRecordDto>> GetByIdAsync(int id);
    Task<ApiResponse<AttendanceRecordDto>> CreateAsync(CreateAttendanceDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface ILeaveService
{
    Task<ApiResponse<PagedResult<LeaveRequestDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<LeaveRequestDto>> GetByIdAsync(int id);
    Task<ApiResponse<LeaveRequestDto>> CreateAsync(CreateLeaveRequestDto dto);
    Task<ApiResponse<LeaveRequestDto>> UpdateStatusAsync(int id, UpdateLeaveStatusDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IPayrollService
{
    Task<ApiResponse<PagedResult<PayrollSlipDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<PayrollSlipDto>> GetByIdAsync(int id);
    Task<ApiResponse<PayrollSlipDto>> CreateAsync(CreatePayrollSlipDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IJobPostingService
{
    Task<ApiResponse<PagedResult<JobPostingDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<JobPostingDto>> GetByIdAsync(int id);
    Task<ApiResponse<JobPostingDto>> CreateAsync(CreateJobPostingDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface ICandidateService
{
    Task<ApiResponse<PagedResult<CandidateDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<CandidateDto>> GetByIdAsync(int id);
    Task<ApiResponse<CandidateDto>> CreateAsync(CreateCandidateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IInterviewService
{
    Task<ApiResponse<PagedResult<InterviewScheduleDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<InterviewScheduleDto>> GetByIdAsync(int id);
    Task<ApiResponse<InterviewScheduleDto>> CreateAsync(CreateInterviewScheduleDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IPerformanceService
{
    Task<ApiResponse<PagedResult<PerformanceReviewDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<PerformanceReviewDto>> GetByIdAsync(int id);
    Task<ApiResponse<PerformanceReviewDto>> CreateAsync(CreatePerformanceReviewDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IAssetService
{
    Task<ApiResponse<PagedResult<AssetDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<AssetDto>> GetByIdAsync(int id);
    Task<ApiResponse<AssetDto>> CreateAsync(CreateAssetDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IAssetAssignmentService
{
    Task<ApiResponse<PagedResult<AssetAssignmentDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<AssetAssignmentDto>> CreateAsync(CreateAssetAssignmentDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
