using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Rules;

public class RemoveOrphanedNodeRule(ModelRepairContext context) : IModelRepairRule
{
    public ModelRepairRuleType RuleType => ModelRepairRuleType.Favorable;

    public void Apply()
    {
        var modelProposal = context.ModelProposalBuilder;
        HashSet<NodeId> usedNodeIds = [];
        foreach (Element1d element in modelProposal.Element1ds)
        {
            var (startNode, endNode) = modelProposal.GetStartAndEndNodes(element, out _);
            usedNodeIds.Add(startNode.Id);
            usedNodeIds.Add(endNode.Id);
        }

        var orphanedNodes = context
            .NodeStore.Values.Where(node => !usedNodeIds.Contains(node.Id))
            .ToList();

        foreach (var orphanedNode in orphanedNodes)
        {
            modelProposal.RemoveNode(orphanedNode);
        }
    }
}
