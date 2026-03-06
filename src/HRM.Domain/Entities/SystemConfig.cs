using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class SystemConfig : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Group { get; set; } = "General"; // e.g., General, Leave, Payroll
}
