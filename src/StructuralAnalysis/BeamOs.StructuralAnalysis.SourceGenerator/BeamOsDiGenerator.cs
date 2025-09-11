using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

/// <summary>
/// Generates extension methods for IServiceCollection to register services for dependency injection.
/// </summary>
[Generator]
public class BeamOsDiGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Generate dependency registration for ICommandHandler<,>, IQueryHandler<,>, and BeamOsBaseEndpoint<,>
        var classDeclarations = context
            .SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
            )
            .Where(cls => cls != null);

        var compilationAndClasses = context.CompilationProvider.Combine(
            classDeclarations.Collect()
        );

        context.RegisterSourceOutput(
            compilationAndClasses,
            (spc, tuple) =>
            {
                var (compilation, classes) = tuple;
                // var toRegister = new List<(string implType, string serviceType)>();
                var toRegister = new List<string>();

                foreach (var cls in classes)
                {
                    var model = compilation.GetSemanticModel(cls.SyntaxTree);
                    if (model.GetDeclaredSymbol(cls) is not ITypeSymbol symbol || symbol.IsAbstract)
                    {
                        continue;
                    }

                    foreach (var iface in symbol.AllInterfaces)
                    {
                        if (
                            iface.ContainingNamespace.ToDisplayString().Contains("BeamOs")
                            && (
                                iface.Name.StartsWith("ICommandHandler")
                                || iface.Name.StartsWith("IQueryHandler")
                            )
                        )
                        {
                            // toRegister.Add((symbol.ToDisplayString(), iface.ToDisplayString()));
                            toRegister.Add(symbol.ToDisplayString());
                        }
                    }

                    // var baseType = symbol.BaseType;
                    // while (baseType != null)
                    // {
                    //     if (
                    //         baseType.Name.StartsWith("BeamOsBaseEndpoint")
                    //         && baseType.ContainingNamespace.ToDisplayString().Contains("BeamOs")
                    //     )
                    //     {
                    //         toRegister.Add(symbol.ToDisplayString());
                    //         break;
                    //     }
                    //     baseType = baseType.BaseType;
                    // }
                }

                if (toRegister.Count == 0)
                {
                    return;
                }

                var source = GenerateExtensionSource(toRegister);
                spc.AddSource("BeamOsDiRegistration.g.cs", source);
            }
        );
    }

    private static string GenerateExtensionSource(List<string> registrations)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("namespace BeamOs.StructuralAnalysis;");
        sb.AppendLine("internal static class BeamOsDiRegistrationExtensions");
        sb.AppendLine("{");
        sb.AppendLine(
            "    public static IServiceCollection AddBeamOsServices(this IServiceCollection services)"
        );
        sb.AppendLine("    {");
        foreach (var registration in registrations)
        {
            sb.AppendLine($"        services.AddScoped<{registration}>();");
        }
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }
}
