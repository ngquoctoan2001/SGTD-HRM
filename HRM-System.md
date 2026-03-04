1️⃣ SYSTEM OVERVIEW

Build a production-ready HRM Web Application with:

Frontend: ReactJS + Vite + TypeScript + TailwindCSS

Backend: .NET 10 Web API

ORM: Entity Framework Core

Database: SQLite

Architecture: Clean Architecture + Repository Pattern

Auth: JWT Authentication

Role-based Authorization (Admin/User)

UI Layout: Match provided dashboard design (sidebar, KPI cards, chart, tables, interview list panel)

System Modules:

Authentication

Employee Management

Attendance

Leave Management

Payroll

Recruitment

Interview Scheduling

Performance Review

Asset Management

Dashboard Analytics

2️⃣ ARCHITECTURE STRUCTURE
Backend (.NET 10)

Use Clean Architecture:

src/
 ├── HRM.API
 ├── HRM.Application
 ├── HRM.Domain
 ├── HRM.Infrastructure
Frontend (React)
hrm-frontend/
 ├── src/
 │   ├── api/
 │   ├── components/
 │   ├── layouts/
 │   ├── pages/
 │   ├── hooks/
 │   ├── store/
 │   ├── routes/
 │   ├── types/
 │   └── utils/
3️⃣ DATABASE

Use SQLite.

Connection string:

Data Source=hrm.db

Use provided SQL schema as database model reference.

Convert all tables into EF Core Entities.

Use:

Fluent API

Index configuration

Enum conversions instead of string for Status fields

Soft delete support (add IsDeleted to major entities)

Run automatic migration on startup.

4️⃣ AUTHENTICATION

Implement:

Register

Login

Refresh Token

JWT generation

Role-based access

Roles:

Admin

User

Protect APIs with [Authorize]

5️⃣ BACKEND FEATURES

Create full CRUD APIs for:

Employees

JobPosting

Candidate

InterviewSchedule

AttendanceRecord

LeaveRequest

PayrollSlip

PerformanceReview

Asset

AssetAssignment

Include:

Pagination

Filtering

Sorting

DTO mapping using AutoMapper

Global Exception Middleware

FluentValidation

Swagger enabled

6️⃣ DASHBOARD API

Create endpoint:

GET /api/dashboard/overview

Return:

{
  totalEmployees,
  monthlyAttendancePercentage,
  pendingLeaveRequests,
  upcomingInterviewsCount,
  recruitmentTrend: [
    { month, hired, applications }
  ],
  recentLeaveRequests: [],
  upcomingInterviews: []
}
7️⃣ FRONTEND UI REQUIREMENTS
🎨 Layout

Replicate design from screenshot:

Left Sidebar (fixed)

Topbar with:

Search input

Notification icon

Settings icon

Main Content Area

Card-based design

Soft shadow

Rounded corners

Clean enterprise UI

Use:

TailwindCSS

HeroIcons

Recharts (for chart)

React Router v6

Axios

Zustand or Redux Toolkit

8️⃣ DASHBOARD PAGE UI STRUCTURE

Top KPI Cards:

Total Employees

Monthly Attendance %

Pending Leave Requests

Upcoming Interviews

Below:

Left:

Monthly Recruitment Trend (Line Chart)

Right:

Upcoming Interviews list panel

Bottom:

Recent Leave Requests table

9️⃣ UI COMPONENTS TO BUILD

Reusable components:

Sidebar

Topbar

Card

DataTable

Modal

FormInput

SelectInput

StatusBadge

Pagination

ConfirmDialog

ChartCard

🔟 STATE MANAGEMENT

Use Zustand or Redux Toolkit.

Global state:

Auth

User info

Theme

Notifications

11️⃣ MCP STITCH INTEGRATION

Use MCP Stitch Server to:

Sync UI layout

Generate page components

Maintain design consistency

Auto-generate form layouts based on DTO

Keep spacing, typography consistent

All UI pages must follow Stitch layout grid system.

12️⃣ ROUTING STRUCTURE
/login
/dashboard
/employees
/attendance
/leave
/payroll
/recruitment
/interview
/performance
/assets
/reports
/settings
13️⃣ SECURITY

Hash passwords with BCrypt

Use JWT expiration

Validate all inputs

Prevent SQL Injection

CORS policy

Rate limiting middleware

14️⃣ OPTIONAL ADVANCED FEATURES

If possible implement:

Export payroll to PDF

Export reports to Excel

Email notification for interview scheduling

Dark mode toggle

Activity logging table

15️⃣ DEPLOYMENT READY

Make system ready for:

Docker containerization

Production configuration

Environment variables

Logging with Serilog

16️⃣ CODE QUALITY REQUIREMENTS

SOLID principles

Async/await everywhere

Proper folder separation

No business logic in controllers

Clean DTO mapping

Unit test ready structure

17️⃣ FINAL OUTPUT EXPECTATION

Antigravity should generate:

Full backend solution

Full frontend project

Database migration

Seed data

Working dashboard

Complete CRUD pages

Auth flow

Project must run with:

Backend:

dotnet run

Frontend:

npm install
npm run dev
END OF WORKFLOW