using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public sealed class ModelProposalNodeStore : IReadOnlyDictionary<NodeId, NodeDefinition>
{
    public ModelProposalNodeStore(IList<Node> nodes, IList<InternalNode> internalNodes)
    {
        this.nodeIdToNodeDict = nodes
            .Concat<NodeDefinition>(internalNodes)
            .ToDictionary(node => node.Id);
    }

    private readonly Dictionary<NodeId, NodeDefinition> nodeIdToNodeDict;
    private readonly Dictionary<NodeId, NodeId> mergedNodeToReplacedNodeIdDict = [];
    private readonly Dictionary<NodeId, NodeProposalBase> modifyNodeProposalCache = [];
    private readonly List<NodeProposalBase> newNodeProposals = [];

    public NodeDefinition this[NodeId key] =>
        this.ApplyExistingProposal(this.nodeIdToNodeDict[key]);

    public IEnumerable<NodeId> Keys => this.nodeIdToNodeDict.Keys;

    public IEnumerable<NodeDefinition> Values =>
        this.nodeIdToNodeDict.Values.Select(node => this.ApplyExistingProposal(node, out _));

    public int Count => this.nodeIdToNodeDict.Count;

    public bool ContainsKey(NodeId key) => this.nodeIdToNodeDict.ContainsKey(key);

    public bool TryGetValue(NodeId key, [MaybeNullWhen(false)] out NodeDefinition value)
    {
        if (this.nodeIdToNodeDict.TryGetValue(key, out var node))
        {
            value = this.ApplyExistingProposal(node, out _);
            return true;
        }

        value = default;
        return false;
    }

    public IEnumerator<KeyValuePair<NodeId, NodeDefinition>> GetEnumerator() =>
        this
            .nodeIdToNodeDict.Select(kvp => new KeyValuePair<NodeId, NodeDefinition>(
                kvp.Key,
                this.ApplyExistingProposal(kvp.Value, out _)
            ))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public NodeDefinition ApplyExistingProposal(
        NodeDefinition node,
        out bool isModifiedInProposal
    ) => this.ApplyExistingProposal(node.Id, out isModifiedInProposal);

    public NodeDefinition ApplyExistingProposal(NodeDefinition node) =>
        this.ApplyExistingProposal(node.Id, out _);

    public NodeDefinition ApplyExistingProposal(NodeId nodeId) =>
        this.ApplyExistingProposal(nodeId, out _);

    public NodeDefinition ApplyExistingProposal(NodeId nodeId, out bool isModifiedInProposal)
    {
        while (this.mergedNodeToReplacedNodeIdDict.TryGetValue(nodeId.Id, out var replacedNodeId))
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

    public void MergeNodes(NodeDefinition originalNode, NodeDefinition targetNode)
    {
        Debug.Assert(
            originalNode.Id != targetNode.Id,
            "Original and target nodes should not be the same"
        );
        this.mergedNodeToReplacedNodeIdDict[originalNode.Id] = targetNode.Id;
    }

    public void RemoveNode(NodeDefinition node)
    {
        this.modifyNodeProposalCache.Remove(node.Id);
    }

    public void AddNodeProposal(NodeProposalBase proposal)
    {
        if (proposal.ExistingId is not null)
        {
            this.modifyNodeProposalCache[proposal.ExistingId.Value] = proposal;
        }
        else
        {
            // todo: add these to the octree for discoverability
            this.newNodeProposals.Add(proposal);
        }
    }

    public IEnumerable<NodeProposal> GetNodeProposals() =>
        this.newNodeProposals.Concat(this.modifyNodeProposalCache.Values).OfType<NodeProposal>();

    public IEnumerable<InternalNodeProposal> GetInternalNodeProposals() =>
        this
            .newNodeProposals.Concat(this.modifyNodeProposalCache.Values)
            .OfType<InternalNodeProposal>();
}
