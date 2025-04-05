using BeamOs.Common.Application;
using BeamOs.Common.Contracts;

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
}

public abstract partial class BeamOsFromBodyBaseEndpoint<TRequest, TResponse>
    : BeamOsBaseEndpoint<TRequest, TResponse> { }

public abstract partial class BeamOsModelResourceBaseEndpoint<TCommand, TBody, TResponse>
    : BeamOsBaseEndpoint<TCommand, TResponse>
    where TCommand : IModelResourceRequest<TBody>, new() { }

public abstract partial class BeamOsModelResourceWithIntIdBaseEndpoint<TCommand, TBody, TResponse>
    : BeamOsBaseEndpoint<TCommand, TResponse>
    where TCommand : IModelResourceWithIntIdRequest<TBody>, new() { }
