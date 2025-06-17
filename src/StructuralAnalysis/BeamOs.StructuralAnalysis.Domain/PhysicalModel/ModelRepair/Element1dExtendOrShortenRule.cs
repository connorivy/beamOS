using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class Element1dExtendOrShortenRule : IndividualNodeVisitingRule
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Standard;

    protected override void ApplyToSingleNode(
        Element1d element,
        NodeDefinition node,
        Point nodeLocation,
        IList<Node> nearbyNodes,
        IList<InternalNode> nearbyInternalNodes,
        IList<Element1d> nearbyElement1ds,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        if (node is not Node nodeAsNode)
        {
            return; // Only apply to Node
        }

        if (SnapToNearbyNode(element, nodeAsNode, nearbyNodes, modelProposalBuilder, tolerance))
        {
            return;
        }
        _ = SnapToNearbyElement1d(nodeAsNode, nearbyElement1ds, modelProposalBuilder, tolerance);
    }

    private static bool SnapToNearbyElement1d(
        Node node,
        IList<Element1d> element1DsCloseToNode,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        foreach (Element1d elem in element1DsCloseToNode)
        {
            var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(elem);

            Point p = node.LocationPoint;
            Point a = startNode.GetLocationPoint(
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            );
            Point b = endNode.GetLocationPoint(
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            );
            // Vector AB
            double abX = b.X.Meters - a.X.Meters;
            double abY = b.Y.Meters - a.Y.Meters;
            double abZ = b.Z.Meters - a.Z.Meters;
            // Vector AP
            double apX = p.X.Meters - a.X.Meters;
            double apY = p.Y.Meters - a.Y.Meters;
            double apZ = p.Z.Meters - a.Z.Meters;
            double abLen2 = (abX * abX) + (abY * abY) + (abZ * abZ);
            if (abLen2 == 0)
            {
                continue;
            }
            double t = ((abX * apX) + (abY * apY) + (abZ * apZ)) / abLen2;
            // Clamp t to [0,1] to stay within segment
            if (t is <= 0 or >= 1)
            {
                continue; // skip endpoints, already handled
            }
            // Calculate the percentage distance (t) from the start node to the projected point
            // t is already calculated above and clamped to (0,1)
            double distToLine = Math.Sqrt(
                Math.Pow(p.X.Meters - (a.X.Meters + (t * abX)), 2)
                    + Math.Pow(p.Y.Meters - (a.Y.Meters + (t * abY)), 2)
                    + Math.Pow(p.Z.Meters - (a.Z.Meters + (t * abZ)), 2)
            );
            if (distToLine < tolerance.Meters)
            {
                // Calculate the percentage distance (t) from the start node to the projected point
                // t is already calculated above and clamped to (0,1)
                // Create an InternalNodeProposal using the correct constructor
                modelProposalBuilder.NodeStore.AddInternalNodeProposal(
                    new InternalNodeProposal(
                        node.ModelId,
                        modelProposalBuilder.Id,
                        Ratio.FromDecimalFractions(t), // Ratio along the element
                        elem.Id, // Element1dId
                        node.Restraint, // Use the node's restraint
                        node.Id
                    )
                );
                return true;
            }
        }
        return false;
    }

    private static bool SnapToNearbyNode(
        Element1d element,
        Node node,
        IList<Node> nearbyNodes,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        // Get the start and end nodes of the element
        var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(element);
        // Determine which node is fixed and which is being considered for snapping
        var fixedNode = node == startNode ? endNode : startNode;
        Point fixedPoint = fixedNode.GetLocationPoint(
            modelProposalBuilder.Element1dStore,
            modelProposalBuilder.NodeStore
        );
        Point nodePoint = node.GetLocationPoint(
            modelProposalBuilder.Element1dStore,
            modelProposalBuilder.NodeStore
        );
        // Direction vector of the element (from fixedNode to node)
        double dirX = nodePoint.X.Meters - fixedPoint.X.Meters;
        double dirY = nodePoint.Y.Meters - fixedPoint.Y.Meters;
        double dirZ = nodePoint.Z.Meters - fixedPoint.Z.Meters;
        double dirLen = Math.Sqrt((dirX * dirX) + (dirY * dirY) + (dirZ * dirZ));
        if (dirLen == 0)
        {
            return false;
        }
        // Normalize direction
        double normDirX = dirX / dirLen;
        double normDirY = dirY / dirLen;
        double normDirZ = dirZ / dirLen;

        var rotationalTolerance = tolerance.Meters * 0.0001;

        foreach (Node candidate in nearbyNodes)
        {
            if (candidate == node)
            {
                continue;
            }
            Point candidatePoint = candidate.LocationPoint;
            // Vector from fixedNode to candidate
            double candX = candidatePoint.X.Meters - fixedPoint.X.Meters;
            double candY = candidatePoint.Y.Meters - fixedPoint.Y.Meters;
            double candZ = candidatePoint.Z.Meters - fixedPoint.Z.Meters;
            double candLen = Math.Sqrt((candX * candX) + (candY * candY) + (candZ * candZ));
            if (candLen == 0)
            {
                continue;
            }
            // Normalize
            double normCandX = candX / candLen;
            double normCandY = candY / candLen;
            double normCandZ = candZ / candLen;
            // Check if direction matches (dot product close to 1 or -1)
            double dot = (normDirX * normCandX) + (normDirY * normCandY) + (normDirZ * normCandZ);
            if (Math.Abs(dot - 1.0) < rotationalTolerance) // same direction
            {
                // Check if candidate is within tolerance of the line extended from fixedNode
                double dist = Math.Abs(candLen - dirLen);
                if (dist < tolerance.Meters)
                {
                    modelProposalBuilder.MergeNodes(node, candidate);
                    return true;
                }
            }
            else if (Math.Abs(dot + 1.0) < rotationalTolerance) // opposite direction (should not happen for 1d element)
            {
                continue;
            }
        }
        return false;
    }
}
