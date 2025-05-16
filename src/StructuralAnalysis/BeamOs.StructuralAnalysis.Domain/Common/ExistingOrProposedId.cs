using BeamOs.Common.Domain.Models;
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
