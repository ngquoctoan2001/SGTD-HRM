using HRM.Application.DTOs.Common;
using HRM.Application.DTOs.Dashboard;
using HRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    /// <summary>
    /// Get dashboard overview with KPI data, charts, and recent activity.
    /// </summary>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<DashboardOverviewDto>), 200)]
    public async Task<IActionResult> GetOverview()
    {
        var result = await _service.GetOverviewAsync();
        return Ok(result);
    }
}
