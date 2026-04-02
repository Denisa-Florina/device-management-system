using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeviceAssignmentsController(IDeviceAssignmentService assignmentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await assignmentService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var assignment = await assignmentService.GetByIdAsync(id);
        return assignment is null ? NotFound() : Ok(assignment);
    }

    [HttpPost]
    public async Task<IActionResult> Assign([FromBody] DeviceAssignmentRequestDto dto)
    {
        try
        {
            var created = await assignmentService.AssignAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/return")]
    public async Task<IActionResult> ReturnDevice(int id)
    {
        var updated = await assignmentService.ReturnDeviceAsync(id);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await assignmentService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
