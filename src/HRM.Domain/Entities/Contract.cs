namespace HRM.Domain.Entities;

public class Contract : BaseEntity
{
    public int EmployeeId { get; set; }
    public string ContractType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal BasicSalary { get; set; }
    public string Status { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;

    public Employee? Employee { get; set; }
}
