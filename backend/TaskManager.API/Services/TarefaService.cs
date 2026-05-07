using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using TaskManager.API.DTOs;
using TaskManager.API.Models;

namespace TaskManager.API.Services;

public class TarefaService(AppDbContext context)
{
    public async Task<ServiceResult<IEnumerable<TarefaResponseDto>>> GetAllAsync(string? status)
    {
        var query = context.Tarefas.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(t => t.Status == status);

        var tarefas = await query
            .OrderByDescending(t => t.DataCriacao)
            .Select(t => ToDto(t))
            .ToListAsync();

        return ServiceResult<IEnumerable<TarefaResponseDto>>.Ok(tarefas);
    }

    public async Task<ServiceResult<TarefaResponseDto>> GetByIdAsync(int id)
    {
        var tarefa = await context.Tarefas.FindAsync(id);

        return tarefa is null
            ? ServiceResult<TarefaResponseDto>.NotFound($"Tarefa com id {id} não encontrada.")
            : ServiceResult<TarefaResponseDto>.Ok(ToDto(tarefa));
    }

    public async Task<ServiceResult<TarefaResponseDto>> CreateAsync(CreateTarefaDto dto)
    {
        var tarefa = new Tarefa
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Status = dto.Status,
            Prioridade = dto.Prioridade,
            DataCriacao = DateTime.UtcNow
        };

        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync();

        return ServiceResult<TarefaResponseDto>.Created(ToDto(tarefa));
    }

    public async Task<ServiceResult<TarefaResponseDto>> UpdateAsync(int id, UpdateTarefaDto dto)
    {
        var tarefa = await context.Tarefas.FindAsync(id);
        if (tarefa is null)
            return ServiceResult<TarefaResponseDto>.NotFound($"Tarefa com id {id} não encontrada.");

        tarefa.Titulo = dto.Titulo;
        tarefa.Descricao = dto.Descricao;
        tarefa.Status = dto.Status;
        tarefa.Prioridade = dto.Prioridade;
        tarefa.DataAtualizacao = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return ServiceResult<TarefaResponseDto>.Ok(ToDto(tarefa));
    }

    public async Task<ServiceResult<TarefaResponseDto>> UpdateStatusAsync(int id, UpdateStatusDto dto)
    {
        var tarefa = await context.Tarefas.FindAsync(id);
        if (tarefa is null)
            return ServiceResult<TarefaResponseDto>.NotFound($"Tarefa com id {id} não encontrada.");

        tarefa.Status = dto.Status;
        tarefa.DataAtualizacao = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return ServiceResult<TarefaResponseDto>.Ok(ToDto(tarefa));
    }

    public async Task<ServiceResult<TarefaResponseDto>> DeleteAsync(int id)
    {
        var tarefa = await context.Tarefas.FindAsync(id);
        if (tarefa is null)
            return ServiceResult<TarefaResponseDto>.NotFound($"Tarefa com id {id} não encontrada.");

        context.Tarefas.Remove(tarefa);
        await context.SaveChangesAsync();

        return ServiceResult<TarefaResponseDto>.Ok(default!);
    }

    private static TarefaResponseDto ToDto(Tarefa t) => new()
    {
        Id = t.Id,
        Titulo = t.Titulo,
        Descricao = t.Descricao,
        Status = t.Status,
        Prioridade = t.Prioridade,
        DataCriacao = t.DataCriacao,
        DataAtualizacao = t.DataAtualizacao
    };
}
