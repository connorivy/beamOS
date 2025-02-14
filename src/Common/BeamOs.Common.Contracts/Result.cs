using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts.Exceptions;

namespace BeamOs.Common.Contracts;

public class Result
{
    public virtual BeamOsError? Error { get; }

    protected Result()
    {
        this.IsError = false;
    }

    protected Result(BeamOsError error)
    {
        this.IsError = true;
        this.Error = error;
    }

    [JsonConstructor]
    [Obsolete("Deserialization constructor. Don't use")]
    public Result(BeamOsError? error, bool isError)
    {
        this.Error = error;
        this.IsError = isError;
    }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(Error))]
#endif
    public virtual bool IsError { get; }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(false, nameof(Error))]
#endif

    [JsonIgnore]
    public virtual bool IsSuccess => !this.IsError;

    public static implicit operator Result(BeamOsError error) => new(error);

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

    public static Result Success { get; } = new();
}

public sealed class Result<TValue> : Result
{
    public TValue? Value { get; }

    public override BeamOsError? Error => base.Error;

    private Result(TValue value)
        : base()
    {
        this.Value = value;
    }

    private Result(BeamOsError error)
        : base(error) { }

    [JsonConstructor]
    [Obsolete("Deserialization constructor. Don't use")]
    public Result(TValue? value, BeamOsError? error, bool isError)
        : base(error, isError)
    {
        this.Value = value;
    }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
#endif
    public override bool IsError => base.IsError;

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
#endif

    [JsonIgnore]
    public override bool IsSuccess => !this.IsError;

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(BeamOsError error) => new(error);

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<BeamOsError, TResult> failure
    ) => !this.IsError ? success(this.Value!) : failure(this.Error);

    public async Task<TResult> MatchAsync<TResult>(
        Func<TValue, TResult> success,
        Func<BeamOsError, TResult> failure
    ) => !this.IsError ? success(this.Value!) : failure(this.Error);
}
