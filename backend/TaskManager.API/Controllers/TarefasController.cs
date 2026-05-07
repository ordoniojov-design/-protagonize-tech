using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarefasController(TarefaService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var result = await service.GetAllAsync(status);
        return ToResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return ToResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTarefaDto dto)
    {
        var result = await service.CreateAsync(dto);
        if (!result.IsSuccess) return ToResponse(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTarefaDto dto)
    {
        var result = await service.UpdateAsync(id, dto);
        return ToResponse(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var result = await service.UpdateStatusAsync(id, dto);
        return ToResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await service.DeleteAsync(id);
        if (!result.IsSuccess) return ToResponse(result);
        return NoContent();
    }

    private ObjectResult ToResponse<T>(ServiceResult<T> result) =>
        StatusCode((int)result.StatusCode, result.IsSuccess
            ? result.Data
            : new { message = result.Message });
}
