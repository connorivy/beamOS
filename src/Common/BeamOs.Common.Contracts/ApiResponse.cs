using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts.Exceptions;

namespace BeamOs.Common.Contracts;

public class ApiResponse
{
    public virtual ProblemDetails? Error { get; }

    protected ApiResponse()
    {
        this.IsError = false;
    }

    protected ApiResponse(ProblemDetails error)
    {
        this.IsError = true;
        this.Error = error;
    }

    [JsonConstructor]
    [Obsolete("Deserialization constructor. Don't use")]
    public ApiResponse(ProblemDetails? error, bool isError)
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

    public static implicit operator ApiResponse(ProblemDetails error) => new(error);

    public static implicit operator Result(ApiResponse apiResponse)
    {
        if (apiResponse.IsError)
        {
            return apiResponse.Error.ToBeamOsError();
        }
        return ApiResponse.Success;
    }

    public void ThrowIfError()
    {
        if (this.IsError)
        {
            throw new BeamOsException(
                $@"
ApiResponse is in error state.
Title: {this.Error.Title}
Detail: {this.Error.Detail}
Status: {this.Error.Status}
Type: {this.Error.Type}
Instance: {this.Error.Instance}
Extensions: {this.Error.Extensions}
"
            );
        }
    }

    public static ApiResponse Success { get; } = new();

    public static ApiResponse<TValue> FromValue<TValue>(TValue value) => new(value);
}

public sealed class ApiResponse<TValue> : ApiResponse
{
    public TValue? Value { get; }

    public override ProblemDetails? Error => base.Error;

    public ApiResponse(TValue value)
        : base()
    {
        this.Value = value;
    }

    public ApiResponse(ProblemDetails error)
        : base(error) { }

    [JsonConstructor]
    [Obsolete("Deserialization constructor. Don't use")]
    public ApiResponse(TValue? value, ProblemDetails? error, bool isError)
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

    public static implicit operator ApiResponse<TValue>(TValue value) => new(value);

    public static implicit operator ApiResponse<TValue>(ProblemDetails error) => new(error);

    public static implicit operator Result<TValue>(ApiResponse<TValue> apiResponse)
    {
        if (apiResponse.IsError)
        {
            return apiResponse.Error.ToBeamOsError();
        }
        return apiResponse.Value;
    }

    public TApiResponse Match<TApiResponse>(
        Func<TValue, TApiResponse> success,
        Func<ProblemDetails, TApiResponse> failure
    ) => !this.IsError ? success(this.Value!) : failure(this.Error);

    public async Task<TApiResponse> MatchAsync<TApiResponse>(
        Func<TValue, TApiResponse> success,
        Func<ProblemDetails, TApiResponse> failure
    ) => !this.IsError ? success(this.Value!) : failure(this.Error);

    public static ApiResponse<TValue> Success() => new(default(TValue), null, false);
}

public record ProblemDetails(
    string Title,
    string Detail,
    int Status,
    string Type,
    string? Instance,
    IDictionary<string, object?>? Extensions
)
{
    public BeamOsError ToBeamOsError()
    {
        return Status switch
        {
            400 => BeamOsError.Validation(description: Detail, metadata: this.Extensions),
            401 => BeamOsError.Unauthorized(description: Detail, metadata: this.Extensions),
            403 => BeamOsError.Forbidden(description: Detail, metadata: this.Extensions),
            404 => BeamOsError.NotFound(description: Detail, metadata: this.Extensions),
            409 => BeamOsError.Conflict(description: Detail, metadata: this.Extensions),
            500 => BeamOsError.Failure(description: Detail, metadata: this.Extensions),
            _ => throw new InvalidOperationException(
                $"Cannot convert ProblemDetails with status {Status} to BeamOsError."
            ),
        };
    }
}
