using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.OpenSees;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;

// [BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/optimize-sections")]
// [BeamOsEndpointType(Http.Post)]
// [BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
// internal class OptimizeSections() : BeamOsBaseEndpoint<ModelIdRequest, AnalyticalResultsResponse>
// {
//     public override async Task<Result<AnalyticalResultsResponse>> ExecuteRequestAsync(
//         ModelIdRequest req,
//         CancellationToken ct = default
//     ) => default;
// }
