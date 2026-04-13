using DeviceManagement.Application.DTOs;

namespace DeviceManagement.Application.Services.Interfaces;

public interface IAIDescriptionService
{
    Task<string> GenerateDescriptionAsync(DeviceRequestDto device);
}
