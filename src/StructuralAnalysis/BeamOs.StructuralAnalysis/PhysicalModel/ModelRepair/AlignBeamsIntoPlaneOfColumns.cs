using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class AlignBeamsIntoPlaneOfColumns(ModelRepairContext context)
    : BeamOrBraceVisitingRule(context)
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Standard;

    protected override void ApplyRuleForBeamOrBrace(
        Element1d element1D,
        NodeDefinition startNode,
        NodeDefinition endNode,
        Point startNodeLocation,
        Point endNodeLocation,
        IList<Node> nearbyStartNodes,
        IList<InternalNode> nearbyStartInternalNodes,
        IEnumerable<Element1d> beamsAndBracesCloseToStart,
        IEnumerable<Element1d> columnsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<InternalNode> nearbyEndInternalNodes,
        IEnumerable<Element1d> beamsAndBracesCloseToEnd,
        IEnumerable<Element1d> columnsCloseToEnd,
        Length tolerance
    )
    {
        if (startNode is not Node startNodeAsNode || endNode is not Node endNodeAsNode)
        {
            // If either start or end node is not a Node, skip this element
            return;
        }
        this.ApplyRuleForBeamOrBrace(
            element1D,
            startNodeAsNode,
            endNodeAsNode,
            columnsCloseToStart,
            columnsCloseToEnd,
            tolerance
        );
    }

    protected void ApplyRuleForBeamOrBrace(
        Element1d element1D,
        Node startNode,
        Node endNode,
        IEnumerable<Element1d> columnsCloseToStart,
        IEnumerable<Element1d> columnsCloseToEnd,
        Length tolerance
    )
    {
        // Try every combination of a column near the start and a column near the end
        foreach (Element1d colStart in columnsCloseToStart)
        {
            foreach (Element1d colEnd in columnsCloseToEnd)
            {
                var (startNodeA, endNodeA) = this.Context.ModelProposalBuilder.GetStartAndEndNodes(
                    colStart,
                    out _
                );
                var (startNodeB, endNodeB) = this.Context.ModelProposalBuilder.GetStartAndEndNodes(
                    colEnd,
                    out _
                );

                Point colStartA = startNodeA.GetLocationPoint(
                    this.Context.ModelProposalBuilder.Element1dStore,
                    this.Context.ModelProposalBuilder.NodeStore
                );
                Point colStartB = startNodeB.GetLocationPoint(
                    this.Context.ModelProposalBuilder.Element1dStore,
                    this.Context.ModelProposalBuilder.NodeStore
                );
                Point colEndA = endNodeA.GetLocationPoint(
                    this.Context.ModelProposalBuilder.Element1dStore,
                    this.Context.ModelProposalBuilder.NodeStore
                );
                Point colEndB = endNodeB.GetLocationPoint(
                    this.Context.ModelProposalBuilder.Element1dStore,
                    this.Context.ModelProposalBuilder.NodeStore
                );

                if (
                    !IsPlaneOfElement1dParallelToPlaneOfColumns(
                        startNode.LocationPoint,
                        endNode.LocationPoint,
                        colStartA,
                        colEndA,
                        colStartB,
                        tolerance,
                        this.Context.ModelProposalBuilder.Settings.YAxisUp
                    )
                )
                {
                    // If the planes are not parallel, skip this combination
                    continue;
                }

                // Project startNode and endNode onto the plane defined by colStartA, colEndA, colStartB
                Point planeOrigin = colStartA;
                // Plane normal: use the same as in IsPlaneOfElement1dParallelToPlaneOfColumns
                double[] c1 =
                {
                    colEndA.X.Meters - colStartA.X.Meters,
                    colEndA.Y.Meters - colStartA.Y.Meters,
                    colEndA.Z.Meters - colStartA.Z.Meters,
                };
                double[] c2 =
                {
                    colStartB.X.Meters - colStartA.X.Meters,
                    colStartB.Y.Meters - colStartA.Y.Meters,
                    colStartB.Z.Meters - colStartA.Z.Meters,
                };
                double[] n =
                {
                    (c1[1] * c2[2]) - (c1[2] * c2[1]),
                    (c1[2] * c2[0]) - (c1[0] * c2[2]),
                    (c1[0] * c2[1]) - (c1[1] * c2[0]),
                };
                double nLen = Math.Sqrt((n[0] * n[0]) + (n[1] * n[1]) + (n[2] * n[2]));
                if (nLen < 1e-9)
                {
                    continue; // skip degenerate plane
                }
                for (int i = 0; i < 3; i++)
                {
                    n[i] /= nLen;
                }

                // Helper to project a point onto the plane
                Point ProjectOntoPlane(Point p)
                {
                    double[] v =
                    {
                        p.X.Meters - planeOrigin.X.Meters,
                        p.Y.Meters - planeOrigin.Y.Meters,
                        p.Z.Meters - planeOrigin.Z.Meters,
                    };
                    double dist = (v[0] * n[0]) + (v[1] * n[1]) + (v[2] * n[2]);
                    return new Point(
                        p.X.Meters - (dist * n[0]),
                        p.Y.Meters - (dist * n[1]),
                        p.Z.Meters - (dist * n[2]),
                        LengthUnit.Meter
                    );
                }

                Point projectedStart = ProjectOntoPlane(startNode.LocationPoint);
                Point projectedEnd = ProjectOntoPlane(endNode.LocationPoint);

                // Propose node moves by creating NodeProposal for each
                // Use the existing node's restraint
                NodeProposal startNodeProposal = new NodeProposal(
                    startNode,
                    this.Context.ModelProposalBuilder.Id,
                    projectedStart
                );
                NodeProposal endNodeProposal = new NodeProposal(
                    endNode,
                    this.Context.ModelProposalBuilder.Id,
                    projectedEnd
                );
                this.Context.ModelProposalBuilder.AddNodeProposals(
                    startNodeProposal,
                    endNodeProposal
                );
            }
        }
    }

    /// <summary>
    /// This method checks if the plane defined by the start and end points of the element and a point directly
    /// above the elment is parallel to the plane defined by the two columns.
    /// </summary>
    /// <param name="elementStart"></param>
    /// <param name="elementEnd"></param>
    /// <param name="column1Start"></param>
    /// <param name="column1End"></param>
    /// <param name="column2Start"></param>
    /// <param name="column2End"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public static bool IsPlaneOfElement1dParallelToPlaneOfColumns(
        Point elementStart,
        Point elementEnd,
        Point column1Start,
        Point column1End,
        Point column2Start,
        Length tolerance,
        bool yAxisUp = true
    )
    {
        // 1. Define a third point above the element (using Y or Z axis)
        double ux = 0;
        double uy = 0;
        double uz = 0;
        if (yAxisUp)
        {
            uy = 1;
        }
        else
        {
            uz = 1;
        }
        // Pick a point above elementStart
        double aboveScale = 1.0; // 1 meter above
        Point elementAbove = new(
            elementStart.X.Meters + (ux * aboveScale),
            elementStart.Y.Meters + (uy * aboveScale),
            elementStart.Z.Meters + (uz * aboveScale),
            LengthUnit.Meter
        );
        // 2. Compute normal of the element's plane
        double[] v1 =
        {
            elementEnd.X.Meters - elementStart.X.Meters,
            elementEnd.Y.Meters - elementStart.Y.Meters,
            elementEnd.Z.Meters - elementStart.Z.Meters,
        };
        double[] v2 =
        {
            elementAbove.X.Meters - elementStart.X.Meters,
            elementAbove.Y.Meters - elementStart.Y.Meters,
            elementAbove.Z.Meters - elementStart.Z.Meters,
        };
        double[] n1 =
        {
            (v1[1] * v2[2]) - (v1[2] * v2[1]),
            (v1[2] * v2[0]) - (v1[0] * v2[2]),
            (v1[0] * v2[1]) - (v1[1] * v2[0]),
        };

        // n1Len is the length (magnitude) of the normal vector n1, which is perpendicular to the plane defined by the element.
        double n1Len = Math.Sqrt((n1[0] * n1[0]) + (n1[1] * n1[1]) + (n1[2] * n1[2]));
        if (n1Len < tolerance.Meters)
        {
            // If n1Len is very small (less than tolerance), it means the plane is degenerate (the vectors are nearly collinear or too short),
            // so the method returns false in that case.
            return false;
        }
        for (int i = 0; i < 3; i++)
        {
            n1[i] /= n1Len;
        }

        // 3. Compute normal of the columns' plane (using column1Start, column1End, column2Start)
        double[] c1 =
        {
            column1End.X.Meters - column1Start.X.Meters,
            column1End.Y.Meters - column1Start.Y.Meters,
            column1End.Z.Meters - column1Start.Z.Meters,
        };
        double[] c2 =
        {
            column2Start.X.Meters - column1Start.X.Meters,
            column2Start.Y.Meters - column1Start.Y.Meters,
            column2Start.Z.Meters - column1Start.Z.Meters,
        };
        double[] n2 =
        {
            (c1[1] * c2[2]) - (c1[2] * c2[1]),
            (c1[2] * c2[0]) - (c1[0] * c2[2]),
            (c1[0] * c2[1]) - (c1[1] * c2[0]),
        };
        double n2Len = Math.Sqrt((n2[0] * n2[0]) + (n2[1] * n2[1]) + (n2[2] * n2[2]));
        if (n2Len < tolerance.Meters)
        {
            return false;
        }
        for (int i = 0; i < 3; i++)
        {
            n2[i] /= n2Len;
        }

        // 4. Check if normals are parallel (dot product ≈ ±1)
        double dot = (n1[0] * n2[0]) + (n1[1] * n2[1]) + (n1[2] * n2[2]);
        double angleTolerance = tolerance.Meters * .01;
        return Math.Abs(Math.Abs(dot) - 1.0) <= angleTolerance;
    }
}
