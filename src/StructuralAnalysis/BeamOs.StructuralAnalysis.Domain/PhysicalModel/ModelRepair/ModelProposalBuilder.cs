using System.Diagnostics;
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
    private readonly IList<Element1d> element1ds;

    public ModelProposalBuilder(
        ModelId modelId,
        string name,
        string description,
        ModelSettings settings,
        Dictionary<int, Node> nodeIdToNodeDict,
        IList<Element1d> element1ds,
        ModelProposalId? id = null
    )
    {
        this.modelProposal = new(modelId, name, description, settings, id);
        this.nodeIdToNodeDict = nodeIdToNodeDict;
        this.element1ds = element1ds;
    }

    public ModelProposalId Id => this.modelProposal.Id;

    private readonly Dictionary<int, NodeProposal> modifyNodeProposalCache = [];
    private readonly Dictionary<int, int> mergedNodeToReplacedNodeIdDict = [];
    private readonly List<NodeProposal> newNodeProposals = [];

    public void MergeNodes(Node originalNode, Node targetNode)
    {
        Debug.Assert(
            originalNode.Id != targetNode.Id,
            "Original and target nodes should not be the same"
        );

        foreach (var element1d in this.element1ds)
        {
            var (startNode, endNode) = this.GetStartAndEndNodes(element1d, out _);
            if (startNode.Id == originalNode.Id)
            {
                this.AddElement1dProposals(
                    new Element1dProposal(element1d, this.Id, startNodeId: targetNode.Id)
                );
            }
            else if (endNode.Id == originalNode.Id)
            {
                this.AddElement1dProposals(
                    new Element1dProposal(element1d, this.Id, endNodeId: targetNode.Id)
                );
            }
        }

        this.mergedNodeToReplacedNodeIdDict[originalNode.Id] = targetNode.Id;
    }

    public void AddNodeProposal(NodeProposal proposal)
    {
        if (proposal.ExistingId is not null)
        {
            this.modifyNodeProposalCache[proposal.ExistingId.Value] = proposal;
        }
        else
        {
            this.newNodeProposals.Add(proposal);
        }
    }

    public Node ApplyExistingProposal(Node node, out bool isModifiedInProposal) =>
        this.ApplyExistingProposal(node.Id, out isModifiedInProposal);

    public Node ApplyExistingProposal(Node node) => this.ApplyExistingProposal(node.Id, out _);

    public Node ApplyExistingProposal(NodeId nodeId, out bool isModifiedInProposal)
    {
        if (this.mergedNodeToReplacedNodeIdDict.TryGetValue(nodeId.Id, out var replacedNodeId))
        {
            nodeId = replacedNodeId;
        }

        if (this.modifyNodeProposalCache.TryGetValue(nodeId, out var proposal))
        {
            isModifiedInProposal = true;
            return proposal.ToDomain();
        }

        isModifiedInProposal = false;
        return this.nodeIdToNodeDict[nodeId];
    }

    private readonly Dictionary<int, Element1dProposal> modifyElement1dProposalCache = [];
    private readonly List<Element1dProposal> newElement1dProposals = [];

    public void AddElement1dProposals(Element1dProposal proposal)
    {
        if (proposal.ExistingId is not null)
        {
            this.modifyElement1dProposalCache[proposal.ExistingId.Value] = proposal;
        }
        else
        {
            this.newElement1dProposals.Add(proposal);
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
            this.ApplyExistingProposal(modifiedElement1d.StartNodeId, out _),
            this.ApplyExistingProposal(modifiedElement1d.EndNodeId, out _)
        );
    }

    public (Node startNode, Node endNode) GetStartAndEndNodes(Element1d element1d) =>
        this.GetStartAndEndNodes(element1d, out _);

    public void AddMaterialProposals(MaterialProposal proposal) =>
        (this.modelProposal.MaterialProposals ??= []).Add(proposal);

    public void AddSectionProfileProposals(SectionProfileProposal proposal) =>
        (this.modelProposal.SectionProfileProposals ??= []).Add(proposal);

    public void AddSectionProfileProposalsFromLibrary(SectionProfileProposalFromLibrary proposal) =>
        (this.modelProposal.SectionProfileProposalsFromLibrary ??= []).Add(proposal);

    public void AddProposalIssues(ProposalIssue proposal) =>
        (this.modelProposal.ProposalIssues ??= []).Add(proposal);

    public ModelProposal Build()
    {
        this.modelProposal.NodeProposals =
        [
            .. this.modifyNodeProposalCache.Values,
            .. this.newNodeProposals,
        ];
        this.modelProposal.Element1dProposals =
        [
            .. this.modifyElement1dProposalCache.Values,
            .. this.newElement1dProposals,
        ];

        return this.modelProposal;
    }
}
