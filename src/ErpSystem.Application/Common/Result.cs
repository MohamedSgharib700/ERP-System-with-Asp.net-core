namespace ErpSystem.Application.Common;

public class Result<T>
{
    public bool Succeeded { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = new();

    public static Result<T> Success(T data, string? message = null) => new() { Succeeded = true, Data = data, Message = message };
    public static Result<T> Failure(string message, List<string>? errors = null) => new() { Succeeded = false, Message = message, Errors = errors ?? new() };
}

public class Result
{
    public bool Succeeded { get; init; }
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = new();

    public static Result Success(string? message = null) => new() { Succeeded = true, Message = message };
    public static Result Failure(string message, List<string>? errors = null) => new() { Succeeded = false, Message = message, Errors = errors ?? new() };
}
