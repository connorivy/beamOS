using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public sealed class ModelProposalBuilder
{
    private readonly ModelProposal modelProposal;

    public ModelProposalBuilder(
        ModelId modelId,
        string name,
        string description,
        ModelSettings settings,
        ModelProposalId? id = null
    )
    {
        this.modelProposal = new(modelId, name, description, settings, id);
    }

    private HashSet<int> nodeProposalCache = [];

    public bool IsNodeProposalCached(int nodeId) => this.nodeProposalCache.Contains(nodeId);

    public void AddNodeProposals(NodeProposal proposal)
    {
        (this.modelProposal.NodeProposals ??= []).Add(proposal);
        if (proposal.ExistingId is not null)
        {
            this.nodeProposalCache.Add(proposal.ExistingId.Value);
        }
    }

    public void AddElement1dProposals(Element1dProposal proposal) =>
        (this.modelProposal.Element1dProposals ??= []).Add(proposal);

    public void AddMaterialProposals(MaterialProposal proposal) =>
        (this.modelProposal.MaterialProposals ??= []).Add(proposal);

    public void AddSectionProfileProposals(SectionProfileProposal proposal) =>
        (this.modelProposal.SectionProfileProposals ??= []).Add(proposal);

    public void AddSectionProfileProposalsFromLibrary(SectionProfileProposalFromLibrary proposal) =>
        (this.modelProposal.SectionProfileProposalsFromLibrary ??= []).Add(proposal);

    public void AddProposalIssues(ProposalIssue proposal) =>
        (this.modelProposal.ProposalIssues ??= []).Add(proposal);

    public ModelProposal Build() => this.modelProposal;
}
