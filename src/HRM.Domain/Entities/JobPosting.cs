using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class JobPosting : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string SalaryRange { get; set; } = string.Empty;
    public JobPostingStatus Status { get; set; } = JobPostingStatus.Hiring;
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
}
