namespace HRM.Domain.Entities;

/// <summary>
/// Base entity with common properties for all domain entities.
/// Supports soft delete pattern.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
