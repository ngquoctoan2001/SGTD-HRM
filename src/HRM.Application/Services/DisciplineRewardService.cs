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

public class DisciplineRewardService : IDisciplineRewardService
{
    private readonly IRepository<DisciplineReward> _repository;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<DisciplineRewardService> _logger;

    public DisciplineRewardService(
        IRepository<DisciplineReward> repository,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<DisciplineRewardService> logger)
    {
        _repository = repository;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<DisciplineRewardDto>>> GetAllAsync(QueryParameters parameters)
    {
        var (records, total) = await _repository.GetPagedAsync(
            parameters.PageNumber, parameters.PageSize,
            null, null, c => c.Employee);
            
        var dtos = _mapper.Map<IEnumerable<DisciplineRewardDto>>(records);

        return ApiResponse<PagedResult<DisciplineRewardDto>>.Ok(new PagedResult<DisciplineRewardDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        });
    }

    public async Task<ApiResponse<DisciplineRewardDto>> GetByIdAsync(int id)
    {
        var entities = await _repository.FindAsync(c => c.Id == id, c => c.Employee);
        var record = entities.FirstOrDefault();
        if (record == null)
            return ApiResponse<DisciplineRewardDto>.Fail("Record not found");

        var dto = _mapper.Map<DisciplineRewardDto>(record);
        return ApiResponse<DisciplineRewardDto>.Ok(dto);
    }

    public async Task<ApiResponse<DisciplineRewardDto>> CreateAsync(CreateDisciplineRewardDto dto)
    {
        try
        {
            var record = _mapper.Map<DisciplineReward>(dto);
            await _repository.AddAsync(record);
            await _uow.SaveChangesAsync();

            var createdDto = _mapper.Map<DisciplineRewardDto>(record);
            return ApiResponse<DisciplineRewardDto>.Ok(createdDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating discipline/reward record");
            return ApiResponse<DisciplineRewardDto>.Fail("Error creating record");
        }
    }

    public async Task<ApiResponse<DisciplineRewardDto>> UpdateAsync(int id, CreateDisciplineRewardDto dto)
    {
        try
        {
            var record = await _repository.GetByIdAsync(id);
            if (record == null)
                return ApiResponse<DisciplineRewardDto>.Fail("Record not found");

            _mapper.Map(dto, record);
            _repository.Update(record);
            await _uow.SaveChangesAsync();

            var updatedDto = _mapper.Map<DisciplineRewardDto>(record);
            return ApiResponse<DisciplineRewardDto>.Ok(updatedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating discipline/reward record");
            return ApiResponse<DisciplineRewardDto>.Fail("Error updating record");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var record = await _repository.GetByIdAsync(id);
            if (record == null)
                return ApiResponse<bool>.Fail("Record not found");

            _repository.SoftDelete(record);
            await _uow.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting discipline/reward record");
            return ApiResponse<bool>.Fail("Error deleting record");
        }
    }
}
