using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

public sealed class DocumentGenerator
{
    private readonly HashSet<string> generatedFileNames = [];
    private readonly Dictionary<string, string> fileNameMappings = [];

    /// <summary>
    /// Generates a csharp class for each member in the apiclient route tree and adds it to the compilation
    /// </summary>
    /// <param name="spc"></param>
    /// <param name="routeTree"></param>
    public void GenerateDocuments(SourceProductionContext spc, Dictionary<string, RouteSegment> routeTree)
    {
        if (routeTree.Count == 0)
        {
            return; // No routes to generate
        }

        var apiClientName = "FluentApiClient";
        RouteSegment topRouteSegment = new()
        {
            Name = apiClientName,
            IsParameter = false
        };

        // Generate the main fluent API client
        this.GenerateFluentApiClient(spc, routeTree, apiClientName);

        // Generate accessor classes for each route segment
        this.GenerateAccessorClasses(spc, routeTree, [topRouteSegment]);
    }

    private void GenerateFluentApiClient(SourceProductionContext spc, Dictionary<string, RouteSegment> routeTree, string apiClientName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine();
        sb.AppendLine("namespace BeamOs.CodeGen.StructuralAnalysisApiClient;");
        sb.AppendLine();
        sb.AppendLine($"public class {apiClientName}");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly IStructuralAnalysisApiClientV1 _apiClient;");
        sb.AppendLine();
        sb.AppendLine($"    public {apiClientName}(IStructuralAnalysisApiClientV1 apiClient)");
        sb.AppendLine("    {");
        sb.AppendLine("        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Generate root level properties
        foreach (var rootSegment in routeTree.Values.Where(s => !s.IsParameter))
        {
            var className = apiClientName + $"_{rootSegment.Name}";
            sb.AppendLine($"    public {className} {rootSegment.Name} => new {className}(_apiClient);");
        }

        sb.AppendLine("}");

        spc.AddSource(this.GetUniqueFileName("FluentApiClient.g.cs"), SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private void GenerateAccessorClasses(SourceProductionContext spc, Dictionary<string, RouteSegment> segments, RouteSegment[] previousSegments)
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

    private void GenerateNamedAccessor(SourceProductionContext spc, RouteSegment segment, RouteSegment[] previousSegments)
    {
        // var originalClassName = segment.Name + "Accessor";
        // var className = this.GetUniqueClassName(originalClassName);
        var className = string.Join("_", previousSegments.Select(p => p.Name).Append(segment.Name));

        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Threading;");
        sb.AppendLine("using System.Threading.Tasks;");
        sb.AppendLine();
        sb.AppendLine("namespace BeamOs.CodeGen.StructuralAnalysisApiClient;");
        sb.AppendLine();
        var ctorArgs = previousSegments
            .Append(segment)
            .Where(p => p.IsParameter)
            .Select(p => $"{p.ParameterType ?? throw new InvalidOperationException($"Parameter type is required for parameter segment '{p.Name}'")} {p.Name}")
            .Prepend($"IStructuralAnalysisApiClientV1 _apiClient");

        sb.AppendLine($"public class {className}({string.Join(", ", ctorArgs)})");
        sb.AppendLine("{");

        // Generate child accessors and indexers
        var childIndexers = new HashSet<string>();
        var methodCallArgs = previousSegments.Append(segment).Where(p => p.IsParameter).Select(p => p.Name).Prepend("_apiClient").ToArray();
        foreach (var child in segment.Children.Values)
        {
            if (child.IsParameter)
            {
                // Generate indexer for parameter child
                var paramType = child.ParameterType ?? throw new InvalidOperationException($"Parameter type is required for parameter segment '{child.Name}'");
                var indexerSignature = $"this[{paramType}]";

                if (!childIndexers.Contains(indexerSignature))
                {
                    childIndexers.Add(indexerSignature);
                    var childClassName = className + $"_{child.Name}";
                    sb.AppendLine($"    public {childClassName} this[{paramType} {child.Name}] => new {childClassName}({string.Join(", ", methodCallArgs.Append(child.Name))});");
                }
            }
            else
            {
                // Generate property for named child
                var childClassName = className + $"_{child.Name}";
                sb.AppendLine($"    public {childClassName} @{child.Name} => new {childClassName}({string.Join(", ", methodCallArgs)});");
            }
        }

        // Generate methods for this segment
        foreach (var method in segment.Methods)
        {
            var parametersInMethodSig = method.Parameters.Where(p => !methodCallArgs.Contains(p.Name));

            sb.AppendLine($"    public {method.ReturnType} {method.Name}({string.Join(", ", parametersInMethodSig.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"))})");
            sb.AppendLine("    {");
            sb.AppendLine($"        return _apiClient.{method.Name}({string.Join(", ", method.Parameters.Select(p => p.Name))});");
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");

        // Use the original class name for filename to ensure unique naming based on original segments
        spc.AddSource(this.GetUniqueFileName($"{className}.g.cs"), SourceText.From(sb.ToString(), Encoding.UTF8));

        // Recursively generate child accessors
        this.GenerateAccessorClasses(spc, segment.Children, [.. previousSegments, segment]);
    }

    private string GetUniqueFileName(string originalName)
    {
        // Check if we already have a mapping for this filename
        if (this.fileNameMappings.TryGetValue(originalName, out var mappedName))
        {
            return mappedName;
        }

        // Clean the name to ensure it's a valid filename
        var cleanName = originalName.Replace("-", "_").Replace(" ", "_");

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
            var baseName = cleanName.EndsWith(".g.cs") ? cleanName.Substring(0, cleanName.Length - 5) : cleanName;
            uniqueName = $"{baseName}_{counter}.g.cs";
            counter++;
        } while (this.generatedFileNames.Contains(uniqueName));

        this.generatedFileNames.Add(uniqueName);
        this.fileNameMappings[originalName] = uniqueName;
        return uniqueName;
    }
}
