using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeamOs.Common.Domain.Models;

public class CustomData : BeamOSValueObject
{
    private static JsonSerializerOptions serializerOptions;

    static CustomData()
    {
        serializerOptions = new();
        serializerOptions.Converters.Add(new StringConverter());
    }

    public CustomData(Dictionary<string, object>? initialData = null)
    {
        this.data = initialData ?? [];
    }

    private Dictionary<string, object> data;

    [MaxLength(128)]
    public string Data
    {
        get => JsonSerializer.Serialize(this.data);
        private set
        {
            this.data = JsonSerializer.Deserialize<Dictionary<string, object>>(
                value,
                serializerOptions
            );
            ;
        }
    }
    public object this[string key] => this.data[key];

    [Pure]
    public CustomData AddOrModifyData(string key, object value)
    {
        Dictionary<string, object> copy = new(this.data);
        copy[key] = value;

        return new(copy);
    }

    [Pure]
    public Dictionary<string, object> AsDict() => this.data;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.data;
    }

    [Obsolete("EF Core Ctor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private CustomData()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    { }
}

//public class StringConverter : JsonConverter<string>
//{
//    public override string Read(
//        ref Utf8JsonReader reader,
//        Type typeToConvert,
//        JsonSerializerOptions options
//    )
//    {
//        if (reader.TokenType == JsonTokenType.String)
//            return reader.GetString();

//        // Handle other cases (e.g., numbers, booleans) if needed
//        return reader.GetString();
//    }

//    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
//    {
//        writer.WriteStringValue(value);
//    }
//}

public class StringConverter : JsonConverter<object>
{
    public override object Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.String)
            return reader.GetString();

        // Handle other cases (e.g., numbers, booleans) if needed
        // For now, just return the value as-is
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            return doc.RootElement.Clone();
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value is string stringValue)
            writer.WriteStringValue(stringValue);
        else
            throw new NotSupportedException("Custom converter only supports string values.");
    }
}
