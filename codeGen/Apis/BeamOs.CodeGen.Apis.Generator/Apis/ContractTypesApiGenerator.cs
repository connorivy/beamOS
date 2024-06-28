using BeamOs.Contracts;

namespace BeamOs.CodeGen.Apis.Generator.Apis;

public class StructuralAnalysisContractsTypesApiGenerator : AbstractGenerator
{
    public StructuralAnalysisContractsTypesApiGenerator(WebApplication app)
        : base(app)
    {
        foreach (
            Type contractType in typeof(IAssemblyMarkerContracts)
                .Assembly
                .ExportedTypes
                .Where(t => !t.IsInterface && !t.IsAbstract)
        )
        {
            if (contractType.FullName is null)
            {
                continue;
            }

            this.AddMethodToApi(contractType.FullName).Produces(200, contractType);
        }
    }

    public const string StructuralAnalysisContractsApiDocumentName = "StructuralAnalysisContracts";
    protected override string ClientName => StructuralAnalysisContractsApiDocumentName;
    protected override string ClientNamespace => "BeamOS.CodeGen.Apis";
    protected override string DestinationPath => $"../{this.ClientNamespace}/{this.ClientName}/";
    protected override string OpenApiDefinitionUrl =>
        $"{this.App.Configuration[$"URLS"].Split(';').First()}/swagger/{this.ClientName}/swagger.json";
    protected override bool GenerateCsClient => false;
}

public static class StructuralAnalysisContractsApiGeneratorUtils
{
    public static async Task GenerateStructuralAnalysisContractsApi(this WebApplication app)
    {
        StructuralAnalysisContractsTypesApiGenerator generator = new(app);
        await generator.GenerateClients();
    }
}
