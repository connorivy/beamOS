using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;

public record Element1dData
{
    [SetsRequiredMembers]
    public Element1dData(
        int startNodeId,
        int endNodeId,
        int materialId,
        int sectionProfileId,
        Angle? sectionProfileRotation,
        Dictionary<string, string>? metadata
    )
    {
        this.StartNodeId = startNodeId;
        this.EndNodeId = endNodeId;
        this.MaterialId = materialId;
        this.SectionProfileId = sectionProfileId;
        this.SectionProfileRotation = sectionProfileRotation ?? new(0, AngleUnit.Degree);
        this.Metadata = metadata;
    }

    public Element1dData() { }

    public required int StartNodeId { get; init; }
    public required int EndNodeId { get; init; }
    public required int MaterialId { get; init; }
    public required int SectionProfileId { get; init; }
    public Angle? SectionProfileRotation { get; init; } = new(0, AngleUnit.Degree);
    public Dictionary<string, string>? Metadata { get; init; }
}

public record PutElement1dRequest : Element1dData, IHasIntId, IBeamOsEntityRequest
{
    [SetsRequiredMembers]
    public PutElement1dRequest(
        int id,
        int startNodeId,
        int endNodeId,
        int materialId,
        int sectionProfileId,
        Angle? sectionProfileRotation,
        Dictionary<string, string>? metadata = null
    )
        : base(
            startNodeId,
            endNodeId,
            materialId,
            sectionProfileId,
            sectionProfileRotation,
            metadata
        )
    {
        this.Id = id;
    }

    public PutElement1dRequest() { }

    public required int Id { get; init; }
}
