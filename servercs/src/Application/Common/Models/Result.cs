namespace Serenity.Application.Common.Models;

public record Result<T>
{
    public bool Success { get; set; }
    public List<ApplicationError> Errors { get; set; } = default!;
    public T Data { get; set; } = default!;
    public int StatusCode { get; set; }

    public static Result<T> ForError(int code, List<ApplicationError> errors)
    {
        return new Result<T> { Success = false, Data = default!, Errors = errors, StatusCode = code };
    }
    public static Result<T> ForSuccess(T data)
    {
        return new Result<T> { Success = true, Data = data, Errors = new List<ApplicationError> { }, StatusCode = 200 };
    }
}