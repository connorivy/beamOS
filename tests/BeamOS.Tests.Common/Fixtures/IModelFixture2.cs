using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public interface IModelFixture2
{
    string Description { get; }
    Element1dFixture2[] Element1ds { get; }
    MaterialFixture2[] Materials { get; }
    MomentLoadFixture2[] MomentLoads { get; }
    string Name { get; }
    NodeFixture2[] Nodes { get; }
    PointLoadFixture2[] PointLoads { get; }
    SectionProfileFixture2[] SectionProfiles { get; }
    ModelSettings Settings { get; }
    SourceInfo SourceInfo { get; }
}
