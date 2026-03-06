using AutoMapper;
using HRM.Application.DTOs;
using HRM.Application.DTOs.Common;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HRM.Application.Services;

public class TrainingCourseService : ITrainingCourseService
{
    private readonly IRepository<TrainingCourse> _repository;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<TrainingCourseService> _logger;

    public TrainingCourseService(
        IRepository<TrainingCourse> repository,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<TrainingCourseService> logger)
    {
        _repository = repository;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<TrainingCourseDto>>> GetAllAsync(QueryParameters parameters)
    {
        var (records, total) = await _repository.GetPagedAsync(
            parameters.PageNumber, parameters.PageSize);
            
        var dtos = _mapper.Map<IEnumerable<TrainingCourseDto>>(records);

        return ApiResponse<PagedResult<TrainingCourseDto>>.Ok(new PagedResult<TrainingCourseDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        });
    }

    public async Task<ApiResponse<TrainingCourseDto>> GetByIdAsync(int id)
    {
        var record = await _repository.GetByIdAsync(id);
        if (record == null)
            return ApiResponse<TrainingCourseDto>.Fail("Course not found");

        var dto = _mapper.Map<TrainingCourseDto>(record);
        return ApiResponse<TrainingCourseDto>.Ok(dto);
    }

    public async Task<ApiResponse<TrainingCourseDto>> CreateAsync(CreateTrainingCourseDto dto)
    {
        try
        {
            var record = _mapper.Map<TrainingCourse>(dto);
            await _repository.AddAsync(record);
            await _uow.SaveChangesAsync();

            var createdDto = _mapper.Map<TrainingCourseDto>(record);
            return ApiResponse<TrainingCourseDto>.Ok(createdDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating training course");
            return ApiResponse<TrainingCourseDto>.Fail("Error creating course");
        }
    }

    public async Task<ApiResponse<TrainingCourseDto>> UpdateAsync(int id, CreateTrainingCourseDto dto)
    {
        try
        {
            var record = await _repository.GetByIdAsync(id);
            if (record == null)
                return ApiResponse<TrainingCourseDto>.Fail("Course not found");

            _mapper.Map(dto, record);
            _repository.Update(record);
            await _uow.SaveChangesAsync();

            var updatedDto = _mapper.Map<TrainingCourseDto>(record);
            return ApiResponse<TrainingCourseDto>.Ok(updatedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating training course");
            return ApiResponse<TrainingCourseDto>.Fail("Error updating course");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var record = await _repository.GetByIdAsync(id);
            if (record == null)
                return ApiResponse<bool>.Fail("Course not found");

            _repository.SoftDelete(record);
            await _uow.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting training course");
            return ApiResponse<bool>.Fail("Error deleting course");
        }
    }
}
