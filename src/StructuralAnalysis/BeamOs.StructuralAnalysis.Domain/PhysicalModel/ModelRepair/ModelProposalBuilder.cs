using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public sealed class ModelProposalBuilder
{
    private readonly ModelProposal modelProposal;
    private readonly Dictionary<int, Node> nodeIdToNodeDict;

    public ModelProposalBuilder(
        ModelId modelId,
        string name,
        string description,
        ModelSettings settings,
        Dictionary<int, Node> nodeIdToNodeDict,
        ModelProposalId? id = null
    )
    {
        this.modelProposal = new(modelId, name, description, settings, id);
        this.nodeIdToNodeDict = nodeIdToNodeDict;
    }

    public ModelProposalId Id => this.modelProposal.Id;

    private readonly Dictionary<int, NodeProposal> modifyNodeProposalCache = [];

    public void AddNodeProposal(NodeProposal proposal)
    {
        (this.modelProposal.NodeProposals ??= []).Add(proposal);
        if (proposal.ExistingId is not null)
        {
            this.modifyNodeProposalCache[proposal.ExistingId.Value] = proposal;
        }
    }

    public Node ApplyExistingProposal(Node node, out bool isModifiedInProposal)
    {
        if (this.modifyNodeProposalCache.TryGetValue(node.Id, out var proposal))
        {
            isModifiedInProposal = true;
            return proposal.ToDomain();
        }

        isModifiedInProposal = false;
        return node;
    }

    private readonly Dictionary<int, Element1dProposal> modifyElement1dProposalCache = [];

    public void AddElement1dProposals(Element1dProposal proposal)
    {
        (this.modelProposal.Element1dProposals ??= []).Add(proposal);
        if (proposal.ExistingId is not null)
        {
            this.modifyElement1dProposalCache[proposal.ExistingId.Value] = proposal;
        }
    }

    public Element1d ApplyExistingProposal(Element1d element1d, out bool isModifiedInProposal)
    {
        if (this.modifyElement1dProposalCache.TryGetValue(element1d.Id, out var proposal))
        {
            isModifiedInProposal = true;
            return proposal.ToDomain(null, null, null);
        }

        isModifiedInProposal = false;
        return element1d;
    }

    public (Node startNode, Node endNode) GetStartAndEndNodes(
        Element1d element1d,
        out bool isModifiedInProposal
    )
    {
        var modifiedElement1d = this.ApplyExistingProposal(element1d, out isModifiedInProposal);

        return (
            this.nodeIdToNodeDict[modifiedElement1d.StartNodeId],
            this.nodeIdToNodeDict[modifiedElement1d.EndNodeId]
        );
    }

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
