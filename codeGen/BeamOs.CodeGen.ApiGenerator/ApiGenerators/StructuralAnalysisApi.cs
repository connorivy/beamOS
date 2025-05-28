using BeamOs.Ai;
using BeamOs.StructuralAnalysis.Api.Endpoints;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public class StructuralAnalysisApi
    : AbstractGeneratorFromEndpoints<IAssemblyMarkerStructuralAnalysisApiEndpoints>
{
    public override string ClientName => "StructuralAnalysisApiClientV1";
    protected override string ClientNamespace => "BeamOs.CodeGen.StructuralAnalysisApiClient";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => "openapi/v1.json";
}

public class AiApi : AbstractGeneratorFromEndpoints<IAssemblyMarkerAi>
{
    public override string ClientName => "AiApiClient";
    protected override string ClientNamespace => "BeamOs.CodeGen.AiApiClient";
    protected override string DestinationPath => "../BeamOs.CodeGen.StructuralAnalysisApiClient/";
    protected override string OpenApiDefinitionPath => "openapi/v1.json";
    protected override bool GenerateTsClient => false;
}
