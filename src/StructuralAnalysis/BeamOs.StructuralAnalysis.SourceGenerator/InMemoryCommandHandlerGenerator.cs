using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

[Generator]
public class InMemoryCommandHandlerGenerator : IIncrementalGenerator
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

        context.RegisterSourceOutput(
            compilationAndClasses,
            static (spc, source) =>
            {
                // System.Diagnostics.Debugger.Launch();

                Compilation compilation = source.compilation;
                ImmutableArray<ClassDeclarationSyntax> classes = source.classes;
                foreach (ClassDeclarationSyntax classDecl in classes)
                {
                    SemanticModel model = compilation.GetSemanticModel(classDecl.SyntaxTree);
                    if (model.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol symbol)
                    {
                        continue;
                    }

                    // Skip abstract classes
                    if (symbol.IsAbstract)
                    {
                        continue;
                    }

                    // Check if class implements ICommandHandler<T1, T2>
                    if (
                        !symbol.AllInterfaces.Any(i =>
                            i.Name == "ICommandHandler" && i is { Arity: 2 }
                        )
                    )
                    {
                        continue;
                    }

                    string newClassName = "InMemory" + symbol.Name;
                    string namespaceName = symbol.ContainingNamespace.ToDisplayString();

                    // Get the ICommandHandler interface and its type arguments
                    INamedTypeSymbol handlerInterface = symbol.AllInterfaces.First(i =>
                        i.Name == "ICommandHandler" && i is { Arity: 2 }
                    );
                    ImmutableArray<ITypeSymbol> typeArgs = handlerInterface.TypeArguments;
                    string interfaceType = $"ICommandHandler<{typeArgs[0]}, {typeArgs[1]}>";

                    List<string> ctorParams = new();
                    List<string> handlerCtorArgs = new();
                    string baseCtorArgs = string.Empty;
                    ConstructorDeclarationSyntax? ctor = classDecl
                        .Members.OfType<ConstructorDeclarationSyntax>()
                        .FirstOrDefault();
                    if (
                        classDecl.ParameterList != null
                        && classDecl.ParameterList.Parameters.Count > 0
                    )
                    {
                        foreach (ParameterSyntax param in classDecl.ParameterList.Parameters)
                        {
                            ITypeSymbol? paramType = model.GetTypeInfo(param.Type!).Type;
                            string? paramTypeName = paramType?.ToDisplayString();
                            string paramName = param.Identifier.Text;
                            if (
                                paramTypeName != null
                                && (
                                    paramTypeName.EndsWith("Repository")
                                    || paramTypeName.EndsWith("IStructuralAnalysisUnitOfWork")
                                )
                            )
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
                    else if (ctor != null)
                    {
                        foreach (ParameterSyntax param in ctor.ParameterList.Parameters)
                        {
                            ITypeSymbol? paramType = model.GetTypeInfo(param.Type!).Type;
                            string? paramTypeName = paramType?.ToDisplayString();
                            string paramName = param.Identifier.Text;
                            if (
                                paramTypeName != null
                                && (
                                    paramTypeName.EndsWith("Repository")
                                    || paramTypeName.EndsWith("IStructuralAnalysisUnitOfWork")
                                )
                            )
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

                    StringBuilder sb = new();
                    sb.AppendLine($"using Microsoft.Extensions.DependencyInjection;");
                    sb.AppendLine($"using BeamOs.Common.Application;");
                    sb.AppendLine($"namespace {namespaceName}");
                    sb.AppendLine("{");
                    // sb.AppendLine($"    public class {newClassName} : {interfaceType}");
                    sb.AppendLine($"    public class {newClassName}");
                    sb.AppendLine("    {");
                    sb.AppendLine($"        private readonly {symbol.Name} _inner;");
                    sb.AppendLine(
                        $"        public {newClassName}({string.Join(", ", ctorParams)})"
                    );
                    sb.AppendLine("        {");
                    sb.AppendLine(
                        $"            _inner = new {symbol.Name}({string.Join(", ", handlerCtorArgs)});"
                    );
                    sb.AppendLine("        }");
                    sb.AppendLine(
                        $"        public System.Threading.Tasks.Task<{typeArgs[1]}> ExecuteAsync({typeArgs[0]} command, System.Threading.CancellationToken cancellationToken = default)"
                    );
                    sb.AppendLine("        {");
                    sb.AppendLine(
                        "            return _inner.ExecuteAsync(command, cancellationToken);"
                    );
                    sb.AppendLine("        }");
                    sb.AppendLine("    }");
                    sb.AppendLine("}");

                    spc.AddSource(
                        $"{newClassName}.g.cs",
                        SourceText.From(sb.ToString(), Encoding.UTF8)
                    );
                }
            }
        );
    }

    private static bool IsClassDeclaration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax;
    }
}
