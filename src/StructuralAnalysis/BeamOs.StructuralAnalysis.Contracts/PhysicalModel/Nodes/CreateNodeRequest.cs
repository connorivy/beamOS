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
