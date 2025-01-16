using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts.Exceptions;

namespace BeamOs.Common.Contracts;

public sealed class Result<TValue>
{
    public TValue? Value { get; }

    public BeamOsError? Error { get; }

    private Result(TValue value)
    {
        this.IsError = false;
        this.Value = value;
    }

    private Result(BeamOsError error)
    {
        this.IsError = true;
        this.Error = error;
    }

    [JsonConstructor]
    [Obsolete("Deserialization constructor. Don't use")]
    public Result(TValue? value, BeamOsError? error, bool isError)
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

    public static implicit operator Result<TValue>(BeamOsError error) => new(error);

    //public bool IsError(out BeamOsError? error)
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
        Func<BeamOsError, TResult> failure
    ) => !this.IsError ? success(this.Value!) : failure(this.Error);

    public async Task<TResult> MatchAsync<TResult>(
        Func<TValue, TResult> success,
        Func<BeamOsError, TResult> failure
    ) => !this.IsError ? success(this.Value!) : failure(this.Error);

    public void ThrowIfError()
    {
        if (this.IsError)
        {
            throw new BeamOsException(
                $@"
Result is in error state.
Message: {this.Error.Description}
ErrorCode: {this.Error.Code}
ErrorType: {this.Error.Type}
Metadata: {this.Error.Metadata}
"
            );
        }
    }
}
