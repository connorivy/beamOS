using System.Diagnostics;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class NodeMergeRule(ModelRepairContext context) : IndividualNodeVisitingRule(context)
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Unfavorable;

    protected override void ApplyToSingleNode(
        Element1d element,
        NodeDefinition node,
        Point nodeLocation,
        IList<Node> nearbyNodes,
        IList<InternalNode> nearbyInternalNodes,
        IList<Element1d> nearbyElement1ds,
        Length tolerance
    )
    {
        var modelProposalBuilder = this.Context.ModelProposalBuilder;
        var nearestNodeWithinTolerance = nearbyNodes
            .Concat<NodeDefinition>(nearbyInternalNodes)
            .Select(definition =>
                (definition, ShortestDistanceTo(nodeLocation, definition, modelProposalBuilder))
            )
            .Where(tuple => tuple.Item2 < tolerance)
            .OrderBy(tuple => tuple.Item2)
            .Select(tuple => tuple.definition)
            .FirstOrDefault();

        if (nearestNodeWithinTolerance is null)
        {
            // No nearby node found within tolerance, nothing to merge
            return;
        }

        Debug.Assert(
            nearestNodeWithinTolerance.Id != node.Id,
            "Candidate node should not be the same as base node"
        );

        if (
            !nearestNodeWithinTolerance.DependsOnNode(
                node.Id,
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            )
        )
        {
            // If the nearest node does not depend on the current node, merge them
            modelProposalBuilder.MergeNodes(node, nearestNodeWithinTolerance);
        }
        else if (
            !node.DependsOnNode(
                nearestNodeWithinTolerance.Id,
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            )
        )
        {
            modelProposalBuilder.MergeNodes(nearestNodeWithinTolerance, node);
        }
    }

    private static Length ShortestDistanceTo(
        Point nodeLocation,
        NodeDefinition nodeDefinition,
        ModelProposalBuilder modelProposalBuilder
    )
    {
        return nodeLocation.CalculateDistance(
            nodeDefinition.GetLocationPoint(
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            )
        );
    }
}
