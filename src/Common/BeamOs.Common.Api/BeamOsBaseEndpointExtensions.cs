using System.Diagnostics;
using System.Runtime.CompilerServices;
using BeamOs.Common.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

[assembly: InternalsVisibleTo("BeamOs.StructuralAnalysis.Api")]

namespace BeamOs.Common.Api;

internal static class BeamOsBaseEndpointExtensions
{
    extension<TRequest, TResponse>(BeamOsBaseEndpoint<TRequest, TResponse> result)
    {
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
    }

    extension<TResponse>(Result<TResponse> result)
    {
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

#pragma warning disable IDE0060 // Remove unused parameter
        public IResult ToWebResult(CancellationToken ct = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (result.IsSuccess)
            {
                return TypedResults.Ok(result.Value);
            }
            return BeamOsErrorUtils.MapErrorToResult(result.Error);
        }
    }

#if NET10_OR_GREATER
    extension<TResponse, TEnumerable>(Result<TResponse> result)
        where TResponse : IAsyncEnumerable<TEnumerable>
    {
        public IResult ToWebResult(
            CancellationToken ct = default
        )
        {
            static async IAsyncEnumerable<System.Net.ServerSentEvents.SseItem<TEnumerable>> ToSse(
                IAsyncEnumerable<TEnumerable> result,
                [EnumeratorCancellation] CancellationToken ct
            )
            {
                await foreach (var item in result.WithCancellation(ct))
                {
                    yield return new System.Net.ServerSentEvents.SseItem<TEnumerable>(item);
                }
            }

            if (result.IsSuccess)
            {
                return TypedResults.ServerSentEvents(ToSse(result.Value, ct));
            }
            return BeamOsErrorUtils.MapErrorToResult(result.Error);
        }
    }
#endif
}
public static class BeamOsBaseEndpointMoreExtensions
{
    extension<TResponse>(Result<TResponse> result)
        where TResponse : IResult
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public IResult ToWebResult(CancellationToken ct = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (result.IsSuccess)
            {
                return TypedResults.Ok(result.Value);
            }
            return BeamOsErrorUtils.MapErrorToResult(result.Error);
        }
    }
}


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
