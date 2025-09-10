using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

public record CreateElement1dRequest
{
    [SetsRequiredMembers]
    public CreateElement1dRequest(
        int startNodeId,
        int endNodeId,
        int materialId,
        int sectionProfileId,
        Angle? sectionProfileRotation,
        int? id,
        Dictionary<string, string>? metadata
    )
    {
        this.StartNodeId = startNodeId;
        this.EndNodeId = endNodeId;
        this.MaterialId = materialId;
        this.SectionProfileId = sectionProfileId;
        this.SectionProfileRotation = sectionProfileRotation ?? new(0, AngleUnit.Degree);
        this.Id = id;
        this.Metadata = metadata;
    }

    public CreateElement1dRequest() { }

    [SetsRequiredMembers]
    public CreateElement1dRequest(Element1dData element1DData)
        : this(
            element1DData.StartNodeId,
            element1DData.EndNodeId,
            element1DData.MaterialId,
            element1DData.SectionProfileId,
            element1DData.SectionProfileRotation,
            null,
            element1DData.Metadata
        ) { }

    public required int StartNodeId { get; init; }
    public required int EndNodeId { get; init; }
    public required int MaterialId { get; init; }
    public required int SectionProfileId { get; init; }
    public Angle? SectionProfileRotation { get; init; } = new(0, AngleUnit.Degree);
    public int? Id { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
