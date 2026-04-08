using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.DTOs.Auth;

namespace DeviceManagement.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<UserDto> AdminCreateUserAsync(AdminCreateUserRequestDto dto);
}
