using System.Runtime.CompilerServices;
using BeamOs.Common.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

[assembly: InternalsVisibleTo("BeamOs.StructuralAnalysis.Api")]

namespace BeamOs.Common.Api;

public abstract partial class BeamOsActualBaseEndpoint<TRequest, TResponse>
{
    public abstract TResponse ExecuteRequestAsync(TRequest req, CancellationToken ct = default);
}

public abstract partial class BeamOsBaseEndpoint<TRequest, TResponse>
{
    public abstract Task<Result<TResponse>> ExecuteRequestAsync(
        TRequest req,
        CancellationToken ct = default
    );

    /// <summary>
    /// Do not remove. This method says 0 usages, but that is not true. It is used instead of ExecuteAsync when
    /// generating strongly typed api clients
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
#pragma warning disable CA1822 // Mark members as static
    internal TResponse GetResponseTypeForClientGenerationPurposes(
#pragma warning restore CA1822 // Mark members as static
    )
    {
        return default!;
    }

    public async Task<IResult> ExecuteAsync(TRequest req, CancellationToken ct = default)
    {
        Result<TResponse> result;
        try
        {
            result = await this.ExecuteRequestAsync(req, ct);
            if (result.IsSuccess)
            {
                return this.MapSuccessToResult(result.Value!);
            }
        }
        catch (Exception ex)
        {
            result = BeamOsError.Failure(
                description: "An unexpected error has occurred. " + ex.Message,
                metadata: new Dictionary<string, object?>
                {
                    ["Request"] = req,
                    ["ExceptionMessage"] = ex.Message,
                    ["StackTrace"] = ex.StackTrace,
                    ["InnerExceptionMessage"] = ex.InnerException?.Message,
                    ["InnerStackTrace"] = ex.InnerException?.StackTrace,
                }
            );
        }
        return BeamOsErrorUtils.MapErrorToResult(result.Error!);
    }

    protected virtual IResult MapSuccessToResult(TResponse response) => TypedResults.Ok(response);
}

// public abstract partial class BeamOsBaseEndpoint<TRequest, TResponse>
//     : BeamOsActualBaseEndpoint<TRequest, Task<Result<TResponse>>> { }

// public abstract partial class BeamOsFromBodyBaseEndpoint<TRequest, TResponse>
//     : BeamOsActualBaseEndpoint<TRequest, TResponse> { }

public abstract partial class BeamOsFromBodyResultBaseEndpoint<TRequest, TResponse>
    : BeamOsBaseEndpoint<TRequest, TResponse> { }

public abstract partial class BeamOsModelResourceBaseEndpoint<TCommand, TBody, TResponse>
    : BeamOsBaseEndpoint<TCommand, TResponse>
    where TCommand : IModelResourceRequest<TBody>, new() { }

public abstract partial class BeamOsModelResourceWithIntIdBaseEndpoint<TCommand, TBody, TResponse>
    : BeamOsBaseEndpoint<TCommand, TResponse>
    where TCommand : IModelResourceWithIntIdRequest<TBody>, new() { }

public abstract partial class BeamOsModelResourceWithIntIdBaseEndpoint<TBody, TResponse>
    : BeamOsBaseEndpoint<ModelResourceWithIntIdRequest<TBody>, TResponse> { }

internal static class BeamOsErrorUtils
{
    internal static ProblemHttpResult MapErrorToResult(BeamOsError error) =>
        error.Type switch
        {
            ErrorType.None => throw new NotImplementedException(),
            ErrorType.Failure => TypedResults.Problem(
                title: "Internal Server Error",
                detail: error.Description,
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                extensions: error.Metadata
            ),
            ErrorType.Validation => TypedResults.Problem(
                title: "Validation Error",
                detail: error.Description,
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                extensions: error.Metadata
            ),
            ErrorType.Conflict => TypedResults.Problem(
                title: "Conflict Error",
                detail: error.Description,
                statusCode: StatusCodes.Status409Conflict,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                extensions: error.Metadata
            ),
            ErrorType.NotFound => TypedResults.Problem(
                title: "Not Found Error",
                detail: error.Description,
                statusCode: StatusCodes.Status404NotFound,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                extensions: error.Metadata
            ),
            ErrorType.Unauthorized => TypedResults.Problem(
                title: "Unauthorized Error",
                detail: error.Description,
                statusCode: StatusCodes.Status401Unauthorized,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.2",
                extensions: error.Metadata
            ),
            ErrorType.Forbidden => TypedResults.Problem(
                title: "Forbidden Error",
                detail: error.Description,
                statusCode: StatusCodes.Status403Forbidden,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                extensions: error.Metadata
            ),
            ErrorType.InvalidOperation => TypedResults.Problem(
                title: "Invalid Operation Error",
                detail: error.Description,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                type: "https://tools.ietf.org/html/rfc4918#section-11.2",
                extensions: error.Metadata
            ),
            _ => throw new NotImplementedException(),
        };
}
