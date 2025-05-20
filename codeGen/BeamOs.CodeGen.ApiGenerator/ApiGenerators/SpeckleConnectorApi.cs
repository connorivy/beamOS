using BeamOs.SpeckleConnector;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public class SpeckleConnectorApi : AbstractGeneratorFromEndpoints<IAssemblyMarkerSpeckleConnector>
{
    public override string ClientName => "SpeckleConnectorApi";
    protected override string ClientNamespace => "BeamOs.CodeGen.SpeckleConnectorApi";

    //protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string DestinationPath => $"../BeamOs.CodeGen.StructuralAnalysisApiClient/";
    protected override string OpenApiDefinitionPath => "openapi/v1.json";
}
