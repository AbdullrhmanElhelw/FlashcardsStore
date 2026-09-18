namespace FlashcardsStore.Application.Common;

public class Result
{
    public bool Succeeded { get; protected set; }
    public bool IsSuccess => Succeeded;
    public string? Error { get; protected set; }
    public List<string> Errors { get; protected set; } = new();

    public static Result Success() => new() { Succeeded = true };
    public static Result Failure(string error) => new() { Succeeded = false, Error = error, Errors = new() { error } };
    public static Result Failure(List<string> errors) => new() { Succeeded = false, Error = string.Join("; ", errors), Errors = errors };
}

public class Result<T> : Result
{
    public T? Data { get; protected set; }
    public T? Value => Data;

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static Result<T> Failure(string error) => new() { Succeeded = false, Error = error, Errors = new() { error } };
    public new static Result<T> Failure(List<string> errors) => new() { Succeeded = false, Error = string.Join("; ", errors), Errors = errors };
}
