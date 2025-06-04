using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class ExtendOrShortenBeamInPlaneWithSurrounding : BeamOrBraceVisitingRule
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Favorable;

    protected override void ApplyRuleForBeamOrBrace(
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
        ExtendOrShortenIfCoplanar(
            startNode,
            endNode,
            nearbyStartNodes,
            beamsAndBracesCloseToStart,
            modelProposalBuilder,
            tolerance
        );
        ExtendOrShortenIfCoplanar(
            endNode,
            startNode,
            nearbyEndNodes,
            beamsAndBracesCloseToEnd,
            modelProposalBuilder,
            tolerance
        );
    }

    private static void ExtendOrShortenIfCoplanar(
        Node thisEnd,
        Node otherEnd,
        IList<Node> nearbyNodes,
        IEnumerable<Element1d> nearbyElements,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        var elementsAndDistances = nearbyElements
            .Select(elem => (elem, ShortestDistanceTo(thisEnd, elem, modelProposalBuilder)))
            .Where(pair => pair.Item2 < tolerance)
            .ToList();

        foreach (Element1d elem in elementsAndDistances.OrderBy(e => e.Item2).Select(e => e.elem))
        {
            (Node n1, Node n2) = modelProposalBuilder.GetStartAndEndNodes(elem);
            if (
                ArePointsRoughlyCoplanar(
                    thisEnd.LocationPoint,
                    otherEnd.LocationPoint,
                    n1.LocationPoint,
                    n2.LocationPoint,
                    tolerance
                )
            )
            {
                return;
            }
        }
    }

    private static Length ShortestDistanceTo(
        Node node,
        Element1d element1d,
        ModelProposalBuilder modelProposalBuilder
    )
    {
        var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(element1d);
        return node.LocationPoint.ShortestDistanceToLine(
            startNode.LocationPoint,
            endNode.LocationPoint
        );
    }

    /// <summary>
    /// Takes the start and end points of two segments and checks if the two lines are roughly coplanar.
    /// </summary>
    /// <param name="startA"></param>
    /// <param name="endA"></param>
    /// <param name="startB"></param>
    /// <param name="endB"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    private static bool ArePointsRoughlyCoplanar(
        Point startA,
        Point endA,
        Point startB,
        Point endB,
        Length tolerance
    )
    {
        // Convert points to vectors
        double v1x = endA.X.Meters - startA.X.Meters;
        double v1y = endA.Y.Meters - startA.Y.Meters;
        double v1z = endA.Z.Meters - startA.Z.Meters;
        double v2x = startB.X.Meters - startA.X.Meters;
        double v2y = startB.Y.Meters - startA.Y.Meters;
        double v2z = startB.Z.Meters - startA.Z.Meters;
        double v3x = endB.X.Meters - startA.X.Meters;
        double v3y = endB.Y.Meters - startA.Y.Meters;
        double v3z = endB.Z.Meters - startA.Z.Meters;

        // Cross product v1 x v2
        double cx = (v1y * v2z) - (v1z * v2y);
        double cy = (v1z * v2x) - (v1x * v2z);
        double cz = (v1x * v2y) - (v1y * v2x);
        // Dot product (v1 x v2) . v3
        double tripleProduct = (cx * v3x) + (cy * v3y) + (cz * v3z);

        // Lengths for normalization
        double v1Len = Math.Sqrt((v1x * v1x) + (v1y * v1y) + (v1z * v1z));
        double v4x = endB.X.Meters - startB.X.Meters;
        double v4y = endB.Y.Meters - startB.Y.Meters;
        double v4z = endB.Z.Meters - startB.Z.Meters;
        double v4Len = Math.Sqrt((v4x * v4x) + (v4y * v4y) + (v4z * v4z));
        double avgLen = (v1Len + v4Len) / 2.0;
        double norm = avgLen * avgLen * avgLen;
        if (norm < 1e-12)
        {
            return false;
        }
        double relTriple = Math.Abs(tripleProduct) / norm;
        return relTriple < tolerance.Meters;
    }

    /// <summary>
    /// This method takes the start and end nodes of two elements. It tries to extend or shorten each element
    /// in such a way that they become joined together without either element changing it's direction.
    /// </summary>
    /// <param name="startNodeA"></param>
    /// <param name="endNodeA"></param>
    /// <param name="startNodeB"></param>
    /// <param name="endNodeB"></param>
    /// <param name="modelProposalBuilder"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    private bool TryExtendElementsToBeCombined(
        Node startNodeA,
        Node endNodeA,
        Node startNodeB,
        Node endNodeB,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        // Get direction vectors for both elements
        double[] dirA = Subtract(endNodeA.LocationPoint, startNodeA.LocationPoint);
        double[] dirB = Subtract(endNodeB.LocationPoint, startNodeB.LocationPoint);

        // Normalize direction vectors
        double[] dirANorm = Normalize(dirA);
        double[] dirBNorm = Normalize(dirB);

        // Check if directions are parallel (collinear or anti-collinear)
        double dot = Dot(dirANorm, dirBNorm);
        bool areParallel = Math.Abs(Math.Abs(dot) - 1.0) < 1e-8;

        if (areParallel)
        {
            // Project B's start onto A's line
            Point intersection = ProjectPointOntoLine(
                startNodeB.LocationPoint,
                startNodeA.LocationPoint,
                dirANorm
            );
            double distA = Distance(intersection, startNodeA.LocationPoint);
            double distB = Distance(intersection, startNodeB.LocationPoint);
            // If the intersection is within tolerance for both elements, they can be joined
            if (distA < tolerance.Meters || distB < tolerance.Meters)
            {
                // TODO: Propose to move endNodeA or startNodeB to intersection point
                // modelProposalBuilder.RequestNodeMove(...)
                return true;
            }
            return false;
        }
        else
        {
            // Not parallel: find intersection of the two lines (if any)
            var maybeIntersection = ClosestPointBetweenLines(
                startNodeA.LocationPoint,
                dirANorm,
                startNodeB.LocationPoint,
                dirBNorm
            );
            if (maybeIntersection.HasValue)
            {
                (Point ptA, Point ptB) = maybeIntersection.Value;
                double dist = Distance(ptA, ptB);
                if (dist < tolerance.Meters)
                {
                    // TODO: Propose to move endNodeA and startNodeB to the intersection point
                    // modelProposalBuilder.RequestNodeMove(...)
                    return true;
                }
            }
            return false;
        }
    }

    // Helper: Subtract two points to get a vector
    private static double[] Subtract(Point a, Point b)
    {
        return new double[]
        {
            a.X.Meters - b.X.Meters,
            a.Y.Meters - b.Y.Meters,
            a.Z.Meters - b.Z.Meters,
        };
    }

    // Helper: Normalize a vector
    private static double[] Normalize(double[] v)
    {
        double len = Math.Sqrt((v[0] * v[0]) + (v[1] * v[1]) + (v[2] * v[2]));
        if (len < 1e-12)
        {
            return new double[] { 0, 0, 0 };
        }
        return new double[] { v[0] / len, v[1] / len, v[2] / len };
    }

    // Helper: Dot product
    private static double Dot(double[] a, double[] b)
    {
        return (a[0] * b[0]) + (a[1] * b[1]) + (a[2] * b[2]);
    }

    // Helper: Distance between two points
    private static double Distance(Point a, Point b)
    {
        double dx = a.X.Meters - b.X.Meters;
        double dy = a.Y.Meters - b.Y.Meters;
        double dz = a.Z.Meters - b.Z.Meters;
        return Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
    }

    // Helper: Project a point onto a line defined by (origin, direction)
    private static Point ProjectPointOntoLine(Point pt, Point origin, double[] direction)
    {
        double[] v = Subtract(pt, origin);
        double t = Dot(v, direction) / Dot(direction, direction);
        return new Point(
            Length.FromMeters(origin.X.Meters + t * direction[0]),
            Length.FromMeters(origin.Y.Meters + t * direction[1]),
            Length.FromMeters(origin.Z.Meters + t * direction[2])
        );
    }

    // Helper: Find closest points between two lines (returns null if lines are parallel)
    private static (Point, Point)? ClosestPointBetweenLines(
        Point p1,
        double[] d1,
        Point p2,
        double[] d2
    )
    {
        double[] r = Subtract(p1, p2);
        double a = Dot(d1, d1);
        double b = Dot(d1, d2);
        double c = Dot(d2, d2);
        double d = Dot(d1, r);
        double e = Dot(d2, r);
        double denom = (a * c) - (b * b);
        if (Math.Abs(denom) < 1e-12)
        {
            return null; // Parallel
        }
        double t1 = ((b * e) - (c * d)) / denom;
        double t2 = ((a * e) - (b * d)) / denom;
        Point ptA = new Point(
            Length.FromMeters(p1.X.Meters + t1 * d1[0]),
            Length.FromMeters(p1.Y.Meters + t1 * d1[1]),
            Length.FromMeters(p1.Z.Meters + t1 * d1[2])
        );
        Point ptB = new Point(
            Length.FromMeters(p2.X.Meters + t2 * d2[0]),
            Length.FromMeters(p2.Y.Meters + t2 * d2[1]),
            Length.FromMeters(p2.Z.Meters + t2 * d2[2])
        );
        return (ptA, ptB);
    }
}
