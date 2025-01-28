using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

namespace BeamOs.Tests.Common;

public interface IModelFixture : ITestFixture
{
    public string GuidString { get; }
    public PhysicalModelSettings Settings { get; }
}

public interface IHasSourceInfo
{
    public SourceInfo SourceInfo { get; }
}

public interface ITestFixture : IHasSourceInfo
{
    public string Description { get; }
    public string Name { get; }
    public IBeamOsEntityResponse MapToResponse();
}
