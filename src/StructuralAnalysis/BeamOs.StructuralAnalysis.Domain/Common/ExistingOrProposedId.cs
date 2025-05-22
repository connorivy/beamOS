using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.Common;

public class ExistingOrProposedId<TId, TProposedId> : BeamOSValueObject
    where TId : struct, IIntBasedId
    where TProposedId : struct, IIntBasedId
{
    public TId? ExistingId { get; init; }
    public TProposedId? ProposedId { get; init; }

    protected ExistingOrProposedId(TId existingId)
    {
        this.ExistingId = existingId;
    }

    protected ExistingOrProposedId(TProposedId proposedId)
    {
        this.ProposedId = proposedId;
    }

    public TId ToDomain(Dictionary<TProposedId, TId> proposedIdToDomainIdDict)
    {
        if (this.ExistingId is not null)
        {
            return this.ExistingId.Value;
        }
        if (this.ProposedId is null)
        {
            throw new InvalidOperationException("Both ExistingId and ProposedId are null.");
        }
        if (!proposedIdToDomainIdDict.TryGetValue(this.ProposedId.Value, out var domainId))
        {
            throw new InvalidOperationException(
                $"ProposedId {this.ProposedId} not found in dictionary."
            );
        }
        return domainId;
    }

    public (TId id, TEntity? entity) ToIdAndEntity<TEntity>(
        Dictionary<TProposedId, TEntity> proposedIdToEntityDict
    )
        where TEntity : class
    {
        if (this.ExistingId is not null)
        {
            return (this.ExistingId.Value, null);
        }
        if (this.ProposedId is null)
        {
            throw new InvalidOperationException("Both ExistingId and ProposedId are null.");
        }
        if (!proposedIdToEntityDict.TryGetValue(this.ProposedId.Value, out var entity))
        {
            throw new InvalidOperationException(
                $"ProposedId {this.ProposedId} not found in dictionary."
            );
        }
        return (default, entity);
    }

    [Obsolete("EF Core Constructor", true)]
    protected ExistingOrProposedId(TId? existingId, TProposedId? proposedId)
        : base()
    {
        this.ExistingId = existingId;
        this.ProposedId = proposedId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ExistingId;
        yield return this.ProposedId;
    }
}

public sealed class ExisitingOrProposedGenericId
{
    public int? ExistingId { get; init; }
    public int? ProposedId { get; init; }

    public static ExisitingOrProposedGenericId FromExistingId(int existingId) =>
        new() { ExistingId = existingId };

    public static ExisitingOrProposedGenericId FromProposedId(int proposedId) =>
        new() { ProposedId = proposedId };

    private ExisitingOrProposedGenericId() { }

    [Obsolete("EF Core Constructor", true)]
    private ExisitingOrProposedGenericId(int? existingId, int? proposedId) { }
}

public sealed class ExistingOrProposedNodeId : ExistingOrProposedId<NodeId, NodeProposalId>
{
    public static implicit operator ExistingOrProposedNodeId(NodeId id) => new(id);

    public static implicit operator ExistingOrProposedNodeId(NodeProposalId id) => new(id);

    public ExistingOrProposedNodeId(NodeId existingId)
        : base(existingId) { }

    public ExistingOrProposedNodeId(NodeProposalId proposedId)
        : base(proposedId) { }

    [Obsolete("EF Core Constructor", true)]
    private ExistingOrProposedNodeId(NodeId? existingId, NodeProposalId? proposedId)
        : base(existingId, proposedId) { }
}

public sealed class ExistingOrProposedMaterialId
    : ExistingOrProposedId<MaterialId, MaterialProposalId>
{
    public ExistingOrProposedMaterialId(MaterialId existingId)
        : base(existingId) { }

    public ExistingOrProposedMaterialId(MaterialProposalId proposedId)
        : base(proposedId) { }

    [Obsolete("EF Core Constructor", true)]
    private ExistingOrProposedMaterialId(MaterialId? existingId, MaterialProposalId? proposedId)
        : base(existingId, proposedId) { }
}

public sealed class ExistingOrProposedSectionProfileId
    : ExistingOrProposedId<SectionProfileId, SectionProfileProposalId>
{
    public ExistingOrProposedSectionProfileId(SectionProfileId existingId)
        : base(existingId) { }

    public ExistingOrProposedSectionProfileId(SectionProfileProposalId proposedId)
        : base(proposedId) { }

    [Obsolete("EF Core Constructor", true)]
    private ExistingOrProposedSectionProfileId(
        SectionProfileId? existingId,
        SectionProfileProposalId? proposedId
    )
        : base(existingId, proposedId) { }
}

public interface IFromId<out TSelf, TId>
{
    public TSelf FromId(TId id);
}
