using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.ApiClient.Builders;

public record CreateElement1dRequestBuilder : CreateModelEntityRequestBuilderBase
{
    public required FixtureId StartNodeId { get; init; }
    public required FixtureId EndNodeId { get; init; }
    public FixtureId? MaterialId { get; init; }
    public FixtureId? SectionProfileId { get; init; }
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
}

public record CreatePointLoadRequestBuilder : CreateModelEntityRequestBuilderBase
{
    public required FixtureId NodeId { get; init; }
    public required Force Force { get; init; }
    public required UnitVector3D Direction { get; init; }
}

public abstract record CreateEntityRequestBuilderBase
{
    public FixtureId Id { get; init; } = Guid.NewGuid().ToString();
}

public abstract record CreateModelEntityRequestBuilderBase : CreateEntityRequestBuilderBase
{
    internal FixtureId ModelId { get; init; }
}

public record struct FixtureId(string Id)
{
    public static implicit operator FixtureId(string id) => new(id);
}
