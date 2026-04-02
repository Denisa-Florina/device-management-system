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
    
    public async Task<DeviceAssignmentDto> AssignAsync(DeviceAssignmentRequestDto dto)
    {
        var existing = await assignmentRepository.GetCurrentAssignmentForDeviceAsync(dto.DeviceId);
        if (existing is not null)
            throw new InvalidOperationException($"Device {dto.DeviceId} is already assigned to another user.");

        _ = await deviceRepository.GetByIdAsync(dto.DeviceId)
            ?? throw new KeyNotFoundException($"Device {dto.DeviceId} not found.");
        _ = await userRepository.GetByIdAsync(dto.UserId)
            ?? throw new KeyNotFoundException($"User {dto.UserId} not found.");

        var assignment = mapper.Map<DeviceAssignment>(dto);
        assignment.AssignedDate = DateTime.UtcNow;

        var created = await assignmentRepository.CreateAsync(assignment);
        var result = await assignmentRepository.GetByIdAsync(created.Id);
        return mapper.Map<DeviceAssignmentDto>(result!);
    }
}
