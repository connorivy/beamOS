using BeamOs.WebApp.EditorEvents;

namespace BeamOs.CodeGen.Apis.Generator.Apis;

public class EditorEventsApi : AbstractGenerator
{
    public override string ClientName => "EditorEventsApi";
    protected override string ClientNamespace => "BeamOs.CodeGen.Apis.EditorApi";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => $"/swagger/{this.ClientName}/swagger.json";

    protected override void AddApiMethods(Func<string, RouteHandlerBuilder> addMethod)
    {
        foreach (
            Type contractType in typeof(IAssemblyMarkerEditorEvents)
                .Assembly
                .ExportedTypes
                .Where(
                    t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(typeof(IEditorAction))
                )
        )
        {
            _ = addMethod($"Handle{contractType.Name}").Accepts(contractType);
        }
    }
}
