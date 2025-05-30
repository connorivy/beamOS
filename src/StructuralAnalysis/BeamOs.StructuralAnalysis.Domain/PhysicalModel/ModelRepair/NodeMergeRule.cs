using System.Diagnostics;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class NodeMergeRule : IModelRepairRule
{
    public void ApplyToBothElementNodes(
        Element1d element1D,
        Node startNode,
        Node endNode,
        IList<Node> nearbyStartNodes,
        IList<Element1d> element1DsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<Element1d> element1DsCloseToEnd,
        ModelProposalBuilder modelProposal,
        Length tolerance
    )
    {
        this.ApplyToSingleElementNode(
            element1D,
            startNode,
            nearbyStartNodes,
            element1DsCloseToStart,
            modelProposal,
            tolerance
        );
        this.ApplyToSingleElementNode(
            element1D,
            endNode,
            nearbyEndNodes,
            element1DsCloseToEnd,
            modelProposal,
            tolerance
        );
    }

    public void ApplyToSingleElementNode(
        Element1d element1d,
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
