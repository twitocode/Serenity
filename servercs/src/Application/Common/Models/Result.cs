namespace Serenity.Application.Common.Models;

public record Result<T>
{
    public bool Success { get; set; }
    public List<ApplicationError> Errors { get; set; }
    public T Data { get; set; }
}