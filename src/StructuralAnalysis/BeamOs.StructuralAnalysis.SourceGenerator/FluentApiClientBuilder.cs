using Microsoft.CodeAnalysis;

namespace BeamOs.StructuralAnalysis.SourceGenerator;

public sealed class FluentApiClientBuilder
{
    private readonly Dictionary<string, RouteSegment> routeTree = [];

    /// <summary>
    /// Adds a method to the generated API client.
    /// Input: /api/models/{modelId:Guid}/node/{id:int}, GetNode, [modelId, id, options]
    /// Expected behavior: Builds a method in the API client that will be called like this:
    ///     apiClient.Models[<modelId>].Nodes.GetNode(id, options);
    /// The implementation of the 'getNode' method should just be
    ///     return await apiClient.<methodName>(<parameters>);
    /// </summary>
    /// <param name="route"></param>
    /// <param name="methodName"></param>
    public void AddMethodAtRoute(
        string route,
        string methodName,
        string returnType,
        ICollection<IParameterSymbol> parameters
    )
    {
        var routeSegments = this.ParseRoute(route);
        var currentSegment = this.routeTree;

        foreach (var segment in routeSegments)
        {
            if (!currentSegment.ContainsKey(segment.Name))
            {
                currentSegment[segment.Name] = new RouteSegment
                {
                    Name = segment.Name,
                    IsParameter = segment.IsParameter,
                    ParameterType = segment.ParameterType,
                    Children = [],
                    Methods = [],
                };
            }

            currentSegment = currentSegment[segment.Name].Children;
        }

        // Add the method to the final segment
        var finalSegment = routeSegments.LastOrDefault();
        if (finalSegment != null)
        {
            var targetSegment = this.GetSegmentByPath(routeSegments);
            targetSegment?.Methods.Add(
                new ApiMethod
                {
                    Name = methodName,
                    ReturnType = returnType,
                    Parameters = parameters,
                }
            );
        }
    }

    private List<RouteSegmentInfo> ParseRoute(string route)
    {
        var segments = new List<RouteSegmentInfo>();

        // Remove leading slash and split by '/'
        var parts = route.TrimStart('/').Split('/');

        foreach (var part in parts)
        {
            // Skip empty parts and "api" prefix as requested
            if (
                string.IsNullOrEmpty(part) || part.Equals("api", StringComparison.OrdinalIgnoreCase)
            )
            {
                continue;
            }

            if (part.StartsWith("{") && part.EndsWith("}"))
            {
                // Parse parameter: {modelId:Guid} or {id:int}
                var paramContent = part.Substring(1, part.Length - 2);
                var colonIndex = paramContent.IndexOf(':');

                string paramName;
                string paramType = "object";

                if (colonIndex > 0)
                {
                    paramName = paramContent.Substring(0, colonIndex);
                    var typeInfo = paramContent.Substring(colonIndex + 1);
                    paramType = this.MapTypeFromRoute(typeInfo);
                }
                else
                {
                    paramName = paramContent;
                }

                segments.Add(
                    new RouteSegmentInfo
                    {
                        Name = paramName,
                        IsParameter = true,
                        ParameterType = paramType,
                    }
                );
            }
            else
            {
                // Regular path segment - pluralize for collection access
                var segmentName = this.PluralizeSegment(part);
                segments.Add(
                    new RouteSegmentInfo
                    {
                        Name = segmentName,
                        IsParameter = false,
                        ParameterType = null,
                    }
                );
            }
        }

        return segments;
    }

    private string PluralizeSegment(string original) => original.Replace('-', '_');

    private string MapTypeFromRoute(string routeType)
    {
        return routeType.ToLowerInvariant() switch
        {
            "guid" => "Guid",
            "int" => "int",
            "long" => "long",
            "bool" => "bool",
            "double" => "double",
            "float" => "float",
            "decimal" => "decimal",
            "datetime" => "DateTime",
            _ => "string",
        };
    }

    private RouteSegment? GetSegmentByPath(List<RouteSegmentInfo> path)
    {
        var current = this.routeTree;
        RouteSegment? lastSegment = null;

        foreach (var segment in path)
        {
            if (current.ContainsKey(segment.Name))
            {
                lastSegment = current[segment.Name];
                current = lastSegment.Children;
            }
            else
            {
                return null;
            }
        }

        return lastSegment;
    }

    public void AddClassesToCompilation(SourceProductionContext spc)
    {
        var documentGenerator = new DocumentGenerator();
        documentGenerator.GenerateDocuments(spc, this.routeTree);
    }
}

public class RouteSegmentInfo
{
    public string Name { get; set; } = string.Empty;
    public bool IsParameter { get; set; }
    public string? ParameterType { get; set; }
}

public class RouteSegment
{
    public string Name
    {
        get;
        set => field = CleanName(value);
    } = string.Empty;
    public bool IsParameter { get; set; }
    public string? ParameterType { get; set; }
    public Dictionary<string, RouteSegment> Children { get; set; } = [];
    public List<ApiMethod> Methods { get; set; } = [];

    private static string CleanName(string name)
    {
        return name.Replace("-", "_").Replace(" ", "_");
    }
}

public class ApiMethod
{
    public string Name { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public ICollection<IParameterSymbol> Parameters { get; set; } = [];
}
