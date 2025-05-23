using System.Diagnostics;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class Element1dExtendOrShortenRule : IModelRepairRule
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
        {
            return;
        }
        double dirX = dx / length;
        double dirY = dy / length;
        double dirZ = dz / length;

        foreach (Node originalNode in nearbyStartNodes)
        {
            Debug.Assert(
                originalNode.Id != startNode.Id,
                "Node ID should not be the same as start node ID"
            );

            var node = modelProposalBuilder.ApplyExistingProposal(originalNode, out _);

            double nx = node.LocationPoint.X.Meters;
            double ny = node.LocationPoint.Y.Meters;
            double nz = node.LocationPoint.Z.Meters;
            double t = (nx - ex) * dirX + (ny - ey) * dirY + (nz - ez) * dirZ;
            double projX = ex + t * dirX;
            double projY = ey + t * dirY;
            double projZ = ez + t * dirZ;
            double distToLine = Math.Sqrt(
                Math.Pow(nx - projX, 2) + Math.Pow(ny - projY, 2) + Math.Pow(nz - projZ, 2)
            );

            if (distToLine < tolerance && t <= 0)
            {
                var proposal = new Element1dProposal(
                    element1D,
                    modelProposalBuilder.Id,
                    startNodeId: node.Id
                );

                modelProposalBuilder.AddElement1dProposals(proposal);
            }
        }

        foreach (Node originalNode in nearbyEndNodes)
        {
            Debug.Assert(
                originalNode.Id != endNode.Id,
                "Node ID should not be the same as end node ID"
            );

            var node = modelProposalBuilder.ApplyExistingProposal(originalNode, out _);

            double nx = node.LocationPoint.X.Meters;
            double ny = node.LocationPoint.Y.Meters;
            double nz = node.LocationPoint.Z.Meters;
            double t = (nx - sx) * dirX + (ny - sy) * dirY + (nz - sz) * dirZ;
            double projX = sx + t * dirX;
            double projY = sy + t * dirY;
            double projZ = sz + t * dirZ;
            double distToLine = Math.Sqrt(
                Math.Pow(nx - projX, 2) + Math.Pow(ny - projY, 2) + Math.Pow(nz - projZ, 2)
            );

            if (distToLine < tolerance && t <= 0)
            {
                var proposal = new Element1dProposal(
                    element1D,
                    modelProposalBuilder.Id,
                    endNodeId: node.Id
                );

                modelProposalBuilder.AddElement1dProposals(proposal);
            }
        }
    }
}
