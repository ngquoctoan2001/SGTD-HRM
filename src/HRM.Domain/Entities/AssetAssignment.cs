namespace HRM.Domain.Entities;

public class AssetAssignment : BaseEntity
{
    public int AssetId { get; set; }
    public int EmployeeId { get; set; }
    public DateOnly AssignedDate { get; set; }
    public DateOnly? ReturnedDate { get; set; }
    public string Notes { get; set; } = string.Empty;

    // Navigation properties
    public Asset Asset { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
