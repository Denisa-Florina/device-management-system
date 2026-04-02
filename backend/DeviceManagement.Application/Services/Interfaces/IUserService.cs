using DeviceManagement.Application.DTOs;

namespace DeviceManagement.Application.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(UserRequestDto dto);
    Task<UserDto?> UpdateAsync(int id, UserRequestDto dto);
    Task<bool> DeleteAsync(int id);
}
