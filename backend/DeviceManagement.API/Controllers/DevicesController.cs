using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController(IDeviceService deviceService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await deviceService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var device = await deviceService.GetByIdAsync(id);
        return device is null ? NotFound() : Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceRequestDto dto)
    {
        var created = await deviceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] DeviceRequestDto dto)
    {
        var updated = await deviceService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await deviceService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
