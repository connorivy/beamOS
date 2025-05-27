using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

// [Generator]
public class InMemoryApiClientGenerator
{
    // public void Initialize(IncrementalGeneratorInitializationContext context)
    // {
    //     IncrementalValuesProvider<InterfaceDeclarationSyntax> interfaceDeclarations =
    //         context.SyntaxProvider.CreateSyntaxProvider(
    //             static (s, _) =>
    //                 s is InterfaceDeclarationSyntax ids
    //                 && ids.Identifier.Text == "IStructuralAnalysisApiClientV1",
    //             static (ctx, _) => (InterfaceDeclarationSyntax)ctx.Node
    //         );
    //     IncrementalValueProvider<(
    //         Compilation compilation,
    //         ImmutableArray<InterfaceDeclarationSyntax> interfaces
    //     )> compilationAndInterfaces = context.CompilationProvider.Combine(
    //         interfaceDeclarations.Collect()
    //     );
    //     context.RegisterSourceOutput(
    //         compilationAndInterfaces,
    //         static (spc, source) =>
    //         {
    //             bool flowControl = NewMethod(spc, source);
    //             if (!flowControl)
    //             {
    //                 return;
    //             }
    //         }
    //     );
    // }

    public static bool CreateInMemoryApiClient(
        SourceProductionContext spc,
        (Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> interfaces) source,
        // Dictionary<string, ITypeSymbol> commandHandlerNameToParameterDisplayNameMap,
        Dictionary<string, InMemoryHandlerInfo> apiMethodNameToParameterTypeName
    )
    {
        Compilation compilation = source.compilation;
        const string interfaceMetadataName =
            "BeamOs.CodeGen.StructuralAnalysisApiClient.IStructuralAnalysisApiClientV1";
        INamedTypeSymbol? interfaceSymbol = compilation.GetTypeByMetadataName(
            interfaceMetadataName
        );
        if (interfaceSymbol == null)
        {
            return false;
        }
        string namespaceName = interfaceSymbol.ContainingNamespace.ToDisplayString();
        string className = "InMemoryApiClient";
        IEnumerable<IMethodSymbol> methods = interfaceSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary);
        StringBuilder handlerFields = new StringBuilder();
        StringBuilder ctorParams = new StringBuilder();
        StringBuilder ctorAssignments = new StringBuilder();
        StringBuilder methodImpls = new StringBuilder();
        StringBuilder partialMethods = new StringBuilder();
        Dictionary<string, string> partialMethodSignatures = new Dictionary<string, string>();
        int partialMethodIndex = 1;
        bool firstParam = true;
        foreach (IMethodSymbol method in methods)
        {
            string methodName = method.Name;
            var orginalMethodName = method.Name.Replace("Async", string.Empty);
            var inMemoryTypeInfo = apiMethodNameToParameterTypeName[orginalMethodName];

            // string originalHandlerName;

            // var commandHandlerName = orginalMethodName + "CommandHandler";
            // var queryHandlerName = orginalMethodName + "QueryHandler";
            // if (handlerNames.Contains(commandHandlerName))
            // {
            //     originalHandlerName = commandHandlerName;
            // }
            // else if (handlerNames.Contains(queryHandlerName))
            // {
            //     originalHandlerName = queryHandlerName;
            // }
            // else
            // {
            //     originalHandlerName = "Could not find handler for " + orginalMethodName;
            // }

            // string handlerName = "InMemory" + originalHandlerName;


            string handlerName = orginalMethodName + "InMemoryHandler";
            string handlerField =
                $"private readonly {inMemoryTypeInfo.HandlerTypeName} _{ToCamelCase(handlerName)};";
            handlerFields.AppendLine("        " + handlerField);
            if (!firstParam)
            {
                ctorParams.Append(", ");
            }
            if (inMemoryTypeInfo.TypeKind == TypeKind.Interface)
            {
                ctorParams.Append($"[FromKeyedServices(\"InMemory\")] ");
            }
            ctorParams.Append(inMemoryTypeInfo.HandlerTypeName + " " + ToCamelCase(handlerName));
            ctorAssignments.AppendLine(
                $"            _{ToCamelCase(handlerName)} = {ToCamelCase(handlerName)};"
            );
            firstParam = false;
            string returnType = method.ReturnType.ToDisplayString();
            string paramList = string.Join(
                ", ",
                method.Parameters.Select(p =>
                    p.Type.ToDisplayString()
                    + " "
                    + p.Name
                    + (p.HasExplicitDefaultValue ? " = default" : string.Empty)
                )
            );
            string paramNames = string.Join(", ", method.Parameters.Select(p => p.Name));
            string cancellationTokenParam =
                method.Parameters.FirstOrDefault(p => p.Type.Name == "CancellationToken")?.Name
                ?? "cancellationToken";

            string handlerCommandType = inMemoryTypeInfo.HandlerParameterTypeName;

            string partialMethodParams = string.Join(
                ", ",
                method.Parameters.Select(p => p.Type.ToDisplayString() + " " + p.Name)
            );
            string signatureKey = handlerCommandType + "|" + partialMethodParams;
            if (!partialMethodSignatures.TryGetValue(signatureKey, out string? partialMethodName))
            {
                partialMethodName = "CreateCommand" + partialMethodIndex++;
                partialMethodSignatures[signatureKey] = partialMethodName;
                partialMethods.AppendLine(
                    $"        private partial {handlerCommandType} {partialMethodName}({partialMethodParams});"
                );
            }
            methodImpls.AppendLine(
                $@"
        public async {returnType} {methodName}({paramList})
        {{
            var command = {partialMethodName}({paramNames});
            return await _{ToCamelCase(handlerName)}.ExecuteAsync(command, {cancellationTokenParam});
        }}
"
            );
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using BeamOs.StructuralAnalysis.Application.InMemory;");
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();
        sb.AppendLine($"public partial class {className}");
        sb.AppendLine("{");
        sb.Append(handlerFields);
        sb.AppendLine();
        sb.AppendLine($"    public {className}({ctorParams})");
        sb.AppendLine("    {");
        sb.Append(ctorAssignments);
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.Append(partialMethods);
        sb.AppendLine();
        sb.Append(methodImpls);
        sb.AppendLine("}");
        spc.AddSource($"{className}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        return true;
    }

    private static string ToCamelCase(string s)
    {
        if (string.IsNullOrEmpty(s) || char.IsLower(s[0]))
        {
            return s;
        }
        return char.ToLowerInvariant(s[0]) + s.Substring(1);
    }
}
