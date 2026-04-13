using System.Text.RegularExpressions;
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
            result.Add(await MapToDto(device));
        }

        return result;
    }

    public async Task<IEnumerable<DeviceDto>> GetForUserAsync(int userId)
    {
        var devices = await deviceRepository.GetAllAsync();
        var result = new List<DeviceDto>();

        foreach (var device in devices)
        {
            var dto = await MapToDto(device);

            if (dto.IsAvailable || dto.CurrentUserId == userId)
                result.Add(dto);
        }

        return result;
    }

    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        var device = await deviceRepository.GetByIdAsync(id);
        if (device is null) return null;

        return await MapToDto(device);
    }

    private async Task<DeviceDto> MapToDto(Device device)
    {
        var current = await assignmentRepository.GetCurrentAssignmentForDeviceAsync(device.Id);
        var dto = mapper.Map<DeviceDto>(device);
        dto.IsAvailable = current is null;
        dto.CurrentUserId = current?.UserId;
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
        return await MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id) =>
        await deviceRepository.DeleteAsync(id);

    public async Task<IEnumerable<DeviceDto>> SearchAsync(string query, int? userId = null)
    {
        var tokens = TokenizeQuery(query);
        if (tokens.Length == 0) return [];

        var devices = await deviceRepository.GetAllAsync();
        var scored = new List<(DeviceDto dto, int score)>();

        foreach (var device in devices)
        {
            var dto = await MapToDto(device);
            
            if (userId.HasValue && !dto.IsAvailable && dto.CurrentUserId != userId.Value)
                continue;

            var score = ComputeScore(device, tokens);
            if (score > 0)
                scored.Add((dto, score));
        }

        return scored
            .OrderByDescending(x => x.score)
            .Select(x => x.dto);
    }

    private static string[] TokenizeQuery(string query)
    {
        var clean = Regex.Replace(query.ToLowerInvariant(), @"[^a-z0-9\s]", " ");
        var collapsed = Regex.Replace(clean, @"\s+", " ").Trim();
        return collapsed
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length >= 1)
            .Distinct()
            .ToArray();
    }

    private static string NormalizeText(string? text) =>
        text is null ? string.Empty : Regex.Replace(text.ToLowerInvariant(), @"\s+", " ").Trim();

    private static int ComputeScore(Device device, string[] tokens)
    {
        var nameFull  = NormalizeText(device.Name);
        var manuFull  = NormalizeText(device.Manufacturer);
        var procFull  = NormalizeText(device.Processor);
        var ramStr    = device.RAM.ToString();

        var nameWords = nameFull.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var manuWords = manuFull.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var procWords = procFull.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var score = 0;
        foreach (var token in tokens)
        {

            if (nameFull == token)                    score += 16;
            else if (nameWords.Contains(token))       score += 12;
            else if (nameFull.Contains(token))        score += 8;


            if (manuFull == token)                    score += 10;
            else if (manuWords.Contains(token))       score += 8;
            else if (manuFull.Contains(token))        score += 4;


            if (procWords.Contains(token))            score += 4;
            else if (procFull.Contains(token))        score += 2;


            if (token == ramStr || token == ramStr + "gb") score += 3;
        }
        return score;
    }
}
