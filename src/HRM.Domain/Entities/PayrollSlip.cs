using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class PayrollSlip : BaseEntity
{
    public int EmployeeId { get; set; }
    public string MonthYear { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

    // Navigation properties
    public Employee Employee { get; set; } = null!;
}
