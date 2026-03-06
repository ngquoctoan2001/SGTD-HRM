using System.Text;
using HRM.API.Middleware;
using HRM.Application.Interfaces;
using HRM.Application.Mapping;
using HRM.Application.Services;
using HRM.Domain.Interfaces;
using HRM.Infrastructure.Data;
using HRM.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ==================== SERILOG ====================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/hrm-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// ==================== DATABASE ====================
builder.Services.AddDbContext<HrmDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=hrm.db"));

// ==================== REPOSITORIES ====================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ==================== SERVICES ====================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<IJobPostingService, JobPostingService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssetAssignmentService, AssetAssignmentService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IDisciplineRewardService, DisciplineRewardService>();
builder.Services.AddScoped<ITrainingCourseService, TrainingCourseService>();

// ==================== AUTOMAPPER ====================
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// ==================== JWT AUTHENTICATION ====================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "HRM-SGTD-Super-Secret-Key-2024-Must-Be-At-Least-32-Characters";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "HRM-API";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "HRM-Frontend";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// ==================== CORS ====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ==================== CONTROLLERS + OPENAPI ====================
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// ==================== MIDDLEWARE PIPELINE ====================
app.UseMiddleware<GlobalExceptionMiddleware>();

// OpenAPI + Scalar API Reference (interactive API docs for testing)
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "HRM System API";
    options.Theme = ScalarTheme.BluePlanet;
    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

// Configure Static Files for Uploads directory
var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads");
if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ==================== AUTO MIGRATION + SEED ====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HrmDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DbSeeder.SeedAsync(db);
}

Log.Information("HRM API started at {Url}", "http://localhost:5000");
Log.Information("API Docs available at: http://localhost:5000/scalar/v1");
app.Run();
