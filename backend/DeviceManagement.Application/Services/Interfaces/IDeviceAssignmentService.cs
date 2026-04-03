using DeviceManagement.Application.DTOs;

namespace DeviceManagement.Application.Services.Interfaces;

public interface IDeviceAssignmentService
{
    Task<IEnumerable<DeviceAssignmentDto>> GetAllAsync();
    Task<DeviceAssignmentDto?> GetByIdAsync(int id);
    Task<DeviceAssignmentDto> AssignAsync(DeviceAssignmentRequestDto dto);
    Task<DeviceAssignmentDto?> ReturnDeviceAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<bool> SelfAssignAsync(int deviceId, string location, int userId);
    Task<bool> SelfUnassignAsync(int userId);
}
