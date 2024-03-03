using NJsonSchema;
using NJsonSchema.Generation;

namespace BeamOs.Api.Common;

public class MarkAsRequiredIfNonNullableSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        foreach (var (propName, prop) in context.Schema.ActualProperties)
        {
            if (!prop.IsNullable(SchemaType.OpenApi3))
            {
                prop.IsRequired = true;
            }
        }
    }
}
