using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Domain.Interfaces;

public interface IDeviceAssignmentRepository
{
    Task<IEnumerable<DeviceAssignment>> GetAllAsync();
    Task<DeviceAssignment?> GetByIdAsync(int id);
    Task<DeviceAssignment> CreateAsync(DeviceAssignment assignment);
    Task<DeviceAssignment> UpdateAsync(DeviceAssignment assignment);
    Task DeleteAsync(int id);
}
