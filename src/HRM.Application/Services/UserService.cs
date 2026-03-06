using AutoMapper;
using HRM.Application.DTOs;
using HRM.Application.DTOs.Common;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using HRM.Domain.Interfaces;

namespace HRM.Application.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IRepository<User> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo;
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetAllAsync(QueryParameters p)
    {
        var (items, total) = await _repo.GetPagedAsync(p.PageNumber, p.PageSize,
            filter: string.IsNullOrEmpty(p.Search) ? null : u => u.Name.Contains(p.Search) || u.Email.Contains(p.Search) || u.Username.Contains(p.Search));
        return ApiResponse<PagedResult<UserDto>>.Ok(
            new PagedResult<UserDto> { Items = _mapper.Map<IEnumerable<UserDto>>(items), TotalCount = total, PageNumber = p.PageNumber, PageSize = p.PageSize }
        );
    }

    public async Task<ApiResponse<UserDto>> GetProfileAsync(int userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<UserDto>.Fail("User not found");
        return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }

    public async Task<ApiResponse<UserDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<UserDto>.Fail("User not found");

        user.Name = dto.Name;
        user.Phone = dto.Phone;
        user.Bio = dto.Bio;
        
        _repo.Update(user);
        await _uow.SaveChangesAsync();
        return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(user), "Profile updated successfully");
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user == null) return ApiResponse<bool>.Fail("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return ApiResponse<bool>.Fail("Vui lòng nhập đúng mật khẩu hiện tại");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        _repo.Update(user);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "Password changed successfully");
    }

    public async Task<ApiResponse<UserDto>> UpdateRoleAsync(int id, UpdateUserRoleDto dto)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return ApiResponse<UserDto>.Fail("User not found");

        if (Enum.TryParse<UserRole>(dto.Role, out var role))
        {
            user.Role = role;
            _repo.Update(user);
            await _uow.SaveChangesAsync();
            return ApiResponse<UserDto>.Ok(_mapper.Map<UserDto>(user), "Role updated successfully");
        }
        return ApiResponse<UserDto>.Fail("Invalid role");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return ApiResponse<bool>.Fail("User not found");
        _repo.SoftDelete(user);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.Ok(true, "User deleted successfully");
    }
}
