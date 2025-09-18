using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.Common;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

internal class DeleteModelEntityProposal : BeamOsModelEntity<DeleteModelEntityProposalId>
{
    public DeleteModelEntityProposal(
        ModelId modelId,
        ModelProposalId modelProposalId,
        int modelEntityId,
        BeamOsObjectType objectType
    )
        : base(new(modelEntityId), modelId)
    {
        this.ModelProposalId = modelProposalId;
        this.ObjectType = objectType;
    }

    public BeamOsObjectType ObjectType { get; private set; }
    public ModelProposalId ModelProposalId { get; private set; }
    public ModelProposal? ModelProposal { get; set; }

    [Obsolete("EF Core Constructor", true)]
    public DeleteModelEntityProposal() { }
}
