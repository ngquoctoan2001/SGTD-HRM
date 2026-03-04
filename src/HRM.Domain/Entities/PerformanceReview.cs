using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class PerformanceReview : BaseEntity
{
    public int EmployeeId { get; set; }
    public int ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public DateOnly ReviewDate { get; set; }
    public string ReviewPeriod { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Goals { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public ReviewStatus Status { get; set; } = ReviewStatus.Draft;

    // Navigation properties
    public Employee Employee { get; set; } = null!;
}
