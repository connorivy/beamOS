using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

public abstract class FluentApiClassGeneratorBase(string apiClientName, string classNamePrefix)
{
    private readonly HashSet<string> generatedFileNames = [];
    private readonly Dictionary<string, string> fileNameMappings = [];

    /// <summary>
    /// Generates a csharp class for each member in the apiclient route tree and adds it to the compilation
    /// </summary>
    /// <param name="spc"></param>
    /// <param name="routeTree"></param>
    public void GenerateFluentApiClasses(
        SourceProductionContext spc,
        Dictionary<string, RouteSegment> routeTree
    )
    {
        if (routeTree.Count == 0)
        {
            return; // No routes to generate
        }

        RouteSegment topRouteSegment = new() { Name = apiClientName, IsParameter = false };

        // Generate the main fluent API client
        this.GenerateFluentApiClient(spc, routeTree, apiClientName);

        // Generate accessor classes for each route segment
        this.GenerateAccessorClasses(spc, routeTree, [topRouteSegment]);
    }

    private void GenerateFluentApiClient(
        SourceProductionContext spc,
        Dictionary<string, RouteSegment> routeTree,
        string apiClientName
    )
    {
        var sb = new StringBuilder();
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine("using BeamOs.StructuralAnalysis.Api;");
        sb.AppendLine();
        sb.AppendLine("namespace BeamOs.CodeGen.StructuralAnalysisApiClient;");
        sb.AppendLine();
        sb.AppendLine($"public class {apiClientName}");
        sb.AppendLine("{");
        sb.AppendLine("    protected IStructuralAnalysisApiClientV2 ProtectedClient { get; }");
        sb.AppendLine();
        sb.AppendLine($"    public {apiClientName}(IStructuralAnalysisApiClientV2 apiClient)");
        sb.AppendLine("    {");
        sb.AppendLine(
            "        ProtectedClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));"
        );
        sb.AppendLine("    }");
        sb.AppendLine();

        // Generate root level properties
        foreach (var rootSegment in routeTree.Values.Where(s => !s.IsParameter))
        {
            var className = this.GetUniqueClassName(rootSegment.Name, [apiClientName]);
            sb.AppendLine(
                $"    public {className} {ToCSharpCase(rootSegment.Name)} => new {className}(ProtectedClient);"
            );
        }

        sb.AppendLine("}");

        spc.AddSource(
            this.GetUniqueFileName($"{apiClientName}.g.cs"),
            SourceText.From(sb.ToString(), Encoding.UTF8)
        );
    }

    private void GenerateAccessorClasses(
        SourceProductionContext spc,
        Dictionary<string, RouteSegment> segments,
        RouteSegment[] previousSegments
    )
    {
        foreach (var segment in segments.Values)
        {
            if (segment.IsParameter)
            {
                // Generate indexed accessor for parameter segments
                this.GenerateNamedAccessor(spc, segment, previousSegments);
            }
            else
            {
                // Generate regular accessor for named segments
                this.GenerateNamedAccessor(spc, segment, previousSegments);
            }
        }
    }

    private void GenerateNamedAccessor(
        SourceProductionContext spc,
        RouteSegment segment,
        RouteSegment[] previousSegments
    )
    {
        // var originalClassName = segment.Name + "Accessor";
        // var className = this.GetUniqueClassName(originalClassName);
        // var className = string.Join("_", previousSegments.Select(p => p.Name).Append(segment.Name));
        var className = this.GetUniqueClassName(segment.Name, previousSegments.Select(p => p.Name));

        var sb = new StringBuilder();

        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine("using BeamOs.StructuralAnalysis.Api;");
        sb.AppendLine("using BeamOs.Common.Contracts;");
        sb.AppendLine();
        sb.AppendLine("namespace BeamOs.CodeGen.StructuralAnalysisApiClient;");
        sb.AppendLine();
        var ctorArgs = previousSegments
            .Append(segment)
            .Where(p => p.IsParameter)
            .Select(p =>
                $"{p.ParameterType ?? throw new InvalidOperationException($"Parameter type is required for parameter segment '{p.Name}'")} {p.Name}"
            )
            .Prepend($"IStructuralAnalysisApiClientV2 ProtectedClient");

        sb.AppendLine($"public class {className}({string.Join(", ", ctorArgs)})");
        sb.AppendLine("{");

        // Generate child accessors and indexers
        var childIndexers = new HashSet<string>();
        var methodCallArgs = previousSegments
            .Append(segment)
            .Where(p => p.IsParameter)
            .Select(p => p.Name)
            .Prepend("ProtectedClient")
            .ToArray();
        foreach (var child in segment.Children.Values)
        {
            var paramName = ToCSharpCase(child.Name);
            // var childClassName = className + $"_{paramName}";
            var childClassName = this.GetUniqueClassName(
                paramName,
                previousSegments.Select(p => p.Name).Append(segment.Name)
            );
            if (child.IsParameter)
            {
                // Generate indexer for parameter child
                var paramType = CleanName(
                    child.ParameterType
                        ?? throw new InvalidOperationException(
                            $"Parameter type is required for parameter segment '{child.Name}'"
                        )
                );
                var indexerSignature = $"this[{paramType}]";

                if (!childIndexers.Contains(indexerSignature))
                {
                    childIndexers.Add(indexerSignature);
                    sb.AppendLine(
                        $"    public {childClassName} this[{paramType} {paramName}] => new {childClassName}({string.Join(", ", methodCallArgs.Append(paramName))});"
                    );
                }
            }
            else
            {
                // Generate property for named child
                sb.AppendLine(
                    $"    public {childClassName} {paramName} => new {childClassName}({string.Join(", ", methodCallArgs)});"
                );
            }
        }

        // Generate methods for this segment
        foreach (var method in segment.Methods)
        {
            sb.AppendLine(this.GetMethod(method, methodCallArgs));
        }

        sb.AppendLine("}");

        // Use the original class name for filename to ensure unique naming based on original segments
        spc.AddSource(
            this.GetUniqueFileName($"{className}.g.cs"),
            SourceText.From(sb.ToString(), Encoding.UTF8)
        );

        // Recursively generate child accessors
        this.GenerateAccessorClasses(spc, segment.Children, [.. previousSegments, segment]);
    }

    // protected abstract string GetMethod(
    //     ApiMethod method,
    //     IEnumerable<string> parametersInMethodSig
    // );

    protected abstract string GetMethod(ApiMethod method, string[] ctorArgs);

    private string GetUniqueFileName(string originalName)
    {
        // Check if we already have a mapping for this filename
        if (this.fileNameMappings.TryGetValue(originalName, out var mappedName))
        {
            return mappedName;
        }

        // Clean the name to ensure it's a valid filename
        var cleanName = CleanName(originalName);

        // If it's unique, return it
        if (!this.generatedFileNames.Contains(cleanName))
        {
            this.generatedFileNames.Add(cleanName);
            this.fileNameMappings[originalName] = cleanName;
            return cleanName;
        }

        // Otherwise, add a suffix to make it unique
        var counter = 1;
        string uniqueName;
        do
        {
            var baseName = cleanName.EndsWith(".g.cs")
                ? cleanName.Substring(0, cleanName.Length - 5)
                : cleanName;
            uniqueName = $"{baseName}_{counter}.g.cs";
            counter++;
        } while (this.generatedFileNames.Contains(uniqueName));

        this.generatedFileNames.Add(uniqueName);
        this.fileNameMappings[originalName] = uniqueName;
        return uniqueName;
    }

    private Dictionary<string, string> segmentKeyToClassName = new();
    private HashSet<string> usedClassNames = new();

    private string GetUniqueClassName(string segmentName, IEnumerable<string> previousSegmentNames)
    {
        segmentName = classNamePrefix + ToCSharpCase(segmentName);
        var segmentsKey = string.Join("", previousSegmentNames.Append(segmentName));
        if (this.segmentKeyToClassName.TryGetValue(segmentsKey, out var existingName))
        {
            return existingName;
        }

        int i = 0;
        while (!this.usedClassNames.Add(segmentName + $"{(i > 0 ? $"{i}" : "")}"))
        {
            i++;
        }
        if (i > 0)
        {
            segmentName += $"{i}";
        }

        this.segmentKeyToClassName[segmentsKey] = segmentName;
        return segmentName;
    }

    private static string ToCSharpCase(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        var parts = name.Split(['_', '-'], StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] =
                parts[i].Length > 1
                    ? char.ToUpper(parts[i][0]) + parts[i].Substring(1)
                    : parts[i].ToUpper();
        }

        return string.Join(string.Empty, parts);
    }

    private static string CleanName(string name)
    {
        return name.Replace("-", "_").Replace(" ", "_");
    }

    protected static string ToParameterName(string name)
    {
        if (char.IsLower(name[0]))
        {
            return name;
        }
        return char.ToLower(name[0]) + name.Substring(1);
    }
}

public sealed class ResultApiClassGenerator(string apiClientName, string classNamePrefix)
    : FluentApiClassGeneratorBase(apiClientName, classNamePrefix)
{
    protected override string GetMethod(ApiMethod method, string[] ctorArgs)
    {
        var parametersInRequestType = method
            .RequestType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p =>
                p.DeclaredAccessibility == Accessibility.Public
                && !p.IsStatic
                && p.SetMethod != null
                && p.SetMethod.DeclaredAccessibility == Accessibility.Public
            )
            .ToArray();

        var parametersNotInCtor = parametersInRequestType
            .Where(p => !ctorArgs.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .ToArray();

        string? requestAssignment;
        IEnumerable<string> parametersInMethodSig;
        if (parametersNotInCtor.Length == parametersInRequestType.Length)
        {
            // none of the properties come from the route, so we can just use the request type as is
            requestAssignment = null;
            parametersInMethodSig =
            [
                $"{method.RequestType.ToDisplayString()} request_",
                "CancellationToken ct = default",
            ];
        }
        else
        {
            requestAssignment =
                @$"
        var request_ = new {method.RequestType.ToDisplayString()}()
        {{
            {string.Join(", ", parametersInRequestType.Select(p => p.Name + " = " + ToParameterName(p.Name)))}
        }};
        ";
            parametersInMethodSig = parametersNotInCtor
                .Select(p => $"{p.Type.ToDisplayString()} {ToParameterName(p.Name)}")
                .Append("CancellationToken ct = default");
        }

        return $$"""
    public Task<ApiResponse<{{method.ReturnType.ToDisplayString()}}>> {{method.Name}}Async({{string.Join(
                ", ",
                parametersInMethodSig
            )}})
    {
        {{requestAssignment}}
        return ProtectedClient.{{method.Name}}(request_, ct);
    }
""";
    }
}

public sealed class ThrowingApiClassGenerator(string apiClientName, string classNamePrefix)
    : FluentApiClassGeneratorBase(apiClientName, classNamePrefix)
{
    protected override string GetMethod(ApiMethod method, string[] ctorArgs)
    {
        var parametersInRequestType = method
            .RequestType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p =>
                p.DeclaredAccessibility == Accessibility.Public
                && !p.IsStatic
                && p.SetMethod != null
                && p.SetMethod.DeclaredAccessibility == Accessibility.Public
            )
            .ToArray();

        var parametersNotInCtor = parametersInRequestType
            .Where(p => !ctorArgs.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
            .ToArray();

        string? requestAssignment;
        IEnumerable<string> parametersInMethodSig;
        if (parametersNotInCtor.Length == parametersInRequestType.Length)
        {
            // none of the properties come from the route, so we can just use the request type as is
            requestAssignment = null;
            parametersInMethodSig = [$"{method.RequestType.ToDisplayString()} request_"];
        }
        else
        {
            requestAssignment =
                @$"
        var request_ = new {method.RequestType.ToDisplayString()}()
        {{
            {string.Join(", ", parametersInRequestType.Select(p => p.Name + " = " + ToParameterName(p.Name)))}
        }};
        ";
            parametersInMethodSig = parametersNotInCtor.Select(p =>
                $"{p.Type.ToDisplayString()} {ToParameterName(p.Name)}"
            );
        }

        return $$"""
    public async Task<{{method.ReturnType.ToDisplayString()}}> {{method.Name}}Async({{string.Join(
                ", ",
                parametersInMethodSig.Append("CancellationToken ct = default")
            )}})
    {
        {{requestAssignment}}
        var result = await ProtectedClient.{{method.Name}}(request_, ct);
        result.ThrowIfError();
        return result.Value!;
    }

    public {{method.ReturnType.ToDisplayString()}} {{method.Name}}({{string.Join(
                ", ",
                parametersInMethodSig
            )}})
    {
        {{requestAssignment}}
        var result = ProtectedClient.{{method.Name}}(request_, CancellationToken.None).GetAwaiter().GetResult();
        result.ThrowIfError();
        return result.Value!;
    }
""";
    }
}
