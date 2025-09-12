using BeamOs.Common.Api;
using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Api.Endpoints;

internal abstract partial class BeamOsEmptyRequestBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<EmptyRequest, TResponse> { }

internal abstract partial class BeamOsModelIdRequestBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<ModelResourceRequest, TResponse> { }

internal abstract partial class BeamOsModelResourceQueryBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<ModelResourceWithIntIdRequest, TResponse> { }

internal abstract partial class BeamOsAnalyticalResultQueryBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<GetAnalyticalResultQuery, TResponse> { }

internal abstract partial class BeamOsAnalyticalResultResourceQueryBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<GetAnalyticalResultResourceQuery, TResponse> { }
