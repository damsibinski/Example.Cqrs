namespace Example.Cqrs.Application.Common;

public sealed class Result
{
    private Result(Error? error)
    {
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public bool IsFailure => Error is not null;

    public Error? Error { get; }

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(Error error) => Failure(error);
}

public sealed class Result<T>
{
    private readonly T? _value;

    private Result(T value)
    {
        _value = value;
        Error = null;
    }

    private Result(Error error)
    {
        _value = default;
        Error = error;
    }

    public bool IsSuccess => Error is null;

    public bool IsFailure => Error is not null;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot read value from a failed result.");

    public Error? Error { get; }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(Error error) => Failure(error);
}
