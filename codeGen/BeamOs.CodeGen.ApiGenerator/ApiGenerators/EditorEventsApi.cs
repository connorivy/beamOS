using BeamOs.WebApp.EditorCommands;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public class EditorEventsApi : AbstractGenerator
{
    public override string ClientName => "EditorEventsApi";
    protected override string ClientNamespace => "BeamOs.CodeGen.EditorApi";
    protected override string DestinationPath => $"../{this.ClientNamespace}/";
    protected override string OpenApiDefinitionPath => $"/swagger/{this.ClientName}/swagger.json";

    protected override void AddApiMethods(Func<string, RouteHandlerBuilder> addMethod)
    {
        foreach (
            Type contractType in typeof(IAssemblyMarkerClientCommands)
                .Assembly
                .ExportedTypes
                .Where(
                    t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(typeof(IEditorCommand))
                )
        )
        {
            _ = addMethod($"Dispatch{contractType.Name}").Accepts(contractType);
        }
    }
}
