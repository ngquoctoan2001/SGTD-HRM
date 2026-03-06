namespace HRM.Application.DTOs;

public class TrainingCourseDto
{
    public int Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateTrainingCourseDto
{
    public string CourseName { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Upcoming";
}
