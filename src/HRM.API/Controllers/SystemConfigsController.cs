using HRM.Domain.Entities;
using HRM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires login
public class SystemConfigsController : ControllerBase
{
    private readonly HrmDbContext _context;

    public SystemConfigsController(HrmDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Allowed for all logged in users as they might need config info,
        // or we can restrict it to Admin later if needed. For now allow read.
        var configs = await _context.SystemConfigs.ToListAsync();
        return Ok(new { success = true, data = new { items = configs } });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Create(SystemConfig request)
    {
        var exists = await _context.SystemConfigs.AnyAsync(x => x.Key == request.Key);
        if (exists) return BadRequest(new { success = false, message = "Key already exists" });

        _context.SystemConfigs.Add(request);
        await _context.SaveChangesAsync();
        return Ok(new { success = true, data = request });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Update(int id, SystemConfig request)
    {
        var config = await _context.SystemConfigs.FindAsync(id);
        if (config == null) return NotFound(new { success = false, message = "Config not found" });

        config.Value = request.Value;
        config.Description = request.Description;
        config.Group = request.Group;

        // Note: Key ideally shouldn't be changed, but if we allow it:
        if (config.Key != request.Key)
        {
            var exists = await _context.SystemConfigs.AnyAsync(x => x.Key == request.Key && x.Id != id);
            if (exists) return BadRequest(new { success = false, message = "Key already exists" });
            config.Key = request.Key;
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true, data = config });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Delete(int id)
    {
        var config = await _context.SystemConfigs.FindAsync(id);
        if (config == null) return NotFound(new { success = false, message = "Config not found" });

        // Soft delete since we have IsDeleted
        config.IsDeleted = true;
        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "Deleted successfully" });
    }
}
