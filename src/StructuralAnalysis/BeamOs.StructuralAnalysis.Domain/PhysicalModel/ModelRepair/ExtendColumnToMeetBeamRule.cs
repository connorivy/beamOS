using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class ExtendColumnToMeetBeamRule : ColumnVisitingRule
{
    protected override void ApplyRuleForColumn(
        Element1d column,
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
        // Determine up direction
        bool yAxisUp = modelProposalBuilder.Settings.YAxisUp;
        Point colStart = startNode.LocationPoint;
        Point colEnd = endNode.LocationPoint;

        Node colBottomNode;
        Node colTopNode;
        Point colBottom;
        Point colTop;
        if (
            (yAxisUp && colStart.Y.Meters < colEnd.Y.Meters)
            || (!yAxisUp && colStart.Z.Meters < colEnd.Z.Meters)
        )
        {
            colBottomNode = startNode;
            colBottom = colStart;
            colTopNode = endNode;
            colTop = colEnd;
        }
        else
        {
            colBottomNode = endNode;
            colBottom = colEnd;
            colTopNode = startNode;
            colTop = colStart;
        }

        // Pass colStart and colEnd to CheckAndExtend
        CheckAndExtend(
            colTopNode,
            colTop,
            true,
            beamsAndBracesCloseToEnd,
            modelProposalBuilder,
            tolerance,
            yAxisUp,
            colStart,
            colEnd
        );
        CheckAndExtend(
            colBottomNode,
            colBottom,
            false,
            beamsAndBracesCloseToStart,
            modelProposalBuilder,
            tolerance,
            yAxisUp,
            colStart,
            colEnd
        );
    }

    private static void CheckAndExtend(
        Node colNodeObj,
        Point colNode,
        bool isTop,
        IEnumerable<Element1d> nearbyBeams,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance,
        bool yAxisUp,
        Point colStart,
        Point colEnd
    )
    {
        double verticalTolerance = tolerance.Meters; // Allowable vertical gap
        foreach (Element1d beam in nearbyBeams)
        {
            (Node beamStart, Node beamEnd) = modelProposalBuilder.GetStartAndEndNodes(beam);
            Point bStart = beamStart.LocationPoint;
            Point bEnd = beamEnd.LocationPoint;

            // Project beam to up/down direction
            double colUp = yAxisUp ? colNode.Y.Meters : colNode.Z.Meters;
            double bStartUp = yAxisUp ? bStart.Y.Meters : bStart.Z.Meters;
            double bEndUp = yAxisUp ? bEnd.Y.Meters : bEnd.Z.Meters;

            // --- New plane comparison logic ---
            double distStart =
                Math.Pow(bStart.X.Meters - colStart.X.Meters, 2)
                + Math.Pow(bStart.Y.Meters - colStart.Y.Meters, 2)
                + Math.Pow(bStart.Z.Meters - colStart.Z.Meters, 2);
            double distEnd =
                Math.Pow(bEnd.X.Meters - colStart.X.Meters, 2)
                + Math.Pow(bEnd.Y.Meters - colStart.Y.Meters, 2)
                + Math.Pow(bEnd.Z.Meters - colStart.Z.Meters, 2);
            Point furthestBeamNode = distStart > distEnd ? bStart : bEnd;
            Point upPoint;
            if (yAxisUp)
            {
                upPoint = new Point(
                    bStart.X,
                    new Length(bStart.Y.Meters + 1.0, LengthUnit.Meter),
                    bStart.Z
                );
            }
            else
            {
                upPoint = new Point(
                    bStart.X,
                    bStart.Y,
                    new Length(bStart.Z.Meters + 1.0, LengthUnit.Meter)
                );
            }
            static (double X, double Y, double Z) PlaneNormal(Point a, Point b, Point c)
            {
                double ax = b.X.Meters - a.X.Meters;
                double ay = b.Y.Meters - a.Y.Meters;
                double az = b.Z.Meters - a.Z.Meters;
                double bx = c.X.Meters - a.X.Meters;
                double by = c.Y.Meters - a.Y.Meters;
                double bz = c.Z.Meters - a.Z.Meters;
                double nx = (ay * bz) - (az * by);
                double ny = (az * bx) - (ax * bz);
                double nz = (ax * by) - (ay * bx);
                double len = Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                return len > 0 ? (nx / len, ny / len, nz / len) : (0, 0, 0);
            }
            (double n1x, double n1y, double n1z) = PlaneNormal(colStart, colEnd, furthestBeamNode);
            (double n2x, double n2y, double n2z) = PlaneNormal(bStart, bEnd, upPoint);
            double dot = (n1x * n2x) + (n1y * n2y) + (n1z * n2z);
            bool inSamePlane = Math.Abs(Math.Abs(dot) - 1.0) < 1e-3; // Tolerance for coplanarity
            // --- End new plane comparison logic ---

            if (!inSamePlane)
            {
                continue;
            }

            // Check if beam is directly over/under (any node exactly above/below column node)
            bool isDirectlyAboveOrBelow = isTop
                ? (bStartUp > colUp - tolerance.Meters && bEndUp > colUp - tolerance.Meters)
                : (bStartUp < colUp + tolerance.Meters && bEndUp < colUp + tolerance.Meters);
            if (isDirectlyAboveOrBelow)
            {
                double targetUp = isTop ? Math.Min(bStartUp, bEndUp) : Math.Max(bStartUp, bEndUp);
                Point newColNode = yAxisUp
                    ? new Point(colNode.X, new Length(targetUp, LengthUnit.Meter), colNode.Z)
                    : new Point(colNode.X, colNode.Y, new Length(targetUp, LengthUnit.Meter));
                NodeProposal colNodeProposal = new NodeProposal(
                    colNodeObj,
                    modelProposalBuilder.Id,
                    newColNode
                );
                modelProposalBuilder.AddNodeProposal(colNodeProposal);
                return;
            }

            // Check if beam is almost over/under (in plane, but up coordinate is within verticalTolerance)
            double closestBeamUp = isTop ? Math.Min(bStartUp, bEndUp) : Math.Max(bStartUp, bEndUp);
            double verticalGap = Math.Abs(closestBeamUp - colUp);
            if (verticalGap <= verticalTolerance)
            {
                // Move column node to meet beam in up direction
                Point newColNode = yAxisUp
                    ? new Point(colNode.X, new Length(closestBeamUp, LengthUnit.Meter), colNode.Z)
                    : new Point(colNode.X, colNode.Y, new Length(closestBeamUp, LengthUnit.Meter));
                NodeProposal colNodeProposal = new NodeProposal(
                    colNodeObj,
                    modelProposalBuilder.Id,
                    newColNode
                );
                modelProposalBuilder.AddNodeProposal(colNodeProposal);
                // Move beam node to meet column in up direction (not in plan)
                Node beamNodeToMove = isTop
                    ? (bStartUp < bEndUp ? beamStart : beamEnd)
                    : (bStartUp > bEndUp ? beamStart : beamEnd);
                Point newBeamNode = yAxisUp
                    ? new Point(colNode.X, new Length(closestBeamUp, LengthUnit.Meter), colNode.Z)
                    : new Point(colNode.X, colNode.Y, new Length(closestBeamUp, LengthUnit.Meter));
                NodeProposal beamNodeProposal = new NodeProposal(
                    beamNodeToMove,
                    modelProposalBuilder.Id,
                    newBeamNode
                );
                modelProposalBuilder.AddNodeProposal(beamNodeProposal);
                return;
            }
        }
    }
}
