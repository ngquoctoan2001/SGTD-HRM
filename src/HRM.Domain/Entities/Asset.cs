using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class Asset : BaseEntity
{
    public string AssetTag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AssetCategory Category { get; set; } = AssetCategory.Electronics;
    public AssetStatus Status { get; set; } = AssetStatus.Available;
    public DateOnly PurchaseDate { get; set; }

    // Navigation properties
    public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();
}
