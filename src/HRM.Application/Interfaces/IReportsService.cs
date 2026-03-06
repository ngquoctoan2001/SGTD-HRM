namespace HRM.Application.Interfaces;

public interface IReportsService
{
    Task<byte[]> ExportPayrollToCsvAsync(int month, int year);
}
