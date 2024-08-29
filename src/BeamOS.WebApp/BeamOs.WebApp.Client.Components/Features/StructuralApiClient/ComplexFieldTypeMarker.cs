using System.Text.Json.Serialization;

namespace BeamOs.WebApp.Client.Components.Features.StructuralApiClient;

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
