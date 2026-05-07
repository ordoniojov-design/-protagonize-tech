using System.Net;

namespace TaskManager.API.Services;

public class ServiceResult<T>
{
    public HttpStatusCode StatusCode { get; private init; }
    public T? Data { get; private init; }
    public string? Message { get; private init; }

    public bool IsSuccess =>
        (int)StatusCode >= 200 && (int)StatusCode < 300;

    private ServiceResult() { }

    public static ServiceResult<T> Ok(T data) => new()
    {
        StatusCode = HttpStatusCode.OK,
        Data = data
    };

    public static ServiceResult<T> Created(T data) => new()
    {
        StatusCode = HttpStatusCode.Created,
        Data = data
    };

    public static ServiceResult<T> NotFound(string message) => new()
    {
        StatusCode = HttpStatusCode.NotFound,
        Message = message
    };

    public static ServiceResult<T> BadRequest(string message) => new()
    {
        StatusCode = HttpStatusCode.BadRequest,
        Message = message
    };

    public static ServiceResult<T> InternalError(string message) => new()
    {
        StatusCode = HttpStatusCode.InternalServerError,
        Message = message
    };
}
