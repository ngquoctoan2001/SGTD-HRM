using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class AttendanceRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string Notes { get; set; } = string.Empty;

    // Navigation properties
    public Employee Employee { get; set; } = null!;
}
