using HRM.Application.DTOs;
using HRM.Application.DTOs.Common;

namespace HRM.Application.Interfaces;

public interface ITrainingCourseService
{
    Task<ApiResponse<PagedResult<TrainingCourseDto>>> GetAllAsync(QueryParameters parameters);
    Task<ApiResponse<TrainingCourseDto>> GetByIdAsync(int id);
    Task<ApiResponse<TrainingCourseDto>> CreateAsync(CreateTrainingCourseDto dto);
    Task<ApiResponse<TrainingCourseDto>> UpdateAsync(int id, CreateTrainingCourseDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
