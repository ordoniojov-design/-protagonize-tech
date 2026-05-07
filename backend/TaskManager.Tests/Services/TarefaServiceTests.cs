using System.Net;
using Xunit;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Services;
using TaskManager.Tests.Helpers;

namespace TaskManager.Tests.Services;

public class TarefaServiceTests
{
    private static TarefaService CreateService(string dbName)
        => new(DbContextFactory.Create(dbName));

    private static async Task<Tarefa> SeedAsync(string dbName, string titulo = "Tarefa Teste")
    {
        using var ctx = DbContextFactory.Create(dbName);
        var tarefa = new Tarefa { Titulo = titulo, Descricao = "Desc", Status = "Pendente", Prioridade = "Média" };
        ctx.Tarefas.Add(tarefa);
        await ctx.SaveChangesAsync();
        return tarefa;
    }

    #region GetAll

    [Fact]
    public async Task GetAll_SemFiltro_RetornaTodas()
    {
        // Arrange
        var db = nameof(GetAll_SemFiltro_RetornaTodas);
        await SeedAsync(db, "T1");
        await SeedAsync(db, "T2");

        // Act
        var result = await CreateService(db).GetAllAsync(null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task GetAll_ComFiltroStatus_RetornaApenasFiltradas()
    {
        // Arrange
        var db = nameof(GetAll_ComFiltroStatus_RetornaApenasFiltradas);
        using var ctx = DbContextFactory.Create(db);
        ctx.Tarefas.AddRange(
            new Tarefa { Titulo = "A", Status = "Pendente",     Prioridade = "Média" },
            new Tarefa { Titulo = "B", Status = "Concluída",    Prioridade = "Média" },
            new Tarefa { Titulo = "C", Status = "Em Andamento", Prioridade = "Média" }
        );
        await ctx.SaveChangesAsync();

        // Act
        var result = await new TarefaService(ctx).GetAllAsync("Pendente");

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Single(result.Data!);
        Assert.All(result.Data!, t => Assert.Equal("Pendente", t.Status));
    }

    [Fact]
    public async Task GetAll_SemTarefas_RetornaListaVazia()
    {
        // Arrange
        var db = nameof(GetAll_SemTarefas_RetornaListaVazia);

        // Act
        var result = await CreateService(db).GetAllAsync(null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Empty(result.Data!);
    }

    #endregion

    #region GetById

    [Fact]
    public async Task GetById_IdExistente_RetornaTarefa()
    {
        // Arrange
        var db = nameof(GetById_IdExistente_RetornaTarefa);
        var tarefa = await SeedAsync(db);

        // Act
        var result = await CreateService(db).GetByIdAsync(tarefa.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(tarefa.Id, result.Data!.Id);
        Assert.Equal(tarefa.Titulo, result.Data.Titulo);
    }

    [Fact]
    public async Task GetById_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var db = nameof(GetById_IdInexistente_RetornaNotFound);

        // Act
        var result = await CreateService(db).GetByIdAsync(999);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Contains("999", result.Message);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_DadosValidos_RetornaCreated()
    {
        // Arrange
        var db = nameof(Create_DadosValidos_RetornaCreated);
        var dto = new CreateTarefaDto { Titulo = "Nova", Descricao = "Desc", Status = "Pendente", Prioridade = "Alta" };

        // Act
        var result = await CreateService(db).CreateAsync(dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Equal("Nova", result.Data!.Titulo);
        Assert.Equal("Alta", result.Data.Prioridade);
        Assert.True(result.Data.Id > 0);
    }

    [Fact]
    public async Task Create_PersisteDadosNoBanco()
    {
        // Arrange
        var db = nameof(Create_PersisteDadosNoBanco);
        var dto = new CreateTarefaDto { Titulo = "Persistida", Status = "Pendente", Prioridade = "Baixa" };

        // Act
        await CreateService(db).CreateAsync(dto);

        // Assert
        using var ctx = DbContextFactory.Create(db);
        Assert.Equal(1, ctx.Tarefas.Count());
        Assert.Equal("Persistida", ctx.Tarefas.First().Titulo);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_IdExistente_AtualizaCampos()
    {
        // Arrange
        var db = nameof(Update_IdExistente_AtualizaCampos);
        var tarefa = await SeedAsync(db);
        var dto = new UpdateTarefaDto { Titulo = "Atualizada", Descricao = "Nova desc", Status = "Concluída", Prioridade = "Alta" };

        // Act
        var result = await CreateService(db).UpdateAsync(tarefa.Id, dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Atualizada", result.Data!.Titulo);
        Assert.Equal("Concluída", result.Data.Status);
        Assert.NotNull(result.Data.DataAtualizacao);
    }

    [Fact]
    public async Task Update_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var db = nameof(Update_IdInexistente_RetornaNotFound);
        var dto = new UpdateTarefaDto { Titulo = "X", Status = "Pendente", Prioridade = "Média" };

        // Act
        var result = await CreateService(db).UpdateAsync(999, dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region UpdateStatus

    [Fact]
    public async Task UpdateStatus_IdExistente_AtualizaStatus()
    {
        // Arrange
        var db = nameof(UpdateStatus_IdExistente_AtualizaStatus);
        var tarefa = await SeedAsync(db);
        var dto = new UpdateStatusDto { Status = "Em Andamento" };

        // Act
        var result = await CreateService(db).UpdateStatusAsync(tarefa.Id, dto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Em Andamento", result.Data!.Status);
        Assert.NotNull(result.Data.DataAtualizacao);
    }

    [Fact]
    public async Task UpdateStatus_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var db = nameof(UpdateStatus_IdInexistente_RetornaNotFound);
        var dto = new UpdateStatusDto { Status = "Concluída" };

        // Act
        var result = await CreateService(db).UpdateStatusAsync(999, dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_IdExistente_RemoveDosBanco()
    {
        // Arrange
        var db = nameof(Delete_IdExistente_RemoveDosBanco);
        var tarefa = await SeedAsync(db);

        // Act
        var result = await CreateService(db).DeleteAsync(tarefa.Id);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        using var ctx = DbContextFactory.Create(db);
        Assert.Empty(ctx.Tarefas);
    }

    [Fact]
    public async Task Delete_IdInexistente_RetornaNotFound()
    {
        // Arrange
        var db = nameof(Delete_IdInexistente_RetornaNotFound);

        // Act
        var result = await CreateService(db).DeleteAsync(999);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    #endregion
}
