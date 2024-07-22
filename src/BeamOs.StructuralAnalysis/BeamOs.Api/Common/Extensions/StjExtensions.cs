using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace BeamOs.Api.Common.Extensions;

public static class StjExtensions
{
    public static void IgnoreRequiredKeyword(this JsonSerializerOptions opts)
    {
        opts.TypeInfoResolver = opts.TypeInfoResolver?.WithAddedModifier(ti =>
        {
            if (ti.Kind != JsonTypeInfoKind.Object)
            {
                return;
            }

            for (var i = 0; i < ti.Properties.Count; i++)
            {
                var pi = ti.Properties[i];
                if (pi.IsRequired)
                {
                    pi.IsRequired = false;
                }
            }
        });
    }
}
