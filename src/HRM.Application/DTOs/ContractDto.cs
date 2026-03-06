namespace HRM.Application.DTOs;

public class ContractDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string ContractType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal BasicSalary { get; set; }
    public string Status { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}

public class CreateContractDto
{
    public int EmployeeId { get; set; }
    public string ContractType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal BasicSalary { get; set; }
    public string Status { get; set; } = "Active";
    public string FileUrl { get; set; } = string.Empty;
}
