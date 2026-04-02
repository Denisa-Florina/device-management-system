using AutoMapper;
using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Services.Interfaces;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Interfaces;

namespace DeviceManagement.Application.Services;

public class DeviceAssignmentService(IDeviceAssignmentRepository assignmentRepository, IDeviceRepository deviceRepository, IUserRepository userRepository, IMapper mapper) : IDeviceAssignmentService
{
    public async Task<IEnumerable<DeviceAssignmentDto>> GetAllAsync()
    {
        var assignments = await assignmentRepository.GetAllAsync();
        return assignments.Select(mapper.Map<DeviceAssignmentDto>);
    }

    public async Task<DeviceAssignmentDto?> GetByIdAsync(int id)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id);
        return assignment is null ? null : mapper.Map<DeviceAssignmentDto>(assignment);
    }

    public async Task<DeviceAssignmentDto?> ReturnDeviceAsync(int id)
    {
        var assignment = await assignmentRepository.GetByIdAsync(id);
        if (assignment is null) return null;

        assignment.ReturnedDate = DateTime.UtcNow;
        var updated = await assignmentRepository.UpdateAsync(assignment);
        return mapper.Map<DeviceAssignmentDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id) =>
        await assignmentRepository.DeleteAsync(id);

}
