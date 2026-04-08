using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.API.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController(IUserService userService, IAuthService authService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await userService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdminCreateUserRequestDto dto)
    {
        try
        {
            var created = await authService.AdminCreateUserAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserRequestDto dto)
    {
        var updated = await userService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await userService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
