using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // e.g., Leave, System, Payroll
    
    public bool IsRead { get; set; }
    public string Link { get; set; } = string.Empty;
}
