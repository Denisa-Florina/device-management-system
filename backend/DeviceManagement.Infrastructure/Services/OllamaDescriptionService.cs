using System.Net.Http.Json;
using System.Text.Json;
using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Services.Interfaces;
using DeviceManagement.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeviceManagement.Infrastructure.Services;

public class OllamaDescriptionService : IAIDescriptionService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<OllamaDescriptionService> _logger;

    public OllamaDescriptionService(HttpClient httpClient, IConfiguration configuration, ILogger<OllamaDescriptionService> logger)
    {
        _httpClient = httpClient;
        _model = configuration["Ollama:Model"] ?? "llama3.2";
        _logger = logger;
    }

    public async Task<string> GenerateDescriptionAsync(DeviceRequestDto device)
    {
        var deviceType = device.Type == DeviceType.Phone ? "phone" : "tablet";

        var prompt = $"""
            Generate a single concise, human-readable description (1-2 sentences) for the following device.
            Focus on its key strengths and suitable use cases. Do not include any extra text or formatting.

            Name: {device.Name}
            Manufacturer: {device.Manufacturer}
            Type: {deviceType}
            Operating System: {device.OperatingSystem} {device.OSVersion}
            Processor: {device.Processor}
            RAM: {device.RAM}GB
            """;

        var request = new
        {
            model = _model,
            prompt,
            stream = false
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/generate", request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("response").GetString()?.Trim()
                   ?? "Unable to generate description.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate device description via Ollama");
            throw new InvalidOperationException("AI description service is unavailable. Make sure Ollama is running.", ex);
        }
    }
}
