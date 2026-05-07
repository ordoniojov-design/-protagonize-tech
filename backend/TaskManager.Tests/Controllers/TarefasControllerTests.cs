using Microsoft.AspNetCore.Mvc;
using Xunit;
using TaskManager.API.Controllers;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Services;
using TaskManager.Tests.Helpers;

namespace TaskManager.Tests.Controllers;

public class TarefasControllerTests
{
    private static TarefasController CreateController(string dbName)
        => new(new TarefaService(DbContextFactory.Create(dbName)));

    private static async Task<Tarefa> SeedAsync(string dbName, string titulo = "Tarefa")
    {
        using var ctx = DbContextFactory.Create(dbName);
        var tarefa = new Tarefa { Titulo = titulo, Status = "Pendente", Prioridade = "Média" };
        ctx.Tarefas.Add(tarefa);
        await ctx.SaveChangesAsync();
        return tarefa;
    }

    #region GetAll

    [Fact]
    public async Task GetAll_RetornaOk()
    {
        // Arrange
        var db = nameof(GetAll_RetornaOk);
        await SeedAsync(db);

        // Act
        var result = await CreateController(db).GetAll(null);

        // Assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, ((ObjectResult)result).StatusCode);
    }

    #endregion

    #region GetById

    [Fact]
    public async Task GetById_IdExistente_RetornaOk()
    {
        // Arrange
        var db = nameof(GetById_IdExistente_RetornaOk);
        var tarefa = await SeedAsync(db);

        // Act
        var result = await CreateController(db).GetById(tarefa.Id);

        // Assert
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, obj.StatusCode);
    }

    [Fact]
    public async Task GetById_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var db = nameof(GetById_IdInexistente_RetornaNotFound);

        // Act
        var result = await CreateController(db).GetById(999);

        // Assert
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, obj.StatusCode);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_DadosValidos_RetornaCreatedAtAction()
    {
        // Arrange
        var db = nameof(Create_DadosValidos_RetornaCreatedAtAction);
        var dto = new CreateTarefaDto { Titulo = "Nova", Status = "Pendente", Prioridade = "Alta" };

        // Act
        var result = await CreateController(db).Create(dto);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.IsType<TarefaResponseDto>(created.Value);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_IdExistente_RetornaOk()
    {
        // Arrange
        var db = nameof(Update_IdExistente_RetornaOk);
        var tarefa = await SeedAsync(db);
        var dto = new UpdateTarefaDto { Titulo = "Editada", Status = "Concluída", Prioridade = "Baixa" };

        // Act
        var result = await CreateController(db).Update(tarefa.Id, dto);

        // Assert
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, obj.StatusCode);
    }

    [Fact]
    public async Task Update_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var db = nameof(Update_IdInexistente_RetornaNotFound);
        var dto = new UpdateTarefaDto { Titulo = "X", Status = "Pendente", Prioridade = "Média" };

        // Act
        var result = await CreateController(db).Update(999, dto);

        // Assert
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, obj.StatusCode);
    }

    #endregion

    #region UpdateStatus

    [Fact]
    public async Task UpdateStatus_IdExistente_RetornaOk()
    {
        // Arrange
        var db = nameof(UpdateStatus_IdExistente_RetornaOk);
        var tarefa = await SeedAsync(db);
        var dto = new UpdateStatusDto { Status = "Em Andamento" };

        // Act
        var result = await CreateController(db).UpdateStatus(tarefa.Id, dto);

        // Assert
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, obj.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var db = nameof(UpdateStatus_IdInexistente_RetornaNotFound);
        var dto = new UpdateStatusDto { Status = "Concluída" };

        // Act
        var result = await CreateController(db).UpdateStatus(999, dto);

        // Assert
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, obj.StatusCode);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_IdExistente_RetornaNoContent()
    {
        // Arrange
        var db = nameof(Delete_IdExistente_RetornaNoContent);
        var tarefa = await SeedAsync(db);

        // Act
        var result = await CreateController(db).Delete(tarefa.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var db = nameof(Delete_IdInexistente_RetornaNotFound);

        // Act
        var result = await CreateController(db).Delete(999);

        // Assert
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, obj.StatusCode);
    }

    #endregion
}
