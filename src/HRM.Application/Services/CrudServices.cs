using AutoMapper;
using HRM.Application.DTOs;
using HRM.Application.DTOs.Common;
using HRM.Application.DTOs.Employee;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using HRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRM.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<Employee> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public EmployeeService(IRepository<Employee> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<EmployeeDto>>> GetAllAsync(QueryParameters p)
    {
        EmployeeStatus? statusFilter = null;
        if (!string.IsNullOrEmpty(p.Status) && Enum.TryParse<EmployeeStatus>(p.Status, out var parsedStatus))
            statusFilter = parsedStatus;

        var (items, total) = await _repo.GetPagedAsync(
            p.PageNumber, p.PageSize,
            filter: e => (string.IsNullOrEmpty(p.Search) || e.Name.Contains(p.Search) || e.Department.Name.Contains(p.Search)) &&
                         (!p.DepartmentId.HasValue || e.DepartmentId == p.DepartmentId.Value) &&
                         (!statusFilter.HasValue || e.Status == statusFilter.Value),
            null,
            e => e.Department);

        var dtos = _mapper.Map<IEnumerable<EmployeeDto>>(items);
        var result = new PagedResult<EmployeeDto> { Items = dtos, TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize };
        return ApiResponse<PagedResult<EmployeeDto>>.Ok(result);
    }

    public async Task<ApiResponse<EmployeeDto>> GetByIdAsync(int id)
    {
        var entities = await _repo.FindAsync(e => e.Id == id, e => e.Department);
        var entity = entities.FirstOrDefault();
        if (entity == null) return ApiResponse<EmployeeDto>.Fail("Employee not found");
        return ApiResponse<EmployeeDto>.Ok(_mapper.Map<EmployeeDto>(entity));
    }

    public async Task<ApiResponse<EmployeeProfileDto>> GetProfileAsync(int id)
    {
        var entities = await _repo.FindAsync(e => e.Id == id,
            e => e.Department,
            e => e.Contracts,
            e => e.LeaveRequests,
            e => e.PerformanceReviews,
            e => e.AttendanceRecords);
        var entity = entities.FirstOrDefault();
        if (entity == null) return ApiResponse<EmployeeProfileDto>.Fail("Employee not found");
        
        // Also fetch DisciplineRewards since they are not directly on Employee, wait they might not be part of the Employee navigation.
        // Let's assume Employee has DisciplineRewards as navigation or we can fetch them separately if needed.
        // Actually, looking at Employee.cs, DisciplineRewards is not a collection. 
        // Let's just map what we have from the navigation entities.
        
        return ApiResponse<EmployeeProfileDto>.Ok(_mapper.Map<EmployeeProfileDto>(entity));
    }

    public async Task<ApiResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto dto)
    {
        var entity = _mapper.Map<Employee>(dto);
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<EmployeeDto>.Ok(_mapper.Map<EmployeeDto>(entity), "Employee created");
    }

    public async Task<ApiResponse<EmployeeDto>> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return ApiResponse<EmployeeDto>.Fail("Employee not found");

        entity.Name = dto.Name;
        entity.Title = dto.Title;
        entity.DepartmentId = dto.DepartmentId;
        entity.Email = dto.Email;
        entity.Phone = dto.Phone;
        entity.Avatar = dto.Avatar;
        if (Enum.TryParse<EmployeeStatus>(dto.Status, out var status))
            entity.Status = status;

        _repo.Update(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<EmployeeDto>.Ok(_mapper.Map<EmployeeDto>(entity), "Employee updated");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return ApiResponse<bool>.Fail("Employee not found");
        _repo.SoftDelete(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Employee deleted");
    }
}

// ==================== ATTENDANCE SERVICE ====================
public class AttendanceService : IAttendanceService
{
    private readonly IRepository<AttendanceRecord> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AttendanceService(IRepository<AttendanceRecord> repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<AttendanceRecordDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            filter: a => !p.EmployeeId.HasValue || a.EmployeeId == p.EmployeeId.Value, 
            null, a => a.Employee);
        var dtos = _mapper.Map<IEnumerable<AttendanceRecordDto>>(items);
        return ApiResponse<PagedResult<AttendanceRecordDto>>.Ok(new PagedResult<AttendanceRecordDto> { Items = dtos, TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<AttendanceRecordDto>> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? ApiResponse<AttendanceRecordDto>.Fail("Not found") : ApiResponse<AttendanceRecordDto>.Ok(_mapper.Map<AttendanceRecordDto>(e));
    }

    public async Task<ApiResponse<AttendanceRecordDto>> CreateAsync(CreateAttendanceDto dto)
    {
        var entity = new AttendanceRecord
        {
            EmployeeId = dto.EmployeeId,
            Date = DateOnly.Parse(dto.Date),
            CheckInTime = string.IsNullOrEmpty(dto.CheckInTime) ? null : TimeOnly.Parse(dto.CheckInTime),
            CheckOutTime = string.IsNullOrEmpty(dto.CheckOutTime) ? null : TimeOnly.Parse(dto.CheckOutTime),
            Status = Enum.TryParse<AttendanceStatus>(dto.Status, out var s) ? s : AttendanceStatus.Present,
            Notes = dto.Notes
        };
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<AttendanceRecordDto>.Ok(_mapper.Map<AttendanceRecordDto>(entity), "Created");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== LEAVE SERVICE ====================
public class LeaveService : ILeaveService
{
    private readonly IRepository<LeaveRequest> _repo;
    private readonly IRepository<User> _userRepo;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public LeaveService(IRepository<LeaveRequest> repo, IRepository<User> userRepo, INotificationService notificationService, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _userRepo = userRepo; _notificationService = notificationService; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<LeaveRequestDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            filter: l => !p.EmployeeId.HasValue || l.EmployeeId == p.EmployeeId.Value, 
            null, l => l.Employee);
        return ApiResponse<PagedResult<LeaveRequestDto>>.Ok(new PagedResult<LeaveRequestDto> { Items = _mapper.Map<IEnumerable<LeaveRequestDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<LeaveRequestDto>> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? ApiResponse<LeaveRequestDto>.Fail("Not found") : ApiResponse<LeaveRequestDto>.Ok(_mapper.Map<LeaveRequestDto>(e));
    }

    public async Task<ApiResponse<LeaveRequestDto>> CreateAsync(CreateLeaveRequestDto dto)
    {
        var entity = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            Type = Enum.TryParse<LeaveType>(dto.Type, out var t) ? t : LeaveType.Annual,
            StartDate = DateOnly.Parse(dto.StartDate),
            EndDate = DateOnly.Parse(dto.EndDate),
            Reason = dto.Reason,
            Status = LeaveStatus.Pending
        };
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<LeaveRequestDto>.Ok(_mapper.Map<LeaveRequestDto>(entity), "Created");
    }

    public async Task<ApiResponse<LeaveRequestDto>> UpdateStatusAsync(int id, UpdateLeaveStatusDto dto)
    {
        var e = (await _repo.FindAsync(l => l.Id == id, l => l.Employee)).FirstOrDefault();
        if (e == null) return ApiResponse<LeaveRequestDto>.Fail("Not found");
        
        var oldStatus = e.Status;
        if (Enum.TryParse<LeaveStatus>(dto.Status, out var s)) e.Status = s;
        _repo.Update(e);
        await _uow.SaveChangesAsync();

        if (oldStatus != e.Status && e.Employee != null)
        {
            var user = (await _userRepo.FindAsync(u => u.Email == e.Employee.Email)).FirstOrDefault();
            if (user != null)
            {
                var msg = e.Status == LeaveStatus.Approved ? "Đơn xin nghỉ phép của bạn đã được duyệt." : "Đơn xin nghỉ phép của bạn đã bị từ chối.";
                await _notificationService.SendNotificationAsync(user.Id, "Cập nhật Nghỉ phép", msg, "Leave", "/leave");
            }
        }

        return ApiResponse<LeaveRequestDto>.Ok(_mapper.Map<LeaveRequestDto>(e), "Status updated");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== PAYROLL SERVICE ====================
public class PayrollService : IPayrollService
{
    private readonly IRepository<PayrollSlip> _repo;
    private readonly IRepository<Employee> _employeeRepo;
    private readonly IRepository<User> _userRepo;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PayrollService(IRepository<PayrollSlip> repo, IRepository<Employee> employeeRepo, IRepository<User> userRepo, INotificationService notificationService, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _employeeRepo = employeeRepo; _userRepo = userRepo; _notificationService = notificationService; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<PayrollSlipDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            filter: p2 => !p.EmployeeId.HasValue || p2.EmployeeId == p.EmployeeId.Value, 
            null, p2 => p2.Employee);
        return ApiResponse<PagedResult<PayrollSlipDto>>.Ok(new PagedResult<PayrollSlipDto> { Items = _mapper.Map<IEnumerable<PayrollSlipDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<PayrollSlipDto>> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? ApiResponse<PayrollSlipDto>.Fail("Not found") : ApiResponse<PayrollSlipDto>.Ok(_mapper.Map<PayrollSlipDto>(e));
    }

    public async Task<ApiResponse<PayrollSlipDto>> CreateAsync(CreatePayrollSlipDto dto)
    {
        var entity = new PayrollSlip
        {
            EmployeeId = dto.EmployeeId,
            MonthYear = dto.MonthYear,
            BasicSalary = dto.BasicSalary,
            TotalAllowances = dto.TotalAllowances,
            TotalDeductions = dto.TotalDeductions,
            NetSalary = dto.BasicSalary + dto.TotalAllowances - dto.TotalDeductions
        };
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var employee = await _employeeRepo.GetByIdAsync(dto.EmployeeId);
        if (employee != null)
        {
            var user = (await _userRepo.FindAsync(u => u.Email == employee.Email)).FirstOrDefault();
            if (user != null)
            {
                await _notificationService.SendNotificationAsync(user.Id, "Phiếu lương mới", $"Phiếu lương kỳ {dto.MonthYear} của bạn đã được tạo.", "Payroll", "/payroll");
            }
        }

        return ApiResponse<PayrollSlipDto>.Ok(_mapper.Map<PayrollSlipDto>(entity), "Created");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== JOB POSTING SERVICE ====================
public class JobPostingService : IJobPostingService
{
    private readonly IRepository<JobPosting> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public JobPostingService(IRepository<JobPosting> repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<JobPostingDto>>> GetAllAsync(QueryParameters p)
    {
        JobPostingStatus? statusFilter = null;
        if (!string.IsNullOrEmpty(p.Status) && Enum.TryParse<JobPostingStatus>(p.Status, out var parsedStatus))
            statusFilter = parsedStatus;

        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            filter: j => (string.IsNullOrEmpty(p.Search) || j.Title.Contains(p.Search) || j.Department.Name.Contains(p.Search)) &&
                         (!p.DepartmentId.HasValue || j.DepartmentId == p.DepartmentId.Value) &&
                         (!statusFilter.HasValue || j.Status == statusFilter.Value),
            null, 
            j => j.Candidates, j => j.Department);
        return ApiResponse<PagedResult<JobPostingDto>>.Ok(new PagedResult<JobPostingDto> { Items = _mapper.Map<IEnumerable<JobPostingDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<JobPostingDto>> GetByIdAsync(int id)
    {
        var entities = await _repo.FindAsync(j => j.Id == id, j => j.Department);
        var e = entities.FirstOrDefault();
        return e == null ? ApiResponse<JobPostingDto>.Fail("Not found") : ApiResponse<JobPostingDto>.Ok(_mapper.Map<JobPostingDto>(e));
    }

    public async Task<ApiResponse<JobPostingDto>> CreateAsync(CreateJobPostingDto dto)
    {
        var entity = _mapper.Map<JobPosting>(dto);
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<JobPostingDto>.Ok(_mapper.Map<JobPostingDto>(entity), "Created");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== DEPARTMENT SERVICE ====================
public class DepartmentService : IDepartmentService
{
    private readonly IRepository<Department> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public DepartmentService(IRepository<Department> repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<DepartmentDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            filter: string.IsNullOrEmpty(p.Search) ? null : d => d.Name.Contains(p.Search));
        return ApiResponse<PagedResult<DepartmentDto>>.Ok(new PagedResult<DepartmentDto> { Items = _mapper.Map<IEnumerable<DepartmentDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<DepartmentDto>> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? ApiResponse<DepartmentDto>.Fail("Not found") : ApiResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(e));
    }

    public async Task<ApiResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
    {
        var entity = _mapper.Map<Department>(dto);
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(entity), "Created");
    }

    public async Task<ApiResponse<DepartmentDto>> UpdateAsync(int id, CreateDepartmentDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return ApiResponse<DepartmentDto>.Fail("Not found");
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        _repo.Update(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(entity), "Updated");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== CANDIDATE SERVICE ====================
public class CandidateService : ICandidateService
{
    private readonly IRepository<Candidate> _repo;
    private readonly IRepository<JobPosting> _jobRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CandidateService(IRepository<Candidate> repo, IRepository<JobPosting> jobRepo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _jobRepo = jobRepo; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<CandidateDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            null, null, c => c.JobPosting);
        return ApiResponse<PagedResult<CandidateDto>>.Ok(new PagedResult<CandidateDto> { Items = _mapper.Map<IEnumerable<CandidateDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<CandidateDto>> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? ApiResponse<CandidateDto>.Fail("Not found") : ApiResponse<CandidateDto>.Ok(_mapper.Map<CandidateDto>(e));
    }

    public async Task<ApiResponse<CandidateDto>> CreateAsync(CreateCandidateDto dto)
    {
        var entity = _mapper.Map<Candidate>(dto);
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        
        return ApiResponse<CandidateDto>.Ok(_mapper.Map<CandidateDto>(entity), "Created");
    }

    public async Task<ApiResponse<CandidateDto>> UpdateStatusAsync(int id, UpdateCandidateStatusDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return ApiResponse<CandidateDto>.Fail("Candidate not found");

        if (Enum.TryParse<CandidateStatus>(dto.Status, true, out var status))
        {
            entity.Status = status;
            _repo.Update(entity);
            await _uow.SaveChangesAsync();
            return ApiResponse<CandidateDto>.Ok(_mapper.Map<CandidateDto>(entity), "Status updated");
        }
        
        return ApiResponse<CandidateDto>.Fail("Invalid status value");
    }
    
    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== INTERVIEW SERVICE ====================
public class InterviewService : IInterviewService
{
    private readonly IRepository<InterviewSchedule> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public InterviewService(IRepository<InterviewSchedule> repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<InterviewScheduleDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            null, null, i => i.Candidate);
        return ApiResponse<PagedResult<InterviewScheduleDto>>.Ok(new PagedResult<InterviewScheduleDto> { Items = _mapper.Map<IEnumerable<InterviewScheduleDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<InterviewScheduleDto>> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? ApiResponse<InterviewScheduleDto>.Fail("Not found") : ApiResponse<InterviewScheduleDto>.Ok(_mapper.Map<InterviewScheduleDto>(e));
    }

    public async Task<ApiResponse<InterviewScheduleDto>> CreateAsync(CreateInterviewScheduleDto dto)
    {
        var entity = new InterviewSchedule
        {
            CandidateId = dto.CandidateId,
            InterviewDate = dto.InterviewDate,
            InterviewType = Enum.TryParse<InterviewType>(dto.InterviewType, out var t) ? t : InterviewType.Online,
            LocationOrLink = dto.LocationOrLink,
            InterviewerName = dto.InterviewerName,
            Notes = dto.Notes
        };
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<InterviewScheduleDto>.Ok(_mapper.Map<InterviewScheduleDto>(entity), "Created");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== PERFORMANCE SERVICE ====================
public class PerformanceService : IPerformanceService
{
    private readonly IRepository<PerformanceReview> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public PerformanceService(IRepository<PerformanceReview> repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<PerformanceReviewDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            filter: pr => !p.EmployeeId.HasValue || pr.EmployeeId == p.EmployeeId.Value, 
            null, pr => pr.Employee);
        return ApiResponse<PagedResult<PerformanceReviewDto>>.Ok(new PagedResult<PerformanceReviewDto> { Items = _mapper.Map<IEnumerable<PerformanceReviewDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<PerformanceReviewDto>> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? ApiResponse<PerformanceReviewDto>.Fail("Not found") : ApiResponse<PerformanceReviewDto>.Ok(_mapper.Map<PerformanceReviewDto>(e));
    }

    public async Task<ApiResponse<PerformanceReviewDto>> CreateAsync(CreatePerformanceReviewDto dto)
    {
        var entity = new PerformanceReview
        {
            EmployeeId = dto.EmployeeId,
            ReviewerId = dto.ReviewerId,
            ReviewerName = dto.ReviewerName,
            ReviewDate = DateOnly.Parse(dto.ReviewDate),
            ReviewPeriod = dto.ReviewPeriod,
            Rating = dto.Rating,
            Goals = dto.Goals,
            Feedback = dto.Feedback
        };
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<PerformanceReviewDto>.Ok(_mapper.Map<PerformanceReviewDto>(entity), "Created");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== ASSET SERVICE ====================
public class AssetService : IAssetService
{
    private readonly IRepository<Asset> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AssetService(IRepository<Asset> repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<AssetDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize);
        return ApiResponse<PagedResult<AssetDto>>.Ok(new PagedResult<AssetDto> { Items = _mapper.Map<IEnumerable<AssetDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<AssetDto>> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? ApiResponse<AssetDto>.Fail("Not found") : ApiResponse<AssetDto>.Ok(_mapper.Map<AssetDto>(e));
    }

    public async Task<ApiResponse<AssetDto>> CreateAsync(CreateAssetDto dto)
    {
        var entity = _mapper.Map<Asset>(dto);
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<AssetDto>.Ok(_mapper.Map<AssetDto>(entity), "Created");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}

// ==================== ASSET ASSIGNMENT SERVICE ====================
public class AssetAssignmentService : IAssetAssignmentService
{
    private readonly IRepository<AssetAssignment> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AssetAssignmentService(IRepository<AssetAssignment> repo, IUnitOfWork uow, IMapper mapper)
    { _repo = repo; _uow = uow; _mapper = mapper; }

    public async Task<ApiResponse<PagedResult<AssetAssignmentDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize);
        return ApiResponse<PagedResult<AssetAssignmentDto>>.Ok(new PagedResult<AssetAssignmentDto> { Items = _mapper.Map<IEnumerable<AssetAssignmentDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize });
    }

    public async Task<ApiResponse<AssetAssignmentDto>> CreateAsync(CreateAssetAssignmentDto dto)
    {
        var entity = new AssetAssignment
        {
            AssetId = dto.AssetId,
            EmployeeId = dto.EmployeeId,
            AssignedDate = DateOnly.Parse(dto.AssignedDate),
            Notes = dto.Notes
        };
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<AssetAssignmentDto>.Ok(_mapper.Map<AssetAssignmentDto>(entity), "Created");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.Fail("Not found");
        _repo.SoftDelete(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Deleted");
    }
}
