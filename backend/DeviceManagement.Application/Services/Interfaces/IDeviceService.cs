using DeviceManagement.Application.DTOs;

namespace DeviceManagement.Application.Services.Interfaces;

public interface IDeviceService
{
    Task<IEnumerable<DeviceDto>> GetAllAsync();
    Task<DeviceDto?> GetByIdAsync(int id);
    Task<DeviceDto> CreateAsync(DeviceRequestDto dto);
    Task<DeviceDto?> UpdateAsync(int id, DeviceRequestDto dto);
    Task<bool> DeleteAsync(int id);
}
