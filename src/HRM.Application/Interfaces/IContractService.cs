using HRM.Application.DTOs;
using HRM.Application.DTOs.Common;

namespace HRM.Application.Interfaces;

public interface IContractService
{
    Task<ApiResponse<PagedResult<ContractDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<ContractDto>> GetByIdAsync(int id);
    Task<ApiResponse<ContractDto>> CreateAsync(CreateContractDto dto);
    Task<ApiResponse<ContractDto>> UpdateAsync(int id, CreateContractDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
