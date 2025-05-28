using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class NodeSnapToElement1dRule : IModelRepairRule
{
    public void Apply(
        Element1d element1D,
        Node startNode,
        Node endNode,
        IList<Node> nearbyStartNodes,
        IList<Element1d> element1DsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<Element1d> element1DsCloseToEnd,
        ModelProposalBuilder modelProposalBuilder,
        double tolerance
    )
    {
        SnapNodesToElements(startNode, element1DsCloseToStart, modelProposalBuilder, tolerance);
        SnapNodesToElements(endNode, element1DsCloseToEnd, modelProposalBuilder, tolerance);
    }

    private static void SnapNodesToElements(
        Node node,
        IList<Element1d> elements,
        ModelProposalBuilder modelProposalBuilder,
        double tolerance
    )
    {
        var nodePt = node.LocationPoint;
        double nx = nodePt.X.Meters;
        double ny = nodePt.Y.Meters;
        double nz = nodePt.Z.Meters;

        foreach (var elem in elements)
        {
            var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(elem, out _);
            var start = startNode.LocationPoint;
            var end = endNode.LocationPoint;

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
            double projX = sx + t * dirX;
            double projY = sy + t * dirY;
            double projZ = sz + t * dirZ;
            double distToLine = Math.Sqrt(
                Math.Pow(nx - projX, 2) + Math.Pow(ny - projY, 2) + Math.Pow(nz - projZ, 2)
            );
            if (distToLine < tolerance)
            {
                // Propose to move node to (projX, projY, projZ)
                modelProposalBuilder.AddNodeProposal(
                    new NodeProposal(
                        node,
                        modelProposalBuilder.Id,
                        new Common.Point(projX, projY, projZ, UnitsNet.Units.LengthUnit.Meter)
                    )
                );
            }
        }
    }
}
