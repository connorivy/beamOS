using BeamOs.StructuralAnalysis.Domain.Common;
using Microsoft.VisualBasic;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public static class ModelRepairRuleUtils
{
    /// <summary>
    /// Takes the start and end points of two segments and checks if the two lines are roughly coplanar.
    /// </summary>
    /// <param name="startA"></param>
    /// <param name="endA"></param>
    /// <param name="startB"></param>
    /// <param name="endB"></param>
    /// <param name="tolerance"></param>
    /// <returns></returns>
    public static bool ArePointsRoughlyCoplanar(
        System.Numerics.Vector3 startA,
        System.Numerics.Vector3 endA,
        System.Numerics.Vector3 startB,
        System.Numerics.Vector3 endB
    )
    {
        // Compute direction vectors for each segment
        System.Numerics.Vector3 dirA = endA - startA;
        // System.Numerics.Vector3 dirB = endB - startB;

        // Compute the normal vector to the plane defined by segment A
        System.Numerics.Vector3 normalA = System.Numerics.Vector3.Cross(dirA, startB - startA);
        // If the normal is close to zero, the points are colinear, treat as coplanar
        if (normalA.Length() < 1e-6f)
        {
            return true;
        }

        // Compute the distance from endB to the plane defined by segment A
        System.Numerics.Vector3 toEndB = endB - startA;
        float dist = MathF.Abs(System.Numerics.Vector3.Dot(normalA, toEndB)) / normalA.Length();

        // Use a reasonable tolerance for coplanarity (e.g., 1e-4)
        const float tolerance = 1e-4f;
        return dist < tolerance;
    }

    public static Point FindIntersectionOfLineAndPlane(
        Point startPointA,
        Point endPointA,
        Point planePointA,
        Point planePointB,
        Point planePointC,
        out double t
    )
    {
        System.Numerics.Vector3 startA = new(
            (float)startPointA.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );
        System.Numerics.Vector3 endA = new(
            (float)endPointA.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );
        System.Numerics.Vector3 planeA = new(
            (float)planePointA.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );
        System.Numerics.Vector3 planeB = new(
            (float)planePointB.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );
        System.Numerics.Vector3 planeC = new(
            (float)planePointC.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );

        // Compute the plane normal
        System.Numerics.Vector3 v1 = planeB - planeA;
        System.Numerics.Vector3 v2 = planeC - planeA;
        System.Numerics.Vector3 normal = System.Numerics.Vector3.Cross(v1, v2);

        // Line direction
        System.Numerics.Vector3 dir = endA - startA;

        // Compute denominator (dot product of normal and line direction)
        float denom = System.Numerics.Vector3.Dot(normal, dir);
        if (MathF.Abs(denom) < 1e-8f)
        {
            // Line is parallel to the plane
            t = double.NaN;
            return null!; // Or throw, or return a special value
        }

        // Compute t for the intersection point
        float num = System.Numerics.Vector3.Dot(normal, planeA - startA);
        t = num / denom;

        // Intersection point
        System.Numerics.Vector3 intersection = startA + (dir * (float)t);

        // Use meters as the default LengthUnit for the result
        return new Point(intersection.X, intersection.Y, intersection.Z, LengthUnit.Meter);
    }

    public static bool ArePointsRoughlyCollinear(
        Point locationPoint1,
        Point locationPoint2,
        Point locationPoint3,
        double angleTolerance
    )
    {
        // Convert points to vectors
        System.Numerics.Vector3 vector1 = new(
            (float)locationPoint1.X.Meters,
            (float)locationPoint1.Y.Meters,
            (float)locationPoint1.Z.Meters
        );
        System.Numerics.Vector3 vector2 = new(
            (float)locationPoint2.X.Meters,
            (float)locationPoint2.Y.Meters,
            (float)locationPoint2.Z.Meters
        );
        System.Numerics.Vector3 vector3 = new(
            (float)locationPoint3.X.Meters,
            (float)locationPoint3.Y.Meters,
            (float)locationPoint3.Z.Meters
        );

        // Calculate direction vectors
        System.Numerics.Vector3 dir1 = vector2 - vector1;
        System.Numerics.Vector3 dir2 = vector3 - vector2;

        // Calculate the angle between the two direction vectors
        float angle = MathF.Acos(
            System.Numerics.Vector3.Dot(dir1, dir2) / (dir1.Length() * dir2.Length())
        );

        // Check if the angle is within the tolerance
        return angle < angleTolerance;
    }
}
