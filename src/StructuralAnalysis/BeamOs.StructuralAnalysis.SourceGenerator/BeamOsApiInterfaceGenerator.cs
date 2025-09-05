using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

[Generator]
public class BeamOsApiInterfaceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
            "BeamOs.Common.Api.BeamOsRouteAttribute",
            static (s, _) => s is ClassDeclarationSyntax,
            static (ctx, _) => (ClassDeclarationSyntax)ctx.TargetNode
        );

        var compilationAndClasses = context.CompilationProvider.Combine(
            classDeclarations.Collect()
        );

        context.RegisterSourceOutput(
            compilationAndClasses,
            (spc, source) =>
            {
                Logger.Context = spc;
                try
                {
                    CreateApiInterface(spc, source);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        );
    }

    private static void CreateApiInterface(
        SourceProductionContext spc,
        (Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes) source
    )
    {
        // System.Diagnostics.Debugger.Launch();
        Compilation compilation = source.compilation;
        ImmutableArray<ClassDeclarationSyntax> classes = source.classes;

        StringBuilder api = new();
        api.AppendLine("#nullable enable");
        api.AppendLine("using System.Threading.Tasks;");
        api.AppendLine("using BeamOs.Common.Api;");
        api.AppendLine("using BeamOs.Common.Contracts;");
        api.AppendLine();
        api.AppendLine("namespace BeamOs.StructuralAnalysis.Api;");
        api.AppendLine();
        api.AppendLine("public interface IStructuralAnalysisApiClientV2");
        api.AppendLine("{");
        var inMemoryImpl = CreateInMemoryImpl();

        FluentApiClientBuilder apiClientBuilder = new();

        foreach (ClassDeclarationSyntax classDecl in classes)
        {
            SemanticModel model = compilation.GetSemanticModel(classDecl.SyntaxTree);
            if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol symbol)
            {
                continue;
            }

            // class should inherit from BeamOsBaseEndpoint<TRequest, TResponse>
            INamedTypeSymbol? baseEndpointType = symbol.BaseType;
            while (!EqualsBeamOsBaseEndpoint(baseEndpointType) && baseEndpointType != null)
            {
                baseEndpointType = baseEndpointType.BaseType;
            }

            if (baseEndpointType == null)
            {
                throw new InvalidOperationException(
                    $"Class {symbol.Name} does not inherit from BeamOsBaseEndpoint."
                );
            }
            var requestType = baseEndpointType.TypeArguments[0].ToDisplayString();
            var returnType = baseEndpointType.TypeArguments[1].ToDisplayString();
            var commandHandlerType = symbol.Constructors.Single().Parameters.Single().Type;

            api.AppendLine(
                $"   public Task<ApiResponse<{returnType}>> {symbol.Name}({requestType} request, CancellationToken ct = default);"
            );
            AddToInMemoryImpl(
                inMemoryImpl,
                symbol,
                commandHandlerType,
                baseEndpointType.TypeArguments[0],
                baseEndpointType.TypeArguments[1]
            );
            apiClientBuilder.AddMethodAtRoute(
                symbol,
                commandHandlerType,
                baseEndpointType.TypeArguments[0],
                baseEndpointType.TypeArguments[1]
            );
        }

        api.AppendLine("}");
        inMemoryImpl.AppendLine("}");
        apiClientBuilder.AddClassesToCompilation(spc);
        spc.AddSource("IStructuralAnalysisApiClientV2.g.cs", api.ToString());
        spc.AddSource("InMemoryApiClient2.g.cs", inMemoryImpl.ToString());
    }

    private static StringBuilder CreateInMemoryImpl()
    {
        StringBuilder impl = new();
        impl.AppendLine("#nullable enable");
        impl.AppendLine("using System.Threading.Tasks;");
        impl.AppendLine("using BeamOs.Common.Api;");
        impl.AppendLine("using BeamOs.Common.Contracts;");
        impl.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        impl.AppendLine();
        impl.AppendLine("namespace BeamOs.StructuralAnalysis.Api;");
        impl.AppendLine();
        impl.AppendLine(
            "public sealed class InMemoryApiClient2(IServiceProvider serviceProvider) : IStructuralAnalysisApiClientV2"
        );
        impl.AppendLine("{");

        return impl;
    }

    private static void AddToInMemoryImpl(
        StringBuilder inMemoryImpl,
        INamedTypeSymbol symbol,
        ITypeSymbol commandHandlerType,
        ITypeSymbol requestType,
        ITypeSymbol returnType
    )
    {
        if (requestType != null && returnType != null)
        {
            inMemoryImpl.AppendLine(
                @$"
    public async Task<ApiResponse<{returnType}>> {symbol.Name}({requestType} request, CancellationToken ct = default) 
    {{
        var handler = serviceProvider.GetRequiredKeyedService<{commandHandlerType.ToDisplayString()}>(""InMemory"");
        var endpoint = new {symbol.ToDisplayString()}(handler);
        return (await endpoint.ExecuteRequestAsync(request, ct)).ToApiResponse();
    }}
"
            );
        }
    }

    private static bool EqualsBeamOsBaseEndpoint(ITypeSymbol? typeSymbol)
    {
        if (typeSymbol == null)
        {
            return false;
        }
        return typeSymbol.Name == "BeamOsBaseEndpoint"
            && typeSymbol.ContainingNamespace.ToDisplayString() == "BeamOs.Common.Api";
    }
}
