using BeamOs.Common.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

namespace BeamOs.StructuralAnalysis.Api.Endpoints;

public abstract partial class BeamOsEmptyRequestBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<EmptyRequest, TResponse> { }

public abstract partial class BeamOsModelIdRequestBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<ModelIdRequest, TResponse> { }

public abstract partial class BeamOsModelResourceQueryBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<ModelEntityRequest, TResponse> { }

public abstract partial class BeamOsAnalyticalResultResourceQueryBaseEndpoint<TResponse>
    : BeamOsBaseEndpoint<GetAnalyticalResultResourceQuery, TResponse> { }
