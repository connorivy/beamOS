using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public interface IHasExpectedNodeResults2
{
    public NodeResultFixture2[] ExpectedNodeResults { get; }
}

public abstract class ModelFixture2 : FixtureBase2, IHasSourceInfo, IModelFixture2
{
    public abstract override Guid Id { get; }
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
