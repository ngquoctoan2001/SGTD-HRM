using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class Candidate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int JobPostingId { get; set; }
    public CandidateStatus Status { get; set; } = CandidateStatus.New;
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public JobPosting JobPosting { get; set; } = null!;
    public ICollection<InterviewSchedule> InterviewSchedules { get; set; } = new List<InterviewSchedule>();
}
