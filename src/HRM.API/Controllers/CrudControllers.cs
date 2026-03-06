using HRM.Application.DTOs;
using HRM.Application.DTOs.Common;
using HRM.Application.DTOs.Employee;
using HRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;
    public EmployeesController(IEmployeeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p) => Ok(await _service.GetAllAsync(p));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpGet("{id}/profile")]
    public async Task<IActionResult> GetProfile(int id)
    {
        var r = await _service.GetProfileAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var r = await _service.UpdateAsync(id, dto);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;
    public AttendanceController(IAttendanceService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p)
    {
        if (User.IsInRole("Employee"))
        {
            p.EmployeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        return Ok(await _service.GetAllAsync(p));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttendanceDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _service;
    public LeaveController(ILeaveService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p)
    {
        if (User.IsInRole("Employee"))
        {
            p.EmployeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        return Ok(await _service.GetAllAsync(p));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateLeaveStatusDto dto)
    {
        var r = await _service.UpdateStatusAsync(id, dto);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _service;
    public PayrollController(IPayrollService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p)
    {
        if (User.IsInRole("Employee"))
        {
            p.EmployeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        return Ok(await _service.GetAllAsync(p));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePayrollSlipDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobPostingsController : ControllerBase
{
    private readonly IJobPostingService _service;
    public JobPostingsController(IJobPostingService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p) => Ok(await _service.GetAllAsync(p));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateJobPostingDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _service;
    public CandidatesController(ICandidateService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p) => Ok(await _service.GetAllAsync(p));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCandidateDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateCandidateStatusDto dto)
    {
        var r = await _service.UpdateStatusAsync(id, dto);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _service;
    public InterviewsController(IInterviewService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p) => Ok(await _service.GetAllAsync(p));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateInterviewScheduleDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerformanceController : ControllerBase
{
    private readonly IPerformanceService _service;
    public PerformanceController(IPerformanceService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p)
    {
        if (User.IsInRole("Employee"))
        {
            p.EmployeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        return Ok(await _service.GetAllAsync(p));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePerformanceReviewDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _service;
    private readonly IAssetAssignmentService _assignService;

    public AssetsController(IAssetService service, IAssetAssignmentService assignService)
    { _service = service; _assignService = assignService; }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p) => Ok(await _service.GetAllAsync(p));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAssetDto dto) => Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(r) : NotFound(r);
    }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetAssignments([FromQuery] QueryParameters p) => Ok(await _assignService.GetAllAsync(p));

    [HttpPost("assignments")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssetAssignmentDto dto) => Ok(await _assignService.CreateAsync(dto));
}
