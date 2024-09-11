using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Text.Json;

namespace BeamOs.Common.Domain.Models;

public class CustomData : BeamOSValueObject
{
    public CustomData(Dictionary<string, object>? initialData = null)
    {
        this.data = initialData ?? [];
    }

    private Dictionary<string, object> data;

    [MaxLength(128)]
    public string Data
    {
        get => JsonSerializer.Serialize(this.data);
        private set => this.data = JsonSerializer.Deserialize<Dictionary<string, object>>(value);
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
