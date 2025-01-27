using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

namespace BeamOs.Tests.Common;

public interface IModelFixture : IHasSourceInfo
{
    string Description { get; }
    string GuidString { get; }
    string Name { get; }
    PhysicalModelSettings Settings { get; }
}

public interface IHasSourceInfo
{
    public SourceInfo SourceInfo { get; }
}
