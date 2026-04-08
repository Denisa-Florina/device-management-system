using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Services.Interfaces;
using DeviceManagement.Application.Settings;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Interfaces;
using DeviceManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DeviceManagement.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtSettings> jwtSettings,
    IUserRepository userRepository) : IAuthService
{
    private readonly JwtSettings _jwt = jwtSettings.Value;

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("Email is already registered.");

        var domainUser = new User
        {
            Name = dto.Name,
            Role = "Customer",
            Location = dto.Location
        };
        var createdUser = await userRepository.CreateAsync(domainUser);

        var appUser = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            UserId = createdUser.Id
        };

        var result = await userManager.CreateAsync(appUser, dto.Password);
        if (!result.Succeeded)
        {
            await userRepository.DeleteAsync(createdUser.Id);
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        await userManager.AddToRoleAsync(appUser, "Customer");

        return new AuthResponseDto
        {
            Token = GenerateToken(appUser, "Customer"),
            Email = appUser.Email!,
            Role = "Customer",
            UserId = appUser.UserId
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var appUser = await userManager.FindByEmailAsync(dto.Email);
        if (appUser is null) return null;

        var valid = await userManager.CheckPasswordAsync(appUser, dto.Password);
        if (!valid) return null;

        var roles = await userManager.GetRolesAsync(appUser);
        var role = roles.FirstOrDefault() ?? "Customer";

        return new AuthResponseDto
        {
            Token = GenerateToken(appUser, role),
            Email = appUser.Email!,
            Role = role,
            UserId = appUser.UserId
        };
    }

    public async Task<UserDto> AdminCreateUserAsync(AdminCreateUserRequestDto dto)
    {
        var validRoles = new[] { "Admin", "Customer" };
        if (!validRoles.Contains(dto.Role))
            throw new InvalidOperationException($"Invalid role '{dto.Role}'. Must be Admin or Customer.");

        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("Email is already registered.");

        var domainUser = new User
        {
            Name = dto.Name,
            Role = dto.Role,
            Location = dto.Location
        };
        var createdUser = await userRepository.CreateAsync(domainUser);

        var appUser = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            UserId = createdUser.Id
        };

        var result = await userManager.CreateAsync(appUser, dto.Password);
        if (!result.Succeeded)
        {
            await userRepository.DeleteAsync(createdUser.Id);
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        await userManager.AddToRoleAsync(appUser, dto.Role);

        return new UserDto
        {
            Id = createdUser.Id,
            Name = createdUser.Name,
            Role = createdUser.Role,
            Location = createdUser.Location
        };
    }

    private string GenerateToken(ApplicationUser user, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.Role, role),
            new Claim("userId", user.UserId?.ToString() ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwt.ExpirationHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
