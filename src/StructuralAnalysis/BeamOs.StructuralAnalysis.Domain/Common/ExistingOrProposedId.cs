using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.Common;

public class ExistingOrProposedId<TId, TProposedId> : BeamOSValueObject
{
    public TId? ExistingId { get; }
    public TProposedId? ProposedId { get; }

    protected ExistingOrProposedId(TId existingId)
    {
        this.ExistingId = existingId;
    }

    protected ExistingOrProposedId(TProposedId proposedId)
    {
        this.ProposedId = proposedId;
    }

    public static implicit operator ExistingOrProposedId<TId, TProposedId>(TId id) => new(id);

    public static implicit operator ExistingOrProposedId<TId, TProposedId>(TProposedId id) =>
        new(id);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ExistingId;
        yield return this.ProposedId;
    }
}

public sealed class ExistingOrProposedNodeId : ExistingOrProposedId<NodeId, NodeProposalId>
{
    private ExistingOrProposedNodeId(NodeId existingId)
        : base(existingId) { }

    private ExistingOrProposedNodeId(NodeProposalId proposedId)
        : base(proposedId) { }
}

public sealed class ExistingOrProposedMaterialId
    : ExistingOrProposedId<MaterialId, MaterialProposalId>
{
    private ExistingOrProposedMaterialId(MaterialId existingId)
        : base(existingId) { }

    private ExistingOrProposedMaterialId(MaterialProposalId proposedId)
        : base(proposedId) { }
}

public sealed class ExistingOrProposedSectionProfileId
    : ExistingOrProposedId<SectionProfileId, SectionProfileProposalId>
{
    private ExistingOrProposedSectionProfileId(SectionProfileId existingId)
        : base(existingId) { }

    private ExistingOrProposedSectionProfileId(SectionProfileProposalId proposedId)
        : base(proposedId) { }
}
