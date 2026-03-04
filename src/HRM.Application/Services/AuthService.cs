using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using HRM.Application.DTOs.Auth;
using HRM.Application.DTOs.Common;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Domain.Enums;
using HRM.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HRM.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public AuthService(IRepository<User> userRepo, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _config = config;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var users = await _userRepo.FindAsync(u => u.Username == request.Username);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<AuthResponse>.Fail("Invalid username or password");

        var authResponse = GenerateTokens(user);
        user.RefreshToken = authResponse.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<AuthResponse>.Ok(authResponse, "Login successful");
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepo.FindAsync(u => u.Username == request.Username || u.Email == request.Email);
        if (existing.Any())
            return ApiResponse<AuthResponse>.Fail("Username or email already exists");

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Role = UserRole.User,
            Bio = "",
            AvatarUrl = ""
        };

        await _userRepo.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var authResponse = GenerateTokens(user);
        user.RefreshToken = authResponse.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<AuthResponse>.Ok(authResponse, "Registration successful");
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
            return ApiResponse<AuthResponse>.Fail("Invalid token");

        var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _userRepo.GetByIdAsync(userId);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return ApiResponse<AuthResponse>.Fail("Invalid refresh token");

        var authResponse = GenerateTokens(user);
        user.RefreshToken = authResponse.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _userRepo.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<AuthResponse>.Ok(authResponse);
    }

    private AuthResponse GenerateTokens(User user)
    {
        var jwtKey = _config["Jwt:Key"] ?? "HRM-SGTD-Super-Secret-Key-2024-Must-Be-At-Least-32-Characters";
        var jwtIssuer = _config["Jwt:Issuer"] ?? "HRM-API";
        var jwtAudience = _config["Jwt:Audience"] ?? "HRM-Frontend";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("name", user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiration,
            signingCredentials: creds);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            Expiration = expiration,
            User = _mapper.Map<UserInfoDto>(user)
        };
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtKey = _config["Jwt:Key"] ?? "HRM-SGTD-Super-Secret-Key-2024-Must-Be-At-Least-32-Characters";
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;
            return principal;
        }
        catch { return null; }
    }
}
