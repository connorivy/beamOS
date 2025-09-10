using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class NodeSnapToElement1dRule(ModelRepairContext context)
    : IndividualNodeVisitingRule(context)
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Standard;

    private void SnapNodesToElements(
        Node node,
        Point nodeLocation,
        IList<Element1d> elements,
        double tolerance
    )
    {
        var nodePt = nodeLocation;
        double nx = nodePt.X.Meters;
        double ny = nodePt.Y.Meters;
        double nz = nodePt.Z.Meters;
        var modelProposalBuilder = this.Context.ModelProposalBuilder;

        foreach (var elem in elements)
        {
            var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(elem, out _);
            var start = startNode.GetLocationPoint(
                this.Context.Element1dStore,
                this.Context.NodeStore
            );
            var end = endNode.GetLocationPoint(this.Context.Element1dStore, this.Context.NodeStore);
            if (
                startNode.DependsOnNode(
                    node.Id,
                    this.Context.Element1dStore,
                    this.Context.NodeStore
                )
                || endNode.DependsOnNode(
                    node.Id,
                    this.Context.Element1dStore,
                    this.Context.NodeStore
                )
            )
            {
                // skip elements that depend on the current node to avoid cycles
                continue;
            }

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
                modelProposalBuilder.AddNodeProposal(
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
        Length tolerance
    )
    {
        if (node is not Node nodeAsNode)
        {
            return; // Only apply to Node types, not InternalNode
        }

        this.SnapNodesToElements(nodeAsNode, nodeLocation, nearbyElement1ds, tolerance.Meters);
    }
}
