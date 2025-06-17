using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class NodeSnapToElement1dRule : IndividualNodeVisitingRule
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Standard;

    private static void SnapNodesToElements(
        Node node,
        Point nodeLocation,
        IList<Element1d> elements,
        ModelProposalBuilder modelProposalBuilder,
        double tolerance
    )
    {
        var nodePt = nodeLocation;
        double nx = nodePt.X.Meters;
        double ny = nodePt.Y.Meters;
        double nz = nodePt.Z.Meters;

        foreach (var elem in elements)
        {
            var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(elem, out _);
            var start = startNode.GetLocationPoint(
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            );
            var end = endNode.GetLocationPoint(
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            );

            double sx = start.X.Meters;
            double sy = start.Y.Meters;
            double sz = start.Z.Meters;
            double ex = end.X.Meters;
            double ey = end.Y.Meters;
            double ez = end.Z.Meters;
            double dx = ex - sx;
            double dy = ey - sy;
            double dz = ez - sz;
            double length = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (length < 1e-8)
                continue;
            double dirX = dx / length;
            double dirY = dy / length;
            double dirZ = dz / length;
            // Project node onto element line
            double t = (nx - sx) * dirX + (ny - sy) * dirY + (nz - sz) * dirZ;
            // Clamp t to [0,1] to stay within segment
            if (t is <= 0 or >= 1)
            {
                continue; // skip endpoints, already handled
            }
            double projX = sx + t * dirX;
            double projY = sy + t * dirY;
            double projZ = sz + t * dirZ;
            double distToLine = Math.Sqrt(
                Math.Pow(nx - projX, 2) + Math.Pow(ny - projY, 2) + Math.Pow(nz - projZ, 2)
            );
            if (distToLine < tolerance)
            {
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
                return;
            }
        }
    }

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
            return; // Only apply to Node types, not InternalNode
        }

        SnapNodesToElements(
            nodeAsNode,
            nodeLocation,
            nearbyElement1ds,
            modelProposalBuilder,
            tolerance.Meters
        );
    }
}
