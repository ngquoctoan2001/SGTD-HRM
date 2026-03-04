using HRM.Domain.Entities;
using HRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Data;

/// <summary>
/// Seed data for development and demo purposes.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(HrmDbContext context)
    {
        if (await context.Users.AnyAsync())
            return; // Already seeded

        // Seed Admin user (password: Admin@123)
        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            Name = "Alex Morgan",
            Email = "admin@hrm.com",
            Phone = "0901234567",
            Bio = "HR Manager",
            AvatarUrl = ""
        };

        var normalUser = new User
        {
            Username = "user",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
            Role = UserRole.User,
            Name = "John Doe",
            Email = "user@hrm.com",
            Phone = "0909876543",
            Bio = "Employee",
            AvatarUrl = ""
        };

        context.Users.AddRange(adminUser, normalUser);

        // Seed Employees
        var employees = new List<Employee>
        {
            new() { Name = "Sarah Jenkins", Title = "Senior Developer", Department = "Engineering", Email = "sarah@company.com", Phone = "0901111111", Status = EmployeeStatus.Active, Avatar = "", JoinDate = new DateTime(2023, 3, 15) },
            new() { Name = "Michael Ross", Title = "Product Manager", Department = "Product", Email = "michael@company.com", Phone = "0902222222", Status = EmployeeStatus.Active, Avatar = "", JoinDate = new DateTime(2022, 7, 1) },
            new() { Name = "Elena Rodriguez", Title = "UI/UX Designer", Department = "Design", Email = "elena@company.com", Phone = "0903333333", Status = EmployeeStatus.Active, Avatar = "", JoinDate = new DateTime(2023, 1, 10) },
            new() { Name = "David Kim", Title = "DevOps Engineer", Department = "Engineering", Email = "david@company.com", Phone = "0904444444", Status = EmployeeStatus.Active, Avatar = "", JoinDate = new DateTime(2023, 6, 20) },
            new() { Name = "Lisa Chen", Title = "QA Engineer", Department = "Engineering", Email = "lisa@company.com", Phone = "0905555555", Status = EmployeeStatus.Active, Avatar = "", JoinDate = new DateTime(2023, 9, 5) },
        };
        context.Employees.AddRange(employees);
        await context.SaveChangesAsync();

        // Seed Job Postings
        var jobPostings = new List<JobPosting>
        {
            new() { Title = "Senior UI/UX Designer", Department = "Design", Location = "Remote", SalaryRange = "$80,000 - $120,000", Status = JobPostingStatus.Hiring, PostedDate = DateTime.UtcNow.AddDays(-10) },
            new() { Title = "Frontend Developer", Department = "Engineering", Location = "Hybrid", SalaryRange = "$70,000 - $100,000", Status = JobPostingStatus.Hiring, PostedDate = DateTime.UtcNow.AddDays(-5) },
            new() { Title = "DevOps Engineer", Department = "Engineering", Location = "On-site", SalaryRange = "$90,000 - $130,000", Status = JobPostingStatus.Hiring, PostedDate = DateTime.UtcNow.AddDays(-3) },
        };
        context.JobPostings.AddRange(jobPostings);
        await context.SaveChangesAsync();

        // Seed Candidates
        var candidates = new List<Candidate>
        {
            new() { Name = "John Doe", Email = "john.doe@email.com", Phone = "0911111111", JobPostingId = jobPostings[0].Id, Status = CandidateStatus.Interviewing },
            new() { Name = "Jane Smith", Email = "jane.smith@email.com", Phone = "0922222222", JobPostingId = jobPostings[1].Id, Status = CandidateStatus.Interviewing },
            new() { Name = "Robert Brown", Email = "robert.b@email.com", Phone = "0933333333", JobPostingId = jobPostings[2].Id, Status = CandidateStatus.New },
            new() { Name = "Alice Vance", Email = "alice.v@email.com", Phone = "0944444444", JobPostingId = jobPostings[0].Id, Status = CandidateStatus.New },
            new() { Name = "Tom Harris", Email = "tom.h@email.com", Phone = "0955555555", JobPostingId = jobPostings[1].Id, Status = CandidateStatus.New },
        };
        context.Candidates.AddRange(candidates);
        await context.SaveChangesAsync();

        // Seed Interview Schedules
        var interviews = new List<InterviewSchedule>
        {
            new() { CandidateId = candidates[0].Id, InterviewDate = DateTime.UtcNow.AddDays(2).Date.AddHours(10), InterviewType = InterviewType.Online, LocationOrLink = "https://meet.google.com/abc", InterviewerName = "HR Team", Notes = "", Status = InterviewStatus.Scheduled },
            new() { CandidateId = candidates[1].Id, InterviewDate = DateTime.UtcNow.AddDays(2).Date.AddHours(14), InterviewType = InterviewType.Online, LocationOrLink = "https://meet.google.com/def", InterviewerName = "Tech Lead", Notes = "", Status = InterviewStatus.Scheduled },
            new() { CandidateId = candidates[2].Id, InterviewDate = DateTime.UtcNow.AddDays(3).Date.AddHours(11), InterviewType = InterviewType.InPerson, LocationOrLink = "Office Room A", InterviewerName = "CTO", Notes = "", Status = InterviewStatus.Scheduled },
            new() { CandidateId = candidates[3].Id, InterviewDate = DateTime.UtcNow.AddDays(4).Date.AddHours(9), InterviewType = InterviewType.Online, LocationOrLink = "https://meet.google.com/ghi", InterviewerName = "Design Lead", Notes = "", Status = InterviewStatus.Scheduled },
            new() { CandidateId = candidates[4].Id, InterviewDate = DateTime.UtcNow.AddDays(5).Date.AddHours(16), InterviewType = InterviewType.InPerson, LocationOrLink = "Office Room B", InterviewerName = "Project Manager", Notes = "", Status = InterviewStatus.Scheduled },
        };
        context.InterviewSchedules.AddRange(interviews);

        // Seed Attendance Records (last 5 days)
        foreach (var emp in employees)
        {
            for (int i = 1; i <= 5; i++)
            {
                context.AttendanceRecords.Add(new AttendanceRecord
                {
                    EmployeeId = emp.Id,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-i)),
                    CheckInTime = new TimeOnly(8, 0 + new Random().Next(0, 30)),
                    CheckOutTime = new TimeOnly(17, 0 + new Random().Next(0, 30)),
                    Status = i == 3 && emp.Id == employees[2].Id ? AttendanceStatus.Late : AttendanceStatus.Present,
                    Notes = ""
                });
            }
        }

        // Seed Leave Requests
        var leaveRequests = new List<LeaveRequest>
        {
            new() { EmployeeId = employees[0].Id, Type = LeaveType.Annual, StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)), EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(9)), Reason = "Family vacation", Status = LeaveStatus.Pending },
            new() { EmployeeId = employees[1].Id, Type = LeaveType.Sick, StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), Reason = "Flu", Status = LeaveStatus.Approved },
            new() { EmployeeId = employees[2].Id, Type = LeaveType.Annual, StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)), EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)), Reason = "Personal matters", Status = LeaveStatus.Rejected },
            new() { EmployeeId = employees[3].Id, Type = LeaveType.Annual, StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)), EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(34)), Reason = "Travel abroad", Status = LeaveStatus.Pending },
        };
        context.LeaveRequests.AddRange(leaveRequests);

        // Seed Payroll Slips
        foreach (var emp in employees)
        {
            context.PayrollSlips.Add(new PayrollSlip
            {
                EmployeeId = emp.Id,
                MonthYear = DateTime.UtcNow.ToString("MM/yyyy"),
                BasicSalary = 5000 + new Random().Next(0, 3000),
                TotalAllowances = 500,
                TotalDeductions = 200,
                NetSalary = 5300,
                Status = PayrollStatus.Draft
            });
        }

        // Seed Assets
        var assets = new List<Asset>
        {
            new() { AssetTag = "LPT-001", Name = "MacBook Pro 14", Category = AssetCategory.Electronics, Status = AssetStatus.Assigned, PurchaseDate = new DateOnly(2024, 1, 15) },
            new() { AssetTag = "LPT-002", Name = "Dell XPS 15", Category = AssetCategory.Electronics, Status = AssetStatus.Available, PurchaseDate = new DateOnly(2024, 3, 10) },
            new() { AssetTag = "DSK-001", Name = "Standing Desk", Category = AssetCategory.Furniture, Status = AssetStatus.Assigned, PurchaseDate = new DateOnly(2024, 2, 20) },
        };
        context.Assets.AddRange(assets);
        await context.SaveChangesAsync();

        // Seed Asset Assignments
        context.AssetAssignments.Add(new AssetAssignment
        {
            AssetId = assets[0].Id,
            EmployeeId = employees[0].Id,
            AssignedDate = new DateOnly(2024, 1, 20),
            Notes = "Assigned for development"
        });
        context.AssetAssignments.Add(new AssetAssignment
        {
            AssetId = assets[2].Id,
            EmployeeId = employees[1].Id,
            AssignedDate = new DateOnly(2024, 2, 25),
            Notes = "Office furniture"
        });

        await context.SaveChangesAsync();
    }
}
