using System.Text.Json;
using System.Text.Json.Serialization;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.ApiClient.Builders;

public record CreateElement1dRequestBuilder : CreateModelEntityRequestBuilderBase
{
    public required FixtureId StartNodeId { get; init; }
    public required FixtureId EndNodeId { get; init; }
    public FixtureId MaterialId { get; set; }
    public FixtureId SectionProfileId { get; set; }
    public Angle SectionProfileRotation { get; init; }
}

public record CreateNodeRequestBuilder : CreateModelEntityRequestBuilderBase
{
    public required Point LocationPoint { get; init; }
    public required RestraintRequest Restraint { get; init; }
}

public record CreateMaterialRequestBuilder : CreateModelEntityRequestBuilderBase
{
    public Pressure ModulusOfElasticity { get; init; }
    public Pressure ModulusOfRigidity { get; init; }
}

public record CreateSectionProfileRequestBuilder : CreateModelEntityRequestBuilderBase
{
    public Area Area { get; init; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; init; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; init; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; init; }
    public Area StrongAxisShearArea { get; init; }
    public Area WeakAxisShearArea { get; init; }
}

public record CreatePointLoadRequestBuilder : CreateModelEntityRequestBuilderBase
{
    public required FixtureId NodeId { get; init; }
    public required Force Force { get; init; }
    public required UnitVector3D Direction { get; init; }
}

public abstract record CreateModelEntityRequestBuilderBase
{
    public FixtureId ModelId { get; internal init; }
    public FixtureId Id { get; init; } = Guid.NewGuid().ToString();
}

[JsonConverter(typeof(FixtureIdConverter))]
public record struct FixtureId(string Id)
{
    public static implicit operator FixtureId(string id) => new(id);

    public static implicit operator FixtureId(Guid id) => new(id.ToString());

    public override string ToString() => this.Id;
}

public class FixtureIdConverter : JsonConverter<FixtureId>
{
    public override FixtureId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return new(reader.GetString());
    }

    public override void Write(
        Utf8JsonWriter writer,
        FixtureId value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.Id);
    }
}
