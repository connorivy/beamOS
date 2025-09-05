using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BeamOs.StructuralAnalysis.Api;

public class EnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        Console.WriteLine(context.JsonTypeInfo.Type.Name);

        if (!context.JsonTypeInfo.Type.IsEnum)
        {
            return Task.CompletedTask;
        }

        var enumType = context.JsonTypeInfo.Type;

        //This is because of a bug that doesn't populate this.
        schema.Enum.Clear(); //Hack just in case they fix the bug.
        foreach (var name in Enum.GetNames(enumType))
        {
            schema.Enum.Add(name);
        }

        // Add x-ms-enum extension
        var enumValues = new JsonArray();
        foreach (var enumVal in Enum.GetNames(enumType))
        {
            enumValues.Add(
                new JsonObject
                {
                    ["name"] = enumVal,
                    ["value"] = (int)Enum.Parse(enumType, enumVal),
                    ["description"] = this.GetEnumDescription(enumType, enumVal),
                }
            );
        }

        schema.Extensions["x-ms-enum"] = new JsonNodeExtension(
            new JsonObject
            {
                ["name"] = enumType.Name,
                ["modelAsString"] = false,
                ["values"] = enumValues,
            }
        );

        // Add enum schemas to OneOf
        foreach (var name in Enum.GetNames(enumType))
        {
            var enumValue = (int)Enum.Parse(enumType, name);
            var enumSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Integer,
                Enum = [enumValue],
                Title = name,
            };

            schema.OneOf.Add(enumSchema);
        }

        return Task.CompletedTask;
    }

    private string GetEnumDescription(Type type, string name)
    {
        var memberInfo = type.GetMember(name).FirstOrDefault();
        var attribute = memberInfo?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? string.Empty;
    }
}

// public class DictSchemaTransformer : IOpenApiSchemaTransformer
// {
//     public Task TransformAsync(
//         OpenApiSchema schema,
//         OpenApiSchemaTransformerContext context,
//         CancellationToken cancellationToken
//     )
//     {
//         Console.WriteLine(context.JsonTypeInfo.Type.Name);

//         if (!context.JsonTypeInfo.Type)
//         {
//             return Task.CompletedTask;
//         }

//         var enumType = context.JsonTypeInfo.Type;

//         schema.PatternProperties

//         //This is because of a bug that doesn't populate this.
//         schema.Enum.Clear(); //Hack just in case they fix the bug.
//         foreach (var name in Enum.GetNames(enumType))
//         {
//             schema.Enum.Add(name);
//         }

//         // Add x-ms-enum extension
//         var enumValues = new JsonArray();
//         foreach (var enumVal in Enum.GetNames(enumType))
//         {
//             enumValues.Add(
//                 new JsonObject
//                 {
//                     ["name"] = enumVal,
//                     ["value"] = (int)Enum.Parse(enumType, enumVal),
//                     ["description"] = this.GetEnumDescription(enumType, enumVal),
//                 }
//             );
//         }

//         schema.Extensions["x-ms-enum"] = new JsonNodeExtension(
//             new JsonObject
//             {
//                 ["name"] = enumType.Name,
//                 ["modelAsString"] = false,
//                 ["values"] = enumValues,
//             }
//         );

//         // Add enum schemas to OneOf
//         foreach (var name in Enum.GetNames(enumType))
//         {
//             var enumValue = (int)Enum.Parse(enumType, name);
//             var enumSchema = new OpenApiSchema
//             {
//                 Type = JsonSchemaType.Integer,
//                 Enum = [enumValue],
//                 Title = name,
//             };

//             schema.OneOf.Add(enumSchema);
//         }

//         return Task.CompletedTask;
//     }

//     private string GetEnumDescription(Type type, string name)
//     {
//         var memberInfo = type.GetMember(name).FirstOrDefault();
//         var attribute = memberInfo?.GetCustomAttribute<DescriptionAttribute>();
//         return attribute?.Description ?? string.Empty;
//     }
// }
