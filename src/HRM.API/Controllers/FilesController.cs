using HRM.Application.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public FilesController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("Vui lòng chọn file để tải lên."));

        var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads");
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        // Limit file type and size (e.g., 5MB)
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(ApiResponse<string>.Fail("Định dạng file không được hỗ trợ."));

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(ApiResponse<string>.Fail("Kích thước file vượt quá 5MB."));

        // Create unique filename
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"/uploads/{fileName}";
        return Ok(ApiResponse<string>.Ok(fileUrl, "Tải file thành công"));
    }

    [HttpGet("{fileName}")]
    [AllowAnonymous]
    public IActionResult GetFile(string fileName)
    {
        var filePath = Path.Combine(_env.ContentRootPath, "Uploads", fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found");

        var ext = Path.GetExtension(fileName).ToLower();
        string contentType = ext switch
        {
            ".pdf" => "application/pdf",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };

        return PhysicalFile(filePath, contentType);
    }
}
