using BeamOs.StructuralAnalysis.Api;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public class StructuralAnalysisApi
    : AbstractGeneratorFromApiProject<IAssemblyMarkerStructuralAnalysisApi>
{
    public override string ClientName => "StructuralAnalysisApiClientV1";
    protected override string ClientNamespace => "BeamOs.CodeGen.StructuralAnalysisApiClient";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => "openapi/v1.json";
}

//public class StructuralAnalysisApi : KiotaGeneratorBase<IAssemblyMarkerStructuralAnalysisApi>
//{
//    public override string ClientName => "StructuralAnalysisApiClientV1";
//    protected override string ClientNamespace => "BeamOs.CodeGen.StructuralAnalysisApiClient";
//    protected override string DestinationPath => $"../{this.ClientNamespace}/";
//    protected override string OpenApiDefinitionPath => "openapi/v1.json";
//}
