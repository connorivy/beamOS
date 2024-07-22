using BeamOs.Api;
using BeamOs.Api.Common.Interfaces;
using BeamOs.CodeGen.Apis.Generator.Extensions;

namespace BeamOs.CodeGen.Apis.Generator.ApiGenerators;

public class StructuralAnalysisApi : AbstractGenerator
{
    public override string ClientName => "StructuralAnalysisApiAlphaClient";
    protected override string ClientNamespace => "BeamOs.CodeGen.Apis.StructuralAnalysisApi";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => $"/swagger/{this.ClientName}/swagger.json";

    protected override void AddApiMethods(Func<string, RouteHandlerBuilder> addMethod)
    {
        foreach (
            Type endpointType in typeof(IAssemblyMarkerApi)
                .Assembly
                .ExportedTypes
                .Where(t => !t.IsInterface && !t.IsAbstract)
        )
        {
            if (
                !endpointType.TryGetImplementedGenericType(
                    typeof(IBeamOsEndpoint<,>),
                    out Type implementedType
                )
            )
            {
                continue;
            }

            var genericArgs = implementedType.GetGenericArguments();
            _ = addMethod(endpointType.Name).Accepts(genericArgs[0]).Produces(200, genericArgs[1]);
        }
    }
}
