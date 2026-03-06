using HRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,HR")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;

    public ReportsController(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    [HttpGet("payroll/export")]
    public async Task<IActionResult> ExportPayroll([FromQuery] int month, [FromQuery] int year)
    {
        if (month < 1 || month > 12 || year < 2000)
            return BadRequest(new { message = "Invalid month or year" });

        var fileContent = await _reportsService.ExportPayrollToCsvAsync(month, year);
        return File(fileContent, "text/csv", $"Payroll_{year}_{month:D2}.csv");
    }
}
