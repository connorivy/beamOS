using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

// [Generator]
public class InMemoryCommandHandlerGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations =
            context.SyntaxProvider.CreateSyntaxProvider(
                static (s, _) => IsClassDeclaration(s),
                static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
            );

        IncrementalValueProvider<(
            Compilation compilation,
            ImmutableArray<ClassDeclarationSyntax> classes
        )> compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());
        Dictionary<string, InMemoryHandlerInfo> apiMethodNameToParameterTypeName = new();

        context.RegisterSourceOutput(
            compilationAndClasses,
            (spc, source) =>
            {
                Compilation compilation = source.compilation;
                ImmutableArray<ClassDeclarationSyntax> classes = source.classes;

                foreach (ClassDeclarationSyntax classDecl in classes)
                {
                    SemanticModel model = compilation.GetSemanticModel(classDecl.SyntaxTree);
                    if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol symbol)
                    {
                        continue;
                    }

                    CreateInMemoryHandlerDecorator(spc, symbol);
                }
            }
        );

        IncrementalValuesProvider<InterfaceDeclarationSyntax> interfaceDeclarations =
            context.SyntaxProvider.CreateSyntaxProvider(
                static (s, _) =>
                    s is InterfaceDeclarationSyntax ids
                    && ids.Identifier.Text == "IStructuralAnalysisApiClientV1",
                static (ctx, _) => (InterfaceDeclarationSyntax)ctx.Node
            );
        IncrementalValueProvider<(
            Compilation compilation,
            ImmutableArray<InterfaceDeclarationSyntax> interfaces
        )> compilationAndInterfaces = context.CompilationProvider.Combine(
            interfaceDeclarations.Collect()
        );
        context.RegisterSourceOutput(
            compilationAndInterfaces,
            (spc, source) =>
            {
                InMemoryApiClientGenerator.CreateInMemoryApiClient(
                    spc,
                    source,
                    apiMethodNameToParameterTypeName
                );
            }
        );
    }

    public static InMemoryHandlerInfo? CreateInMemoryHandlerDecorator(
        SourceProductionContext spc,
        INamedTypeSymbol symbol
    )
    {
        // Skip abstract classes
        if (symbol.IsAbstract)
        {
            return null;
        }

        // Check if class implements ICommandHandler<T1, T2> or IQueryHandler<T1, T2>
        if (
            !symbol.AllInterfaces.Any(i =>
                (i.Name == "ICommandHandler" && i is { Arity: 2 })
                || (i.Name == "IQueryHandler" && i is { Arity: 2 })
            )
        )
        {
            return null;
        }

        string newClassName = "InMemory" + symbol.Name;
        string namespaceName = symbol.ContainingNamespace.ToDisplayString();

        // Get the ICommandHandler interface and its type arguments
        INamedTypeSymbol? commandHandlerInterface = symbol.AllInterfaces.FirstOrDefault(i =>
            i.Name == "ICommandHandler" && i is { Arity: 2 }
        );
        INamedTypeSymbol? queryHandlerInterface = symbol.AllInterfaces.FirstOrDefault(i =>
            i.Name == "IQueryHandler" && i is { Arity: 2 }
        );
        INamedTypeSymbol handlerInterface =
            commandHandlerInterface
            ?? queryHandlerInterface
            ?? throw new InvalidOperationException(
                $"Class {symbol.Name} does not implement ICommandHandler<T1, T2> or QueryHandler<T1, T2>."
            );

        ImmutableArray<ITypeSymbol> typeArgs = handlerInterface.TypeArguments;

        List<string> ctorParams = new();
        List<string> handlerCtorArgs = new();

        // Use the first accessible constructor, or the parameterless one if available
        IMethodSymbol? ctor = symbol
            .Constructors.Where(c => c.DeclaredAccessibility is Accessibility.Public)
            .OrderBy(c => c.Parameters.Length)
            .FirstOrDefault();

        if (ctor != null)
        {
            foreach (IParameterSymbol param in ctor.Parameters)
            {
                string paramTypeName = param.Type.ToDisplayString();
                string paramName = param.Name;
                if (param.Type.TypeKind == TypeKind.Interface)
                {
                    ctorParams.Add(
                        $"[FromKeyedServices(\"InMemory\")] {paramTypeName} {paramName}"
                    );
                }
                else
                {
                    ctorParams.Add($"{paramTypeName} {paramName}");
                }
                handlerCtorArgs.Add(paramName);
            }
        }
        string returnType =
            "System.Threading.Tasks.Task<BeamOs.Common.Contracts.Result<"
            + typeArgs[1].ToDisplayString()
            + ">>";

        StringBuilder sb = new();
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using BeamOs.Common.Application;");
        sb.AppendLine($"using {namespaceName};");
        sb.AppendLine("namespace BeamOs.StructuralAnalysis.Application.InMemory");
        sb.AppendLine("{");
        sb.AppendLine($"    public class {newClassName}");
        sb.AppendLine("    {");
        sb.AppendLine($"        private readonly {symbol.Name} _inner;");
        sb.AppendLine($"        public {newClassName}({string.Join(", ", ctorParams)})");
        sb.AppendLine("        {");
        sb.AppendLine(
            $"            _inner = new {symbol.Name}({string.Join(", ", handlerCtorArgs)});"
        );
        sb.AppendLine("        }");
        sb.AppendLine(
            $"        public {returnType} ExecuteAsync({typeArgs[0]} command, System.Threading.CancellationToken cancellationToken = default)"
        );
        sb.AppendLine("        {");
        sb.AppendLine("            return _inner.ExecuteAsync(command, cancellationToken);");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        spc.AddSource($"{newClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        //return "global::BeamOs.StructuralAnalysis.Application.InMemory." + newClassName;

        return new InMemoryHandlerInfo(
            symbol.TypeKind,
            "global::BeamOs.StructuralAnalysis.Application.InMemory." + newClassName,
            typeArgs[0].ToDisplayString(),
            returnType
        );
    }

    private static bool IsClassDeclaration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax;
    }
}
