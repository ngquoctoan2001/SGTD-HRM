namespace HRM.Application.DTOs;

public class DisciplineRewardDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime DecisionDate { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class CreateDisciplineRewardDto
{
    public int EmployeeId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime DecisionDate { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}
