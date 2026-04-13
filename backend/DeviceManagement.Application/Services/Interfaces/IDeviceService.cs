using DeviceManagement.Application.DTOs;

namespace DeviceManagement.Application.Services.Interfaces;

public interface IDeviceService
{
    Task<IEnumerable<DeviceDto>> GetAllAsync();
    Task<IEnumerable<DeviceDto>> GetForUserAsync(int userId);
    Task<IEnumerable<DeviceDto>> SearchAsync(string query, int? userId = null);
    Task<DeviceDto?> GetByIdAsync(int id);
    Task<DeviceDto> CreateAsync(DeviceRequestDto dto);
    Task<DeviceDto?> UpdateAsync(int id, DeviceRequestDto dto);
    Task<bool> DeleteAsync(int id);
}
