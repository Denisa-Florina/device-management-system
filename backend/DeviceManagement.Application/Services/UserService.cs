using AutoMapper;
using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Services.Interfaces;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Interfaces;

namespace DeviceManagement.Application.Services;

public class UserService(IUserRepository userRepository, IDeviceAssignmentRepository assignmentRepository, IMapper mapper) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await userRepository.GetAllAsync();
        var assignments = await assignmentRepository.GetAllAsync();
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var current = assignments.FirstOrDefault(a => a.UserId == user.Id && a.ReturnedDate == null);
            var dto = mapper.Map<UserDto>(user);
            dto.CurrentDeviceName = current?.Device?.Name;
            result.Add(dto);
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null) return null;

        var assignments = await assignmentRepository.GetAllAsync();
        var current = assignments.FirstOrDefault(a => a.UserId == id && a.ReturnedDate == null);
        var dto = mapper.Map<UserDto>(user);
        dto.CurrentDeviceName = current?.Device?.Name;
        return dto;
    }

    public async Task<UserDto> CreateAsync(UserRequestDto dto)
    {
        var user = mapper.Map<User>(dto);
        var created = await userRepository.CreateAsync(user);
        return mapper.Map<UserDto>(created);
    }

    public async Task<UserDto?> UpdateAsync(int id, UserRequestDto dto)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null) return null;

        mapper.Map(dto, user);
        var updated = await userRepository.UpdateAsync(user);
        var assignments = await assignmentRepository.GetAllAsync();
        var current = assignments.FirstOrDefault(a => a.UserId == id && a.ReturnedDate == null);
        var result = mapper.Map<UserDto>(updated);
        result.CurrentDeviceName = current?.Device?.Name;
        return result;
    }

    public async Task<bool> DeleteAsync(int id) =>
        await userRepository.DeleteAsync(id);
}
