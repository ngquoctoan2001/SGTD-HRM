using AutoMapper;
using HRM.Application.DTOs;
using HRM.Application.DTOs.Auth;
using HRM.Application.DTOs.Dashboard;
using HRM.Application.DTOs.Employee;
using HRM.Domain.Entities;

namespace HRM.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserInfoDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        // Employee
        CreateMap<Domain.Entities.Employee, EmployeeDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
        CreateMap<CreateEmployeeDto, Domain.Entities.Employee>();
        CreateMap<UpdateEmployeeDto, Domain.Entities.Employee>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Attendance
        CreateMap<AttendanceRecord, AttendanceRecordDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.Name))
            .ForMember(d => d.Date, o => o.MapFrom(s => s.Date.ToString("yyyy-MM-dd")))
            .ForMember(d => d.CheckInTime, o => o.MapFrom(s => s.CheckInTime.HasValue ? s.CheckInTime.Value.ToString("HH:mm") : null))
            .ForMember(d => d.CheckOutTime, o => o.MapFrom(s => s.CheckOutTime.HasValue ? s.CheckOutTime.Value.ToString("HH:mm") : null))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // Leave Request
        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.Name))
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.StartDate, o => o.MapFrom(s => s.StartDate.ToString("yyyy-MM-dd")))
            .ForMember(d => d.EndDate, o => o.MapFrom(s => s.EndDate.ToString("yyyy-MM-dd")))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // Leave for Dashboard
        CreateMap<LeaveRequest, RecentLeaveRequestDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.Name))
            .ForMember(d => d.EmployeeAvatar, o => o.MapFrom(s => s.Employee.Avatar))
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Duration, o => o.MapFrom(s =>
                s.StartDate == s.EndDate
                    ? $"1 Day ({s.StartDate:MMM dd})"
                    : $"{(s.EndDate.DayNumber - s.StartDate.DayNumber + 1)} Days ({s.StartDate:MMM dd}-{s.EndDate:dd})"));

        // Payroll
        CreateMap<PayrollSlip, PayrollSlipDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.Name))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // Job Posting
        CreateMap<JobPosting, JobPostingDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CandidateCount, o => o.MapFrom(s => s.Candidates.Count));
        CreateMap<CreateJobPostingDto, JobPosting>();

        // Candidate
        CreateMap<Candidate, CandidateDto>()
            .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.JobPosting.Title))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
        CreateMap<CreateCandidateDto, Candidate>();

        // Interview
        CreateMap<InterviewSchedule, InterviewScheduleDto>()
            .ForMember(d => d.CandidateName, o => o.MapFrom(s => s.Candidate.Name))
            .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.Candidate.JobPosting.Title))
            .ForMember(d => d.InterviewType, o => o.MapFrom(s => s.InterviewType.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // Interview for Dashboard
        CreateMap<InterviewSchedule, UpcomingInterviewDto>()
            .ForMember(d => d.CandidateName, o => o.MapFrom(s => s.Candidate.Name))
            .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.Candidate.JobPosting.Title))
            .ForMember(d => d.Time, o => o.MapFrom(s => s.InterviewDate.ToString("hh:mm tt")));

        // Performance Review
        CreateMap<PerformanceReview, PerformanceReviewDto>()
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.Name))
            .ForMember(d => d.ReviewDate, o => o.MapFrom(s => s.ReviewDate.ToString("yyyy-MM-dd")))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // Asset
        CreateMap<Asset, AssetDto>()
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PurchaseDate, o => o.MapFrom(s => s.PurchaseDate.ToString("yyyy-MM-dd")));
        CreateMap<CreateAssetDto, Asset>();

        // Asset Assignment
        CreateMap<AssetAssignment, AssetAssignmentDto>()
            .ForMember(d => d.AssetName, o => o.MapFrom(s => s.Asset.Name))
            .ForMember(d => d.EmployeeName, o => o.MapFrom(s => s.Employee.Name))
            .ForMember(d => d.AssignedDate, o => o.MapFrom(s => s.AssignedDate.ToString("yyyy-MM-dd")))
            .ForMember(d => d.ReturnedDate, o => o.MapFrom(s => s.ReturnedDate.HasValue ? s.ReturnedDate.Value.ToString("yyyy-MM-dd") : null));
    }
}
