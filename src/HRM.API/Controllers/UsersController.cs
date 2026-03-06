using System.Security.Claims;
using HRM.Application.DTOs;
using HRM.Application.DTOs.Common;
using HRM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters p)
    {
        return Ok(await _userService.GetAllAsync(p));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var res = await _userService.GetProfileAsync(userId);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var res = await _userService.UpdateProfileAsync(userId, dto);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpPut("me/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var res = await _userService.ChangePasswordAsync(userId, dto);
        return res.Success ? Ok(res) : BadRequest(res);
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateUserRoleDto dto)
    {
        var res = await _userService.UpdateRoleAsync(id, dto);
        return res.Success ? Ok(res) : NotFound(res);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _userService.DeleteAsync(id);
        return res.Success ? Ok(res) : NotFound(res);
    }
}
