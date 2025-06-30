using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.Common;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public sealed class DeleteModelEntityProposal : BeamOsModelEntity<ModelEntityDeleteProposalId>
{
    public DeleteModelEntityProposal(
        ModelId modelId,
        ModelProposalId modelProposalId,
        int modelEntityId,
        BeamOsObjectType objectType,
        ModelEntityDeleteProposalId? id = null
    )
        : base(id ?? default, modelId)
    {
        this.ModelProposalId = modelProposalId;
        this.ModelEntityId = modelEntityId;
        this.ObjectType = objectType;
    }

    public int ModelEntityId { get; private set; }
    public BeamOsObjectType ObjectType { get; private set; }
    public ModelProposalId ModelProposalId { get; private set; }
    public ModelProposal? ModelProposal { get; set; }

    [Obsolete("EF Core Constructor", true)]
    public DeleteModelEntityProposal() { }
}
