using AutoMapper;
using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Services.Interfaces;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Interfaces;

namespace DeviceManagement.Application.Services;

public class DeviceService(IDeviceRepository deviceRepository, IDeviceAssignmentRepository assignmentRepository, IMapper mapper) : IDeviceService
{
    public async Task<IEnumerable<DeviceDto>> GetAllAsync()
    {
        var devices = await deviceRepository.GetAllAsync();
        var result = new List<DeviceDto>();

        foreach (var device in devices)
        {
            var current = await assignmentRepository.GetCurrentAssignmentForDeviceAsync(device.Id);
            var dto = mapper.Map<DeviceDto>(device);
            dto.IsAvailable = current is null;
            dto.CurrentUserName = current?.User?.Name;
            dto.CurrentLocation = current?.Location;
            result.Add(dto);
        }

        return result;
    }

    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        var device = await deviceRepository.GetByIdAsync(id);
        if (device is null) return null;

        var current = await assignmentRepository.GetCurrentAssignmentForDeviceAsync(id);
        var dto = mapper.Map<DeviceDto>(device);
        dto.IsAvailable = current is null;
        dto.CurrentUserName = current?.User?.Name;
        dto.CurrentLocation = current?.Location;
        return dto;
    }

    public async Task<DeviceDto> CreateAsync(DeviceRequestDto dto)
    {
        var device = mapper.Map<Device>(dto);
        var created = await deviceRepository.CreateAsync(device);
        var result = mapper.Map<DeviceDto>(created);
        result.IsAvailable = true;
        return result;
    }

    public async Task<DeviceDto?> UpdateAsync(int id, DeviceRequestDto dto)
    {
        var device = await deviceRepository.GetByIdAsync(id);
        if (device is null) return null;

        mapper.Map(dto, device);
        var updated = await deviceRepository.UpdateAsync(device);
        var current = await assignmentRepository.GetCurrentAssignmentForDeviceAsync(id);
        var result = mapper.Map<DeviceDto>(updated);
        result.IsAvailable = current is null;
        result.CurrentUserName = current?.User?.Name;
        result.CurrentLocation = current?.Location;
        return result;
    }

    public async Task<bool> DeleteAsync(int id) =>
        await deviceRepository.DeleteAsync(id);
}
