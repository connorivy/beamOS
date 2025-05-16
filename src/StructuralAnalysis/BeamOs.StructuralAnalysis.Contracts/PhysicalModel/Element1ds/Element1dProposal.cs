using System.Text.Json.Serialization;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

[JsonPolymorphic]
[JsonDerivedType(typeof(CreateElement1dProposal), typeDiscriminator: "Create")]
[JsonDerivedType(typeof(ModifyElement1dProposal), typeDiscriminator: "Modify")]
public abstract record Element1dProposal
{
    public ProposedID? StartNodeId { get; protected init; }
    public ProposedID? EndNodeId { get; protected init; }
    public ProposedID? MaterialId { get; protected init; }
    public ProposedID? SectionProfileId { get; protected init; }
    public Angle? SectionProfileRotation { get; protected init; } = new(0, AngleUnit.Degree);
    public Dictionary<string, string>? Metadata { get; protected init; }

    public static Element1dProposal Create(
        ProposedID startNodeId,
        ProposedID endNodeId,
        ProposedID materialId,
        ProposedID sectionProfileId,
        Angle? sectionProfileRotation = null,
        Dictionary<string, string>? metadata = null,
        int? id = null
    )
    {
        return new CreateElement1dProposal
        {
            Id = id,
            StartNodeId = startNodeId,
            EndNodeId = endNodeId,
            MaterialId = materialId,
            SectionProfileId = sectionProfileId,
            SectionProfileRotation = sectionProfileRotation ?? new(0, AngleUnit.Degree),
            Metadata = metadata,
        };
    }

    public static Element1dProposal Modify(
        int existingId,
        ProposedID? startNodeId = null,
        ProposedID? endNodeId = null,
        ProposedID? materialId = null,
        ProposedID? sectionProfileId = null,
        Angle? sectionProfileRotation = null,
        Dictionary<string, string>? metadata = null
    )
    {
        return new ModifyElement1dProposal
        {
            ExistingElement1dId = existingId,
            StartNodeId = startNodeId ?? default,
            EndNodeId = endNodeId ?? default,
            MaterialId = materialId ?? default,
            SectionProfileId = sectionProfileId ?? default,
            SectionProfileRotation = sectionProfileRotation ?? new(0, AngleUnit.Degree),
            Metadata = metadata,
        };
    }
}

public record ProposedID
{
    public int? ExistingId { get; private init; }
    public int? ProposedId { get; private init; }

    [JsonIgnore]
    public bool IsDefault => this.ExistingId is null && this.ProposedId is null;

    public static ProposedID Existing(int existingId) => new() { ExistingId = existingId };

    public static ProposedID Proposed(int proposedId) => new() { ProposedId = proposedId };

    [Obsolete("Deserialization constructor. Do not use.")]
    public ProposedID(int? existingId, int? proposedId)
    {
        ExistingId = existingId;
        ProposedId = proposedId;
    }

    public static ProposedID Default => new() { ExistingId = null, ProposedId = null };

    private ProposedID() { }
}

public record CreateElement1dProposal : Element1dProposal
{
    public int? Id { get; init; }
    public new required ProposedID StartNodeId
    {
        get => base.StartNodeId;
        init => base.StartNodeId = value;
    }
    public new required ProposedID EndNodeId
    {
        get => base.EndNodeId;
        init => base.EndNodeId = value;
    }
    public new required ProposedID MaterialId
    {
        get => base.MaterialId;
        init => base.MaterialId = value;
    }
    public new required ProposedID SectionProfileId
    {
        get => base.SectionProfileId;
        init => base.SectionProfileId = value;
    }
}

public record ModifyElement1dProposal : Element1dProposal
{
    public required int ExistingElement1dId { get; init; }
}
