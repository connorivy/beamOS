using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

// [JsonPolymorphic]
// [JsonDerivedType(typeof(CreateElement1dProposal), typeDiscriminator: "Create")]
// [JsonDerivedType(typeof(ModifyElement1dProposal), typeDiscriminator: "Modify")]
public abstract record Element1dProposalBase
{
    public AngleContract? SectionProfileRotation { get; protected init; } =
        new(0, AngleUnitContract.Degree);
    public Dictionary<string, string>? Metadata { get; protected init; }

    public static CreateElement1dProposal Create(
        ProposedID startNodeId,
        ProposedID endNodeId,
        ProposedID materialId,
        ProposedID sectionProfileId,
        AngleContract? sectionProfileRotation = null,
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
            SectionProfileRotation = sectionProfileRotation ?? new(0, AngleUnitContract.Degree),
            Metadata = metadata,
        };
    }

    public static ModifyElement1dProposal Modify(
        int existingId,
        ProposedID? startNodeId = null,
        ProposedID? endNodeId = null,
        ProposedID? materialId = null,
        ProposedID? sectionProfileId = null,
        AngleContract? sectionProfileRotation = null,
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
            SectionProfileRotation = sectionProfileRotation ?? new(0, AngleUnitContract.Degree),
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

public record CreateElement1dProposal : Element1dProposalBase
{
    public int? Id { get; init; }
    public required ProposedID StartNodeId { get; init; }
    public required ProposedID EndNodeId { get; init; }
    public required ProposedID MaterialId { get; init; }
    public required ProposedID SectionProfileId { get; init; }
}

public record ModifyElement1dProposal : Element1dProposalBase
{
    public required int ExistingElement1dId { get; init; }
    public ProposedID? StartNodeId { get; init; }
    public ProposedID? EndNodeId { get; init; }
    public ProposedID? MaterialId { get; init; }
    public ProposedID? SectionProfileId { get; init; }
}

public record CreateElement1dProposalResponse : Element1dProposalBase, IHasIntId, IEntityProposal
{
    public int Id { get; init; }
    public required ProposedID StartNodeId { get; init; }
    public required ProposedID EndNodeId { get; init; }
    public required ProposedID MaterialId { get; init; }
    public required ProposedID SectionProfileId { get; init; }

    [JsonIgnore]
    public BeamOsObjectType ObjectType => BeamOsObjectType.Element1d;

    [JsonIgnore]
    public ProposalType ProposalType => ProposalType.Create;
}

public record ModifyElement1dProposalResponse
    : Element1dProposalBase,
        IHasIntId,
        IEntityModificationProposal
{
    public int Id { get; init; }
    public required int ExistingElement1dId { get; init; }
    public required ProposedID StartNodeId { get; init; }
    public required ProposedID EndNodeId { get; init; }
    public required ProposedID MaterialId { get; init; }
    public required ProposedID SectionProfileId { get; init; }

    public int ExistingId => this.ExistingElement1dId;

    public BeamOsObjectType ObjectType => BeamOsObjectType.Element1d;
}
