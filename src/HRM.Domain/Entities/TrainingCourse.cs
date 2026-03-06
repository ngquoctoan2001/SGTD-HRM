namespace HRM.Domain.Entities;

public class TrainingCourse : BaseEntity
{
    public string CourseName { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty; // "Upcoming", "In Progress", "Completed"

    public ICollection<TrainingParticipant> Participants { get; set; } = new List<TrainingParticipant>();
}

public class TrainingParticipant : BaseEntity
{
    public int TrainingCourseId { get; set; }
    public int EmployeeId { get; set; }
    public string Status { get; set; } = "Enrolled"; // "Enrolled", "Completed", "Failed"

    public TrainingCourse? TrainingCourse { get; set; }
    public Employee? Employee { get; set; }
}
