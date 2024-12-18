using System.Text.Json;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

[JsonSerializable(typeof(CreateNodeRequest))]
[JsonSerializable(typeof(UpdateNodeRequest))]
[JsonSerializable(typeof(NodeResponse))]
[JsonSerializable(typeof(Result<NodeResponse>))]
internal partial class BeamOsJsonSerializerContext : JsonSerializerContext { }

public static class BeamOsSerializerOptions
{
    private static JsonSerializerOptions? options;
    public static JsonSerializerOptions Default
    {
        get
        {
            if (options is null)
            {
                options = new() { PropertyNameCaseInsensitive = true };
                options.TypeInfoResolverChain.Insert(0, BeamOsJsonSerializerContext.Default);
            }
            return options;
        }
    }

    public static Action<JsonSerializerOptions> DefaultConfig { get; } =
        static (options) =>
        {
            options.PropertyNameCaseInsensitive = true;
            options.TypeInfoResolverChain.Insert(0, BeamOsJsonSerializerContext.Default);
        };
}
