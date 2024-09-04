using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

[JsonConverter(typeof(ComplexFieldTypeMarkerConverter))]
public class ComplexFieldTypeMarker(bool isRequired) : Dictionary<string, object?>
{
    [JsonIgnore]
    public bool IsRequired { get; } = isRequired;

    [JsonIgnore]
    public Dictionary<string, object> ValuesWithDisplayInformation { get; } = [];

    [JsonIgnore]
    public int NumRecordsAutoFilled { get; set; }

    public void Add2(string key, object value)
    {
        this.ValuesWithDisplayInformation.Add(key, value);
        this.Add(key, value is ComplexFieldTypeMarker ? value : null);
    }

    public object Get(string key)
    {
        if (this.ValuesWithDisplayInformation[key] is SimpleFieldTypeMarker simple)
        {
            return simple with { Value = this[key] };
        }
        return this.ValuesWithDisplayInformation[key];
    }

    public void Set(string key, object value) => this[key] = value;
}

public class ComplexFieldTypeMarkerConverter : JsonConverter<ComplexFieldTypeMarker>
{
    public override ComplexFieldTypeMarker? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) => throw new NotImplementedException();

    public override void Write(
        Utf8JsonWriter writer,
        ComplexFieldTypeMarker value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
        foreach (var kvp in value.ValuesWithDisplayInformation)
        {
            if (
                kvp.Value is SimpleFieldTypeMarker simpleFieldTypeMarker
                && !simpleFieldTypeMarker.IsRequired
                && value[kvp.Key] is null
            )
            {
                // Removes optional values that the user hasn't given a value to.
                // If we don't remove these values, then they could potentially later get
                // deserialize to a value type from null.
                continue;
            }

            writer.WritePropertyName(kvp.Key);
            JsonSerializer.Serialize(writer, value[kvp.Key], options);
        }
        writer.WriteEndObject();
    }
}
