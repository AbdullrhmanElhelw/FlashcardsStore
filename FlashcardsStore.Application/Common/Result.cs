namespace FlashcardsStore.Application.Common;

public class Result<T>
{
    public bool Succeeded { get; protected set; }
    public T? Data { get; protected set; }
    public string? Error { get; protected set; }
    public List<string> Errors { get; protected set; } = new();

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
    public static Result<T> Failure(string error) => new() { Succeeded = false, Error = error, Errors = new() { error } };
    public static Result<T> Failure(List<string> errors) => new() { Succeeded = false, Error = string.Join("; ", errors), Errors = errors };
}
