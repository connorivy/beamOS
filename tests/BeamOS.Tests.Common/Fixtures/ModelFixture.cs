using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Interfaces;

namespace BeamOS.Tests.Common.Fixtures;

public interface IHasExpectedNodeResults2 : IModelFixture2
{
    public NodeResultFixture2[] ExpectedNodeResults { get; }
}

public interface IHasExpectedNodeDisplacementResults
    : IHasModelSettings,
        IHasFixtureId,
        IHasPhysicalModelSettings
{
    public NodeDisplacementResultFixture[] ExpectedNodeDisplacementResults { get; }
}

public interface IHasModelSettings
{
    ModelSettings Settings { get; }
}

public interface IHasPhysicalModelSettings
{
    PhysicalModelSettings ModelSettings { get; }
}

public interface IHasFixtureId
{
    FixtureId Id { get; }
}

public abstract class ModelFixture2 : FixtureBase2, IHasSourceInfo, IModelFixture2
{
    public sealed override FixtureId Id => this.ModelGuid;
    public abstract Guid ModelGuid { get; }
    public abstract ModelSettings Settings { get; }
    public abstract SourceInfo SourceInfo { get; }
    public virtual string Name { get; } = "Test Model";
    public virtual string Description { get; } = "Test Model Description";
    public virtual NodeFixture2[] Nodes { get; } = [];
    public virtual Element1dFixture2[] Element1ds { get; } = [];

    public virtual MaterialFixture2[] Materials { get; } = [];
    public virtual SectionProfileFixture2[] SectionProfiles { get; } = [];
    public virtual PointLoadFixture2[] PointLoads { get; } = [];
    public virtual MomentLoadFixture2[] MomentLoads { get; } = [];
}
