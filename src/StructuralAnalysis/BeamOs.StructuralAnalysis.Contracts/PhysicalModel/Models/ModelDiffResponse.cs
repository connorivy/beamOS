using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record ModelDiffResponse
{
    public List<EntityDiff<NodeResponse>> Nodes { get; init; } = [];
    public List<EntityDiff<Element1dResponse>> Element1ds { get; init; } = [];
    public List<EntityDiff<MaterialResponse>> Materials { get; init; } = [];
    public List<EntityDiff<SectionProfileResponse>> SectionProfiles { get; init; } = [];
    public List<EntityDiff<PointLoadResponse>> PointLoads { get; init; } = [];
    public List<EntityDiff<MomentLoadResponse>> MomentLoads { get; init; } = [];
}

public record EntityDiff<T>
{
    public required DiffStatus Status { get; init; }
    public required T Entity { get; init; }
}

public enum DiffStatus
{
    Added = 1,
    Removed = 2,
    Modified = 3,
}
