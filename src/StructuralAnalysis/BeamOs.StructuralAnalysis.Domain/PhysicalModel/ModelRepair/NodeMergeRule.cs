using System.Diagnostics;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class NodeMergeRule : IndividualNodeVisitingRule
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Unfavorable;

    protected override void ApplyToSingleNode(
        Element1d element,
        Node node,
        IList<Node> nearbyNodes,
        IList<Element1d> nearbyElement1ds,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        foreach (Node nearbyNode in nearbyNodes)
        {
            Debug.Assert(
                nearbyNode.Id != node.Id,
                "Candidate node should not be the same as base node"
            );

            // Calculate distance between nodes in meters
            Point p1 = node.LocationPoint;
            Point p2 = nearbyNode.LocationPoint;
            double dx = p1.X.Meters - p2.X.Meters;
            double dy = p1.Y.Meters - p2.Y.Meters;
            double dz = p1.Z.Meters - p2.Z.Meters;
            double distance = Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
            if (distance <= tolerance.Meters)
            {
                modelProposalBuilder.MergeNodes(node, nearbyNode);
            }
        }
    }
}
