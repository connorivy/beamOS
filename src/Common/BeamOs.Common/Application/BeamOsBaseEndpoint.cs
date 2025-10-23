using System.Runtime.CompilerServices;
using BeamOs.Common.Contracts;

[assembly: InternalsVisibleTo("BeamOs.StructuralAnalysis.Api")]

namespace BeamOs.Common.Api;

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
    internal Task<TResponse> GetResponseTypeForClientGenerationPurposes(
#pragma warning restore CA1822 // Mark members as static
    )
    {
        return Task.FromResult<TResponse>(default!);
    }

    public async Task<Result<TResponse>> ExecuteAsync(TRequest req, CancellationToken ct = default)
    {
        try
        {
            return await this.ExecuteRequestAsync(req, ct);
        }
        catch (Exception ex)
        {
            return BeamOsError.Failure(
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
    }
}

// public abstract partial class BeamOsBaseEndpoint<TRequest, TResponse>
//     : BeamOsActualBaseEndpoint<TRequest, Task<Result<TResponse>>> { }

// public abstract partial class BeamOsFromBodyBaseEndpoint<TRequest, TResponse>
//     : BeamOsActualBaseEndpoint<TRequest, TResponse> { }

public abstract partial class BeamOsFromBodyResultBaseEndpoint<TRequest, TResponse>
    : BeamOsBaseEndpoint<TRequest, TResponse> { }

public abstract partial class BeamOsModelResourceBaseEndpoint<TRequest, TResponse>
    : BeamOsBaseEndpoint<ModelResourceRequest<TRequest>, TResponse> { }

public abstract partial class BeamOsModelResourceWithIntIdBaseEndpoint<TRequest, TResponse>
    : BeamOsBaseEndpoint<ModelResourceWithIntIdRequest<TRequest>, TResponse> { }

// public abstract partial class BeamOsModelResourceWithIntIdBaseEndpoint<TBody, TResponse>
//     : BeamOsBaseEndpoint<ModelResourceWithIntIdRequest<TBody>, TResponse> { }
