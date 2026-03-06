namespace HRM.Domain.Entities;

public class DisciplineReward : BaseEntity
{
    public int EmployeeId { get; set; }
    public string Type { get; set; } = string.Empty; // "Reward" or "Discipline"
    public DateTime DecisionDate { get; set; }
    public decimal Amount { get; set; } // Bonus amount or penalty deduction
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Approved", "Pending"

    public Employee? Employee { get; set; }
}
