using HRM.Domain.Entities;
using HRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRM.Infrastructure.Data;

public class HrmDbContext : DbContext
{
    public HrmDbContext(DbContextOptions<HrmDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<DisciplineReward> DisciplineRewards => Set<DisciplineReward>();
    public DbSet<TrainingCourse> TrainingCourses => Set<TrainingCourse>();
    public DbSet<TrainingParticipant> TrainingParticipants => Set<TrainingParticipant>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<InterviewSchedule> InterviewSchedules => Set<InterviewSchedule>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<PayrollSlip> PayrollSlips => Set<PayrollSlip>();
    public DbSet<PerformanceReview> PerformanceReviews => Set<PerformanceReview>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Department>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Contract>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Contract>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<JobPosting>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<DisciplineReward>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TrainingCourse>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TrainingParticipant>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Candidate>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<InterviewSchedule>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<AttendanceRecord>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<LeaveRequest>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PayrollSlip>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PerformanceReview>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Asset>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<AssetAssignment>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SystemConfig>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);

        // ==================== USER ====================
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
        });

        // ==================== DEPARTMENT ====================
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        // ==================== EMPLOYEE ====================
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Avatar).HasMaxLength(500);

            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Employees)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==================== CONTRACT ====================
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId);
            entity.Property(e => e.ContractType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.BasicSalary).HasPrecision(18, 2);
            entity.Property(e => e.FileUrl).HasMaxLength(500);

            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.Contracts)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== DISCIPLINE & REWARD ====================
        modelBuilder.Entity<DisciplineReward>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId);
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

            entity.HasOne(e => e.Employee)
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== TRAINING COURSE ====================
        modelBuilder.Entity<TrainingCourse>(entity =>
        {
            entity.Property(e => e.CourseName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Instructor).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<TrainingParticipant>(entity =>
        {
            entity.HasIndex(e => e.TrainingCourseId);
            entity.HasIndex(e => e.EmployeeId);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(e => e.TrainingCourse)
                  .WithMany(tc => tc.Participants)
                  .HasForeignKey(e => e.TrainingCourseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Employee)
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== JOB POSTING ====================
        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.SalaryRange).HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(e => e.Department)
                  .WithMany(d => d.JobPostings)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==================== CANDIDATE ====================
        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasIndex(e => e.JobPostingId);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(e => e.JobPosting)
                  .WithMany(j => j.Candidates)
                  .HasForeignKey(e => e.JobPostingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== INTERVIEW SCHEDULE ====================
        modelBuilder.Entity<InterviewSchedule>(entity =>
        {
            entity.HasIndex(e => e.CandidateId);
            entity.Property(e => e.InterviewType).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.LocationOrLink).HasMaxLength(500);
            entity.Property(e => e.InterviewerName).HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(e => e.Candidate)
                  .WithMany(c => c.InterviewSchedules)
                  .HasForeignKey(e => e.CandidateId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== ATTENDANCE RECORD ====================
        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId);
            entity.HasIndex(e => e.Date);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.AttendanceRecords)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== LEAVE REQUEST ====================
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId);
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(50);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.LeaveRequests)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== PAYROLL SLIP ====================
        modelBuilder.Entity<PayrollSlip>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId);
            entity.Property(e => e.MonthYear).HasMaxLength(20).IsRequired();
            entity.Property(e => e.BasicSalary).HasPrecision(18, 2);
            entity.Property(e => e.TotalAllowances).HasPrecision(18, 2);
            entity.Property(e => e.TotalDeductions).HasPrecision(18, 2);
            entity.Property(e => e.NetSalary).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.PayrollSlips)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== PERFORMANCE REVIEW ====================
        modelBuilder.Entity<PerformanceReview>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId);
            entity.Property(e => e.ReviewerName).HasMaxLength(200);
            entity.Property(e => e.ReviewPeriod).HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.PerformanceReviews)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== ASSET ====================
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasIndex(e => e.AssetTag).IsUnique();
            entity.Property(e => e.AssetTag).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasConversion<string>().HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(100);
        });

        // ==================== ASSET ASSIGNMENT ====================
        modelBuilder.Entity<AssetAssignment>(entity =>
        {
            entity.HasIndex(e => e.AssetId);
            entity.HasIndex(e => e.EmployeeId);

            entity.HasOne(e => e.Asset)
                  .WithMany(a => a.AssetAssignments)
                  .HasForeignKey(e => e.AssetId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.AssetAssignments)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ==================== SYSTEM CONFIG ====================
        modelBuilder.Entity<SystemConfig>(entity =>
        {
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Key).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Group).HasMaxLength(100);
        });

        // ==================== NOTIFICATIONS ====================
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Message).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Link).HasMaxLength(500);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-update timestamps
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
