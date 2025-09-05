using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

// [Generator]
// public class BeamOsRouteInMemoryGenerator : IIncrementalGenerator
// {
//     public void Initialize(IncrementalGeneratorInitializationContext context)
//     {
//         IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations =
//             context.SyntaxProvider.CreateSyntaxProvider(
//                 static (s, _) => HasBeamOsRouteAttribute(s),
//                 static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
//             );

//         IncrementalValueProvider<(
//             Compilation compilation,
//             ImmutableArray<ClassDeclarationSyntax> classes
//         )> compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());
//         Dictionary<string, InMemoryHandlerInfo> apiMethodNameToParameterTypeName = [];

//         context.RegisterSourceOutput(
//             compilationAndClasses,
//             (spc, source) =>
//             {
//                 try
//                 {
//                     Logger.Context = spc;
//                     CreateInMemoryCommandHandlers(spc, source, apiMethodNameToParameterTypeName);
//                 }
//                 catch (Exception ex)
//                 {
//                     Logger.LogException(ex);
//                 }
//             }
//         );

//         IncrementalValuesProvider<InterfaceDeclarationSyntax> interfaceDeclarations =
//             context.SyntaxProvider.CreateSyntaxProvider(
//                 static (s, _) =>
//                     s is InterfaceDeclarationSyntax ids
//                     && ids.Identifier.Text == "IStructuralAnalysisApiClientV1",
//                 static (ctx, _) => (InterfaceDeclarationSyntax)ctx.Node
//             );
//         IncrementalValueProvider<(
//             Compilation compilation,
//             ImmutableArray<InterfaceDeclarationSyntax> interfaces
//         )> compilationAndInterfaces = context.CompilationProvider.Combine(
//             interfaceDeclarations.Collect()
//         );
//         context.RegisterSourceOutput(
//             compilationAndInterfaces,
//             (spc, source) =>
//             {
//                 try
//                 {
//                     InMemoryApiClientGenerator.CreateInMemoryApiClient(
//                         spc,
//                         source,
//                         apiMethodNameToParameterTypeName
//                     );
//                 }
//                 catch (Exception ex)
//                 {
//                     Logger.LogException(ex);
//                 }
//             }
//         );
//     }

//     private static void CreateInMemoryCommandHandlers(
//         SourceProductionContext spc,
//         (Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes) source,
//         Dictionary<string, InMemoryHandlerInfo> apiMethodNameToParameterTypeName
//     )
//     {
//         // System.Diagnostics.Debugger.Launch();
//         Compilation compilation = source.compilation;
//         ImmutableArray<ClassDeclarationSyntax> classes = source.classes;

//         StringBuilder di = new();
//         di.AppendLine("using Microsoft.Extensions.DependencyInjection;");
//         di.AppendLine("using BeamOs.StructuralAnalysis.Application.InMemory;");
//         di.AppendLine($"namespace BeamOs.StructuralAnalysis.Api.Endpoints;");
//         di.AppendLine();
//         di.AppendLine($"public static class InMemoryCommandHandlerRegistration");
//         di.AppendLine("{");
//         di.AppendLine(
//             $"    public static IServiceCollection AddInMemoryCommandHandlers(this IServiceCollection services)"
//         );
//         di.AppendLine("    {");

//         foreach (ClassDeclarationSyntax classDecl in classes)
//         {
//             SemanticModel model = compilation.GetSemanticModel(classDecl.SyntaxTree);
//             if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol symbol)
//             {
//                 continue;
//             }
//             if (apiMethodNameToParameterTypeName.ContainsKey(symbol.Name))
//             {
//                 continue; // Already processed this class
//             }

//             // Check for exactly one constructor parameter (primary or otherwise)
//             var ctors = classDecl.Members.OfType<ConstructorDeclarationSyntax>().ToList();
//             int paramCount = 0;
//             ParameterSyntax? singleParam = null;
//             if (classDecl.ParameterList != null && classDecl.ParameterList.Parameters.Count == 1)
//             {
//                 paramCount = 1;
//                 singleParam = classDecl.ParameterList.Parameters[0];
//             }
//             else if (ctors.Count == 1 && ctors[0].ParameterList.Parameters.Count == 1)
//             {
//                 paramCount = 1;
//                 singleParam = ctors[0].ParameterList.Parameters[0];
//             }
//             else if (ctors.Count > 1)
//             {
//                 continue; // More than one constructor
//             }
//             if (paramCount != 1 || singleParam == null)
//             {
//                 continue;
//             }

//             // Get the type symbol for the single constructor argument
//             INamedTypeSymbol? ctorParamTypeSymbol = null;
//             if (singleParam.Type != null)
//             {
//                 ctorParamTypeSymbol = (INamedTypeSymbol)model.GetTypeInfo(singleParam.Type).Type;
//             }

//             var route = symbol
//                 .GetAttributes()
//                 .Where(a => a.AttributeClass?.Name.Contains("BeamOsRoute") ?? false)
//                 .Select(a => a.ConstructorArguments.First().Value.ToString())
//                 .First();
//             if (ctorParamTypeSymbol.TypeKind == TypeKind.Interface)
//             {
//                 if (
//                     ctorParamTypeSymbol
//                     is not { Name: "ICommandHandler", Arity: 2 }
//                         and not { Name: "IQueryHandler", Arity: 2 }
//                 )
//                 {
//                     throw new InvalidOperationException(
//                         $"Class {symbol.Name} does not implement ICommandHandler or QueryHandler."
//                     );
//                 }

//                 apiMethodNameToParameterTypeName.Add(
//                     symbol.Name,
//                     new InMemoryHandlerInfo(
//                         ctorParamTypeSymbol.TypeKind,
//                         ctorParamTypeSymbol.ToDisplayString(),
//                         ctorParamTypeSymbol.TypeArguments[0].ToDisplayString(),
//                         ctorParamTypeSymbol.TypeArguments[1].ToDisplayString(),
//                         route
//                     )
//                 );
//             }
//             else
//             {
//                 var inMemoryType = InMemoryCommandHandlerGenerator.CreateInMemoryHandlerDecorator(
//                     spc,
//                     ctorParamTypeSymbol,
//                     di,
//                     route
//                 );
//                 if (inMemoryType is not null)
//                 {
//                     apiMethodNameToParameterTypeName.Add(symbol.Name, inMemoryType);
//                 }
//             }
//         }

//         di.AppendLine("        return services; ");
//         di.AppendLine("    }");
//         di.AppendLine("}");
//         spc.AddSource("InMemoryCommandHandlerRegistration.g.cs", di.ToString());
//     }

//     private static bool HasBeamOsRouteAttribute(SyntaxNode node)
//     {
//         if (node is not ClassDeclarationSyntax cds)
//         {
//             return false;
//         }
//         return cds
//             .AttributeLists.SelectMany(a => a.Attributes)
//             .Any(attr => attr.Name.ToString().Contains("BeamOsRoute"));
//     }
// }

public class InMemoryHandlerInfo(
    TypeKind typeKind,
    string handlerTypeName,
    string handlerParameterTypeName,
    string handlerReturnTypeName,
    string route
)
{
    public TypeKind TypeKind { get; } = typeKind;
    public string HandlerTypeName { get; } = handlerTypeName;
    public string HandlerParameterTypeName { get; } = handlerParameterTypeName;
    public string HandlerReturnTypeName { get; } = handlerReturnTypeName;
    public string Route { get; } = route;
}
