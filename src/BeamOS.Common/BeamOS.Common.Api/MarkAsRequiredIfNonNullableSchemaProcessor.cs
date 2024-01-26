using NJsonSchema;
using NJsonSchema.Generation;

namespace BeamOS.Common.Api;

public class MarkAsRequiredIfNonNullableSchemaProcessor : ISchemaProcessor
{
    //https://github.com/RicoSuter/NSwag/issues/3110
    // may need to change schema.Properties to schema.ActualProperties for issues with inheritance
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
