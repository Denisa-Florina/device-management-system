using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Services.Interfaces;

namespace DeviceManagement.IntegrationTests;

public class FakeAIDescriptionService : IAIDescriptionService
{
    public Task<string> GenerateDescriptionAsync(DeviceRequestDto device)
    {
        return Task.FromResult($"A {device.Manufacturer} {device.Name} device.");
    }
}
