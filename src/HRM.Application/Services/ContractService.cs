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

public class ContractService : IContractService
{
    private readonly IRepository<Contract> _contractRepository;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<ContractService> _logger;

    public ContractService(
        IRepository<Contract> contractRepository,
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<ContractService> logger)
    {
        _contractRepository = contractRepository;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<ContractDto>>> GetAllAsync(QueryParameters parameters)
    {
        var (contracts, total) = await _contractRepository.GetPagedAsync(
            parameters.PageNumber, parameters.PageSize,
            null, null, c => c.Employee);
            
        var dtos = _mapper.Map<IEnumerable<ContractDto>>(contracts);

        return ApiResponse<PagedResult<ContractDto>>.Ok(new PagedResult<ContractDto>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        });
    }

    public async Task<ApiResponse<ContractDto>> GetByIdAsync(int id)
    {
        var entities = await _contractRepository.FindAsync(c => c.Id == id, c => c.Employee);
        var contract = entities.FirstOrDefault();
        if (contract == null)
            return ApiResponse<ContractDto>.Fail("Contract not found");

        var dto = _mapper.Map<ContractDto>(contract);
        return ApiResponse<ContractDto>.Ok(dto);
    }

    public async Task<ApiResponse<ContractDto>> CreateAsync(CreateContractDto dto)
    {
        try
        {
            var contract = _mapper.Map<Contract>(dto);
            await _contractRepository.AddAsync(contract);
            await _uow.SaveChangesAsync();

            var createdDto = _mapper.Map<ContractDto>(contract);
            return ApiResponse<ContractDto>.Ok(createdDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contract");
            return ApiResponse<ContractDto>.Fail("Error creating contract");
        }
    }

    public async Task<ApiResponse<ContractDto>> UpdateAsync(int id, CreateContractDto dto)
    {
        try
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return ApiResponse<ContractDto>.Fail("Contract not found");

            _mapper.Map(dto, contract);
            _contractRepository.Update(contract);
            await _uow.SaveChangesAsync();

            var updatedDto = _mapper.Map<ContractDto>(contract);
            return ApiResponse<ContractDto>.Ok(updatedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contract");
            return ApiResponse<ContractDto>.Fail("Error updating contract");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return ApiResponse<bool>.Fail("Contract not found");

            _contractRepository.SoftDelete(contract);
            await _uow.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contract");
            return ApiResponse<bool>.Fail("Error deleting contract");
        }
    }
}
