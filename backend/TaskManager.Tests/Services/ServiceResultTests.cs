using System.Net;
using Xunit;
using TaskManager.API.Services;

namespace TaskManager.Tests.Services;

public class ServiceResultTests
{
    [Fact]
    public void Ok_IsSuccess_True()
    {
        // Arrange & Act
        var result = ServiceResult<string>.Ok("dado");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("dado", result.Data);
    }

    [Fact]
    public void Created_IsSuccess_True()
    {
        // Arrange & Act
        var result = ServiceResult<string>.Created("criado");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
    }

    [Fact]
    public void NotFound_IsSuccess_False()
    {
        // Arrange & Act
        var result = ServiceResult<string>.NotFound("não encontrado");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("não encontrado", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public void BadRequest_IsSuccess_False()
    {
        // Arrange & Act
        var result = ServiceResult<string>.BadRequest("inválido");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public void InternalError_IsSuccess_False()
    {
        // Arrange & Act
        var result = ServiceResult<string>.InternalError("erro interno");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }
}
