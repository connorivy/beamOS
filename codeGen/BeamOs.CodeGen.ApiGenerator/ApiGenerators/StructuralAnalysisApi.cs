using BeamOs.Ai;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public class StructuralAnalysisApi
    : AbstractGeneratorFromEndpoints<BeamOs.StructuralAnalysis.Api.IStructuralAnalysisApiClientV2>
{
    public override string ClientName => "StructuralAnalysisApiClientV1";
    protected override string ClientNamespace => "BeamOs.CodeGen.StructuralAnalysisApiClient";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => "openapi/v1.json";

    protected override void MapEndpoints(WebApplication app) =>
        app.MapGroup("api").MapStructuralEndpoints();
}

public class AiApi : AbstractGeneratorFromEndpoints<IAssemblyMarkerAi>
{
    public override string ClientName => "AiApiClient";
    protected override string ClientNamespace => "BeamOs.CodeGen.AiApiClient";
    protected override string DestinationPath => "../BeamOs.CodeGen.StructuralAnalysisApiClient/";
    protected override string OpenApiDefinitionPath => "openapi/v1.json";
    protected override bool GenerateTsClient => false;
}
