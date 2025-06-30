using BeamOs.Common.Api;
using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Api.Endpoints;

public abstract partial class BeamOsEmptyRequestBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<EmptyRequest, TResponse> { }

public abstract partial class BeamOsModelIdRequestBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<ModelResourceRequest, TResponse> { }

public abstract partial class BeamOsModelResourceQueryBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<ModelResourceWithIntIdRequest, TResponse> { }

public abstract partial class BeamOsAnalyticalResultResourceQueryBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<GetAnalyticalResultResourceQuery, TResponse> { }
