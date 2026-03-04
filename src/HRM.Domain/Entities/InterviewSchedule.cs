using HRM.Domain.Enums;

namespace HRM.Domain.Entities;

public class InterviewSchedule : BaseEntity
{
    public int CandidateId { get; set; }
    public DateTime InterviewDate { get; set; }
    public InterviewType InterviewType { get; set; } = InterviewType.Online;
    public string LocationOrLink { get; set; } = string.Empty;
    public string InterviewerName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
}
