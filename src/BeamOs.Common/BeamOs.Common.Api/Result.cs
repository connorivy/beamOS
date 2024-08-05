using System.Text.Json.Serialization;

namespace BeamOs.Common.Api;

public class Result
{
    [JsonConstructor]
    private Result(bool isSuccess, BeamOsError error)
    {
        if (isSuccess && error != BeamOsError.None || !isSuccess && error == BeamOsError.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        this.IsSuccess = isSuccess;
        this.Error = error;
    }

    public bool IsSuccess { get; }

    [JsonIgnore]
    public bool IsFailure => !this.IsSuccess;

    public BeamOsError Error { get; }

    public static Result Success() => new(true, BeamOsError.None);

    public static Result Failure(BeamOsError error) => new(false, error);
}

public readonly struct Result<TValue>
{
    private readonly TValue? value;
    private readonly Exception? error;

    private Result(TValue value)
    {
        this.IsError = false;
        this.value = value;
    }

    private Result(Exception error)
    {
        this.IsError = true;
        this.error = error;
    }

    public bool IsError { get; }
    public bool IsSuccess => !this.IsError;

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(Exception error) => new(error);

    public bool TryIsError(out Exception? error)
    {
        if (!this.IsError)
        {
            error = default;
            return false;
        }

        error = this.error!;
        return true;
    }

    public bool TryIsSuccess(out TValue? value)
    {
        if (!this.IsSuccess)
        {
            value = default;
            return false;
        }

        value = this.value!;
        return true;
    }

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<Exception, TResult> failure
    ) => !this.IsError ? success(this.value!) : failure(this.error!);

    public async Task<TResult> MatchAsync<TResult>(
        Func<TValue, TResult> success,
        Func<Exception, TResult> failure
    ) => !this.IsError ? success(this.value!) : failure(this.error!);
}
