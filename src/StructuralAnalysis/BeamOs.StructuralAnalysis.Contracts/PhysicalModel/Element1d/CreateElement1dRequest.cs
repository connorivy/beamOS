using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;

public record CreateElement1dRequest
{
    [SetsRequiredMembers]
    public CreateElement1dRequest(
        int startNodeId,
        int endNodeId,
        int materialId,
        int sectionProfileId,
        AngleContract? sectionProfileRotation,
        int? id,
        Dictionary<string, string>? metadata
    )
    {
        this.StartNodeId = startNodeId;
        this.EndNodeId = endNodeId;
        this.MaterialId = materialId;
        this.SectionProfileId = sectionProfileId;
        this.SectionProfileRotation = sectionProfileRotation;
        this.Id = id;
        this.Metadata = metadata;
    }

    public CreateElement1dRequest() { }

    public required int StartNodeId { get; init; }
    public required int EndNodeId { get; init; }
    public required int MaterialId { get; init; }
    public required int SectionProfileId { get; init; }
    public AngleContract? SectionProfileRotation { get; init; }
    public int? Id { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
