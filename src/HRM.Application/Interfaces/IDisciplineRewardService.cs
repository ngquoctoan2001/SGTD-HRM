using HRM.Application.DTOs;
using HRM.Application.DTOs.Common;

namespace HRM.Application.Interfaces;

public interface IDisciplineRewardService
{
    Task<ApiResponse<PagedResult<DisciplineRewardDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<DisciplineRewardDto>> GetByIdAsync(int id);
    Task<ApiResponse<DisciplineRewardDto>> CreateAsync(CreateDisciplineRewardDto dto);
    Task<ApiResponse<DisciplineRewardDto>> UpdateAsync(int id, CreateDisciplineRewardDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
