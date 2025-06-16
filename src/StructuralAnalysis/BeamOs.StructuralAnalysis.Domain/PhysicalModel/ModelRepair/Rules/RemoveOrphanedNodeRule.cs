using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Rules;

public class RemoveOrphanedNodeRule : IModelRepairRule
{
    public ModelRepairRuleType RuleType => ModelRepairRuleType.Favorable;

    public void Apply(ModelProposalBuilder modelProposal, Length tolerance)
    {
        HashSet<NodeId> usedNodeIds = [];
        foreach (Element1d element in modelProposal.Element1ds)
        {
            var (startNode, endNode) = modelProposal.GetStartAndEndNodes(element, out _);
            usedNodeIds.Add(startNode.Id);
            usedNodeIds.Add(endNode.Id);
        }

        var orphanedNodes = modelProposal
            .Nodes.Where(node => !usedNodeIds.Contains(node.Id))
            .ToList();

        foreach (var orphanedNodeId in orphanedNodes)
        {
            modelProposal.RemoveNode(orphanedNodeId);
        }
    }
}
