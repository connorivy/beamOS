using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

namespace BeamOs.Tests.Common;
public interface IModelFixture
{
    string Description { get; }
    string GuidString { get; }
    string Name { get; }
    PhysicalModelSettings Settings { get; }
    SourceInfo SourceInfo { get; }
}