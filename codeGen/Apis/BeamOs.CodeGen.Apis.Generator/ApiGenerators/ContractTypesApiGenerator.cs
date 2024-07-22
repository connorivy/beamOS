using BeamOs.Contracts;

namespace BeamOs.CodeGen.Apis.Generator.ApiGenerators;

public class StructuralAnalysisContractsTypesApiGenerator : AbstractGenerator
{
    public override string ClientName => "StructuralAnalysisContracts";
    protected override string ClientNamespace => "BeamOs.CodeGen.Apis";
    protected override string DestinationPath => $"../{this.ClientNamespace}/{this.ClientName}/";
    protected override string OpenApiDefinitionPath => $"/swagger/{this.ClientName}/swagger.json";
    protected override bool GenerateCsClient => false;

    protected override void AddApiMethods(Func<string, RouteHandlerBuilder> addMethod)
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

            _ = addMethod(contractType.FullName).Produces(200, contractType);
        }
    }
}
