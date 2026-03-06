using System.Text;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Domain.Interfaces;

namespace HRM.Application.Services;

public class ReportsService : IReportsService
{
    private readonly IRepository<PayrollSlip> _payrollRepo;

    public ReportsService(IRepository<PayrollSlip> payrollRepo)
    {
        _payrollRepo = payrollRepo;
    }

    public async Task<byte[]> ExportPayrollToCsvAsync(int month, int year)
    {
        var prefix = $"{year}-{month:D2}";
        var slips = await _payrollRepo.FindAsync(
            p => p.MonthYear.StartsWith(prefix),
            p => p.Employee
        );

        var csv = new StringBuilder();
        csv.AppendLine("Id,Employee,MonthYear,BasicSalary,TotalAllowances,TotalDeductions,NetSalary,Status");
        
        foreach (var slip in slips)
        {
            csv.AppendLine($"{slip.Id},\"{slip.Employee.Name}\",{slip.MonthYear},{slip.BasicSalary},{slip.TotalAllowances},{slip.TotalDeductions},{slip.NetSalary},{slip.Status}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }
}
