using BeamOs.StructuralAnalysis;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

// [assembly: DotWrap.DotWrapExternalPropertyMeta(
//     typeof(BeamOsResultApiClient),
//     nameof(BeamOsResultApiClient.Models)
// )]

// [assembly: DotWrap.DotWrapExternalPropertyMeta(
//     typeof(BeamOsApiClient),
//     nameof(BeamOsApiClient.Models)
// )]
// [assembly: DotWrap.DotWrapExternalIndexerMeta(
//     typeof(BeamOsApiResultModels)
// )]

[assembly: DotWrap.DotWrapExposeAssembly()]
[assembly: DotWrap.DotWrapExposeAssembly(typeof(ModelResponse))]
[assembly: DotWrap.DotWrapExternalTypeConfig(typeof(BeamOsApiClient))]
[assembly: DotWrap.DotWrapExternalTypeConfig(typeof(BeamOsResultApiClient))]