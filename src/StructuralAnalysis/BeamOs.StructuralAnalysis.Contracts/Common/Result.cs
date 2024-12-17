using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public sealed class Result<TValue>
{
    public TValue? Value { get; }

    public Exception? Error { get; }

    private Result(TValue value)
    {
        this.IsError = false;
        this.Value = value;
    }

    private Result(Exception error)
    {
        this.IsError = true;
        this.Error = error;
    }

    [JsonConstructor]
    internal Result(TValue? value, Exception? error, bool isError)
    {
        this.Value = value;
        this.Error = error;
        this.IsError = isError;
    }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
#endif
    public bool IsError { get; }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
#endif

    [JsonIgnore]
    public bool IsSuccess => !this.IsError;

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(Exception error) => new(error);

    //public bool IsError(out Exception? error)
    //{
    //    if (!this.isError)
    //    {
    //        error = default;
    //        return false;
    //    }

    //    error = this.Error!;
    //    return true;
    //}

    //public bool IsSuccess(out TValue? value)
    //{
    //    if (!this.IsSuccessField)
    //    {
    //        value = default;
    //        return false;
    //    }

    //    value = this.Value!;
    //    return true;
    //}

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<Exception, TResult> failure
    ) => !this.IsError ? success(this.Value!) : failure(this.Error!);

    public async Task<TResult> MatchAsync<TResult>(
        Func<TValue, TResult> success,
        Func<Exception, TResult> failure
    ) => !this.IsError ? success(this.Value!) : failure(this.Error!);
}
