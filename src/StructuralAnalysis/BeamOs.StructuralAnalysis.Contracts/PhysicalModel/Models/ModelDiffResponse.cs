using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record ModelDiffData
{
    public required Guid BaseModelId { get; init; }
    public required Guid TargetModelId { get; init; }
    public List<EntityDiff<NodeResponse>> Nodes { get; init; } = [];
    public List<EntityDiff<Element1dResponse>> Element1ds { get; init; } = [];
    public List<EntityDiff<MaterialResponse>> Materials { get; init; } = [];
    public List<EntityDiff<SectionProfileResponse>> SectionProfiles { get; init; } = [];
    public List<EntityDiff<PointLoadResponse>> PointLoads { get; init; } = [];
    public List<EntityDiff<MomentLoadResponse>> MomentLoads { get; init; } = [];
}

public record EntityDiff<T>
{
    public DiffStatus Status
    {
        get
        {
            if (this.SourceEntity is null && this.TargetEntity is not null)
            {
                return DiffStatus.Added;
            }
            else if (this.SourceEntity is not null && this.TargetEntity is null)
            {
                return DiffStatus.Removed;
            }
            else
            {
                return DiffStatus.Modified;
            }
        }
    }
    public required T? TargetEntity { get; init; }
    public required T? SourceEntity { get; init; }
}

public enum DiffStatus
{
    Added = 1,
    Removed = 2,
    Modified = 3,
}
