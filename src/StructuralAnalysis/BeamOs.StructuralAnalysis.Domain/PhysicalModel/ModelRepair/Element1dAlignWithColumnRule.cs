using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class Element1dAlignWithColumnRule : BeamAndBraceVisitingRule
{
    protected override void ApplyToBothElementNodes(
        Element1d element1D,
        Node startNode,
        Node endNode,
        IList<Node> nearbyStartNodes,
        IEnumerable<Element1d> beamsAndBracesCloseToStart,
        IEnumerable<Element1d> columnsCloseToStart,
        IList<Node> nearbyEndNodes,
        IEnumerable<Element1d> beamsAndBracesCloseToEnd,
        IEnumerable<Element1d> columnsCloseToEnd,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        // Determine up axis
        bool yAxisUp = modelProposalBuilder.Settings.YAxisUp;
        Func<Point, Length> upSelector;
        if (yAxisUp)
        {
            upSelector = p => p.Y;
        }
        else
        {
            upSelector = p => p.Z;
        }

        // Helper to find closest column and its axis value
        (Element1d? column, double? axisValue) FindClosestColumn(
            IEnumerable<Element1d> columns,
            Point nodePt
        )
        {
            Element1d? closest = null;
            double minDist = double.MaxValue;
            double? axisVal = null;
            foreach (Element1d col in columns)
            {
                if (col.StartNode == null)
                {
                    continue;
                }
                Point colStart = col.StartNode.LocationPoint;
                double colAxis = upSelector(colStart).Meters;
                double dist = Math.Abs(upSelector(nodePt).Meters - colAxis);
                if (dist < minDist && dist < tolerance.Meters)
                {
                    minDist = dist;
                    closest = col;
                    axisVal = colAxis;
                }
            }
            return (closest, axisVal);
        }

        (Element1d? colStart, double? axisStart) = FindClosestColumn(
            columnsCloseToStart,
            startNode.LocationPoint
        );
        (Element1d? colEnd, double? axisEnd) = FindClosestColumn(
            columnsCloseToEnd,
            endNode.LocationPoint
        );

        // If neither end is near a column, do nothing
        if (axisStart == null && axisEnd == null)
        {
            return;
        }

        // If only one end is near a column, align both ends to that axis
        double? targetAxis = axisStart ?? axisEnd;
        if (targetAxis == null)
        {
            return;
        }

        // Calculate new points for both nodes, keeping the beam's direction
        Point startPt = startNode.LocationPoint;
        Point endPt = endNode.LocationPoint;
        Length dirX = endPt.X - startPt.X;
        Length dirY = endPt.Y - startPt.Y;
        Length dirZ = endPt.Z - startPt.Z;
        double length = Math.Sqrt(
            Math.Pow(dirX.Meters, 2) + Math.Pow(dirY.Meters, 2) + Math.Pow(dirZ.Meters, 2)
        );
        if (length < 1e-8)
        {
            return;
        }
        // Normalize direction
        double dx = dirX.Meters / length;
        double dy = dirY.Meters / length;
        double dz = dirZ.Meters / length;

        // Set new start and end points with the up axis snapped to targetAxis
        double startUp = upSelector(startPt).Meters;
        double endUp = upSelector(endPt).Meters;
        double deltaUp = endUp - startUp;
        double newStartUp = targetAxis.Value;
        double newEndUp = targetAxis.Value + deltaUp;

        Point newStart;
        Point newEnd;
        if (yAxisUp)
        {
            newStart = new Point(startPt.X, Length.FromMeters(newStartUp), startPt.Z);
            newEnd = new Point(endPt.X, Length.FromMeters(newEndUp), endPt.Z);
        }
        else
        {
            newStart = new Point(startPt.X, startPt.Y, Length.FromMeters(newStartUp));
            newEnd = new Point(endPt.X, endPt.Y, Length.FromMeters(newEndUp));
        }

        // Only propose if the move is significant
        if (startPt.CalculateDistance(newStart.X, newStart.Y, newStart.Z) > tolerance.Meters / 10)
        {
            modelProposalBuilder.AddNodeProposal(
                new NodeProposal(startNode, modelProposalBuilder.Id, newStart)
            );
        }
        if (endPt.CalculateDistance(newEnd.X, newEnd.Y, newEnd.Z) > tolerance.Meters / 10)
        {
            modelProposalBuilder.AddNodeProposal(
                new NodeProposal(endNode, modelProposalBuilder.Id, newEnd)
            );
        }
    }
}
