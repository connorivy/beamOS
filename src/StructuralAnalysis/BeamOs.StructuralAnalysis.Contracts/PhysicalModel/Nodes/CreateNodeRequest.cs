using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

public record CreateNodeRequest : NodeData
{
    [SetsRequiredMembers]
    public CreateNodeRequest(
        Point locationPoint,
        Restraint restraint,
        int? id = null,
        Dictionary<string, string>? metadata = null
    )
        : base(locationPoint, restraint, metadata)
    {
        this.Id = id;
    }

    [SetsRequiredMembers]
    public CreateNodeRequest(NodeData nodeData)
        : this(nodeData.LocationPoint, nodeData.Restraint, null, nodeData.Metadata) { }

    public CreateNodeRequest() { }

    public int? Id { get; init; }
}

public record NodeData
{
    [SetsRequiredMembers]
    public NodeData(
        Point locationPoint,
        Restraint restraint,
        Dictionary<string, string>? metadata = null
    )
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
        this.Metadata = metadata;
    }

    public NodeData() { }

    public required Point LocationPoint { get; init; }
    public required Restraint Restraint { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}

public record PutNodeRequest : NodeData, IHasIntId, IBeamOsEntityRequest
{
    public PutNodeRequest() { }

    [SetsRequiredMembers]
    public PutNodeRequest(
        int id,
        Point locationPoint,
        Restraint restraint,
        Dictionary<string, string>? metadata = null
    )
        : base(locationPoint, restraint, metadata)
    {
        this.Id = id;
    }

    public required int Id { get; init; }
}

// public record Node : NodeData, IHasIntId, IBeamOsEntityRequest
// {
//     public Node() { }

//     [SetsRequiredMembers]
//     public Node(
//         int id,
//         Point locationPoint,
//         Restraint restraint,
//         Dictionary<string, string>? metadata = null
//     )
//         : base(locationPoint, restraint, metadata)
//     {
//         this.Id = id;
//     }

//     public required int Id { get; init; }
// }

public record CreateNodeProposalResponse : NodeData, IEntityProposal
{
    public required int Id { get; init; }

    [JsonIgnore]
    public BeamOsObjectType ObjectType { get; } = BeamOsObjectType.Node;

    [JsonIgnore]
    public ProposalType ProposalType { get; protected init; } = ProposalType.Create;
}

public record CreateInternalNodeProposalResponse : IEntityProposal
{
    [SetsRequiredMembers]
    public CreateInternalNodeProposalResponse(
        ProposedID element1dId,
        Ratio ratioAlongElement1d,
        Restraint? restraint = null,
        Dictionary<string, string>? metadata = null
    )
    {
        this.Element1dId = element1dId;
        if (ratioAlongElement1d.As(RatioUnit.DecimalFraction) is < 0 or > 1)
        {
            throw new ArgumentException("Ratio along element must be between 0 and 1");
        }

        this.RatioAlongElement1d = ratioAlongElement1d;
        this.Metadata = metadata;
        this.Restraint = restraint;
    }

    public required ProposedID Element1dId { get; init; }
    public required Ratio RatioAlongElement1d { get; init; }
    public Restraint? Restraint { get; init; }

    // public InternalNodeData() { }

    public Dictionary<string, string>? Metadata { get; init; }

    [JsonIgnore]
    public BeamOsObjectType ObjectType => BeamOsObjectType.InternalNode;

    [JsonIgnore]
    public ProposalType ProposalType { get; protected init; } = ProposalType.Create;

    public required int Id { get; init; }
}

public record ModifyNodeProposalResponse : CreateNodeProposalResponse, IEntityModificationProposal
{
    public required int ExistingNodeId { get; init; }

    [JsonIgnore]
    public int ExistingId => this.ExistingNodeId;

    public ModifyNodeProposalResponse()
    {
        this.ProposalType = ProposalType.Modify;
    }
}

public record ModifyInternalNodeProposalResponse
    : CreateInternalNodeProposalResponse,
        IEntityModificationProposal
{
    [SetsRequiredMembers]
    public ModifyInternalNodeProposalResponse(
        int id,
        int existingInternalNodeId,
        ProposedID element1dId,
        Ratio ratioAlongElement1d,
        Restraint? restraint = null,
        Dictionary<string, string>? metadata = null
    )
        : base(element1dId, ratioAlongElement1d, restraint, metadata)
    {
        this.Id = id;
        this.ExistingInternalNodeId = existingInternalNodeId;
        this.ProposalType = ProposalType.Modify;
    }

    public required int ExistingInternalNodeId { get; init; }

    [JsonIgnore]
    public int ExistingId => this.ExistingInternalNodeId;
}

public interface IEntityProposal : IHasIntId
{
    [JsonIgnore]
    public BeamOsObjectType ObjectType { get; }

    [JsonIgnore]
    public ProposalType ProposalType { get; }
    public ModelEntityId ToModelEntityId() => new(this.ObjectType, this.Id);
}

public record EntityProposal(BeamOsObjectType ObjectType, int Id, ProposalType ProposalType)
    : IEntityProposal { }

public interface IEntityModificationProposal : IEntityProposal
{
    /// <summary>
    /// The ID of the existing object that this proposal modifies.
    /// </summary>
    public int ExistingId { get; }

    ProposalType IEntityProposal.ProposalType => ProposalType.Modify;
}

public enum ProposalType
{
    Undefined = 0,
    Create,
    Modify,
    Delete,
}

public readonly record struct ModelEntityId(BeamOsObjectType ObjectType, int Id);

public readonly record struct DeleteEntityProposalId(
    BeamOsObjectType EntityType,
    int ExistingEntityId
) { }
