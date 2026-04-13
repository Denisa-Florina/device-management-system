using System.Net;
using System.Net.Http.Json;
using DeviceManagement.Application.DTOs;
using DeviceManagement.Domain.Enums;

namespace DeviceManagement.IntegrationTests;

public class DevicesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DevicesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private DeviceRequestDto CreateSampleDevice(string name = "Test Phone") => new()
    {
        Name = name,
        Manufacturer = "TestCorp",
        Type = DeviceType.Phone,
        OperatingSystem = "Android",
        OSVersion = "14",
        Processor = "Snapdragon 8 Gen 3",
        RAM = 12
    };

    [Fact]
    public async Task CreateDevice_ReturnsCreated_WithDeviceData()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var dto = CreateSampleDevice("Galaxy Test");

        var response = await client.PostAsJsonAsync("/api/devices", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var device = await response.Content.ReadFromJsonAsync<DeviceDto>();
        Assert.NotNull(device);
        Assert.Equal("Galaxy Test", device.Name);
        Assert.Equal("TestCorp", device.Manufacturer);
        Assert.Equal(12, device.RAM);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithDeviceList()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync("/api/devices", CreateSampleDevice("ListTest"));

        var response = await client.GetAsync("/api/devices");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var devices = await response.Content.ReadFromJsonAsync<List<DeviceDto>>();
        Assert.NotNull(devices);
        Assert.Contains(devices, d => d.Name == "ListTest");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForNonExistentDevice()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/devices/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDevice_ReturnsOk_WithUpdatedData()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var createResponse = await client.PostAsJsonAsync("/api/devices", CreateSampleDevice("BeforeUpdate"));
        var created = await createResponse.Content.ReadFromJsonAsync<DeviceDto>();

        var updateDto = CreateSampleDevice("AfterUpdate");
        updateDto.RAM = 16;
        var response = await client.PutAsJsonAsync($"/api/devices/{created!.Id}", updateDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<DeviceDto>();
        Assert.Equal("AfterUpdate", updated!.Name);
        Assert.Equal(16, updated.RAM);
    }

    [Fact]
    public async Task SearchDevices_ReturnsMatchingResults()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        await client.PostAsJsonAsync("/api/devices", CreateSampleDevice("Pixel 9 Pro"));
        await client.PostAsJsonAsync("/api/devices", CreateSampleDevice("Galaxy S24"));

        var response = await client.GetAsync("/api/devices/search?q=Pixel");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var devices = await response.Content.ReadFromJsonAsync<List<DeviceDto>>();
        Assert.NotNull(devices);
        Assert.Contains(devices, d => d.Name == "Pixel 9 Pro");
        Assert.DoesNotContain(devices, d => d.Name == "Galaxy S24");
    }

    [Fact]
    public async Task GenerateDescription_ReturnsOk_WithDescription()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();
        var dto = CreateSampleDevice("AI Test Phone");

        var response = await client.PostAsJsonAsync("/api/devices/generate-description", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<DescriptionResponse>();
        Assert.NotNull(result);
        Assert.Contains("TestCorp", result.Description);
    }

    [Fact]
    public async Task CustomerCannotCreateDevice_ReturnsForbidden()
    {
        var client = await _factory.CreateAuthenticatedClientAsync("Customer");

        var response = await client.PostAsJsonAsync("/api/devices", CreateSampleDevice());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private record DescriptionResponse(string Description);
}
