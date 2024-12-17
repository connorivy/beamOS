//using BeamOs.CodeGen.ApiGenerator.ApiGenerators;
//using BeamOs.WebApp.Client.EditorCommands;
//using BeamOs.WebApp.Client.Events.Interfaces;

//namespace BeamOs.CodeGen.Apis.Generator.ApiGenerators;

//public class EditorEventsApi : AbstractGenerator
//{
//    public override string ClientName => "EditorEventsApi";
//    protected override string ClientNamespace => "BeamOs.CodeGen.Apis.EditorApi";
//    protected override string DestinationPath => $"../{this.ClientNamespace}/";
//    protected override string OpenApiDefinitionPath => $"/swagger/{this.ClientName}/swagger.json";

//    protected override void AddApiMethods(Func<string, RouteHandlerBuilder> addMethod)
//    {
//        foreach (
//            Type contractType in typeof(IAssemblyMarkerClientActions).Assembly.ExportedTypes.Where(
//                t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(typeof(IEditorCommand))
//            )
//        )
//        {
//            _ = addMethod($"Dispatch{contractType.Name}").Accepts(contractType);
//        }
//    }
//}
