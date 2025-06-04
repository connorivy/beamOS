using BeamOs.StructuralAnalysis.Domain.Common;

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

    // public static bool ArePointsRoughlyCollinear(
    //     Point locationPoint1,
    //     Point locationPoint2,
    //     Point locationPoint3,
    //     Angle xAxisAngleTolerance,
    //     Angle yAxisAngleTolerance,
    //     Angle zAxisAngleTolerance,
    //     Angle overallAngleTolerance
    // )
    // {
    //     // Convert points to vectors
    //     var v1 = new System.Numerics.Vector3(
    //         (float)locationPoint1.X.Meters,
    //         (float)locationPoint1.Y.Meters,
    //         (float)locationPoint1.Z.Meters
    //     );
    //     var v2 = new System.Numerics.Vector3(
    //         (float)locationPoint2.X.Meters,
    //         (float)locationPoint2.Y.Meters,
    //         (float)locationPoint2.Z.Meters
    //     );
    //     var v3 = new System.Numerics.Vector3(
    //         (float)locationPoint3.X.Meters,
    //         (float)locationPoint3.Y.Meters,
    //         (float)locationPoint3.Z.Meters
    //     );

    //     // Direction vectors
    //     System.Numerics.Vector3 dir1 = v2 - v1;
    //     System.Numerics.Vector3 dir2 = v3 - v2;

    //     // Project onto each axis and check angle tolerances
    //     // X axis
    //     System.Numerics.Vector2 dir1X = new System.Numerics.Vector2(dir1.Y, dir1.Z);
    //     System.Numerics.Vector2 dir2X = new System.Numerics.Vector2(dir2.Y, dir2.Z);
    //     float angleX = MathF.Acos(
    //         System.Numerics.Vector2.Dot(dir1X, dir2X) / (dir1X.Length() * dir2X.Length())
    //     );
    //     // Y axis
    //     System.Numerics.Vector2 dir1Y = new System.Numerics.Vector2(dir1.X, dir1.Z);
    //     System.Numerics.Vector2 dir2Y = new System.Numerics.Vector2(dir2.X, dir2.Z);
    //     float angleY = MathF.Acos(
    //         System.Numerics.Vector2.Dot(dir1Y, dir2Y) / (dir1Y.Length() * dir2Y.Length())
    //     );
    //     // Z axis
    //     System.Numerics.Vector2 dir1Z = new System.Numerics.Vector2(dir1.X, dir1.Y);
    //     System.Numerics.Vector2 dir2Z = new System.Numerics.Vector2(dir2.X, dir2.Y);
    //     float angleZ = MathF.Acos(
    //         System.Numerics.Vector2.Dot(dir1Z, dir2Z) / (dir1Z.Length() * dir2Z.Length())
    //     );

    //     // Compute the overall 3D angle between the two direction vectors
    //     float overallAngle = MathF.Acos(
    //         System.Numerics.Vector3.Dot(dir1, dir2) / (dir1.Length() * dir2.Length())
    //     );

    //     // Check if all angles are within their respective tolerances
    //     return angleX < xAxisAngleTolerance.Radians
    //         && angleY < yAxisAngleTolerance.Radians
    //         && angleZ < zAxisAngleTolerance.Radians
    //         && overallAngle < overallAngleTolerance.Radians;
    // }


    /// <summary>
    /// Checks if the endpoint of a line can be extended to a given point within specified tolerances.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="point"></param>
    /// <param name="xAxisLengthTolerance"></param>
    /// <param name="yAxisLengthTolerance"></param>
    /// <param name="zAxisLengthTolerance"></param>
    /// <param name="overallLengthTolerance"></param>
    /// <returns></returns>
    public static bool CanLineEndpointBeExtendedToPointWithinTolerance(
        Point lineStartPoint,
        Point lineEndPoint,
        Point point,
        Length xAxisLengthTolerance,
        Length yAxisLengthTolerance,
        Length zAxisLengthTolerance,
        Length overallLengthTolerance
    )
    {
        System.Numerics.Vector3 start = new System.Numerics.Vector3(
            (float)lineStartPoint.X.Meters,
            (float)lineStartPoint.Y.Meters,
            (float)lineStartPoint.Z.Meters
        );
        System.Numerics.Vector3 end = new System.Numerics.Vector3(
            (float)lineEndPoint.X.Meters,
            (float)lineEndPoint.Y.Meters,
            (float)lineEndPoint.Z.Meters
        );
        System.Numerics.Vector3 target = new System.Numerics.Vector3(
            (float)point.X.Meters,
            (float)point.Y.Meters,
            (float)point.Z.Meters
        );

        System.Numerics.Vector3 lineDir = end - start;
        System.Numerics.Vector3 toTarget = target - end;

        // If the target is at the endpoint, it's trivially extendable
        if (toTarget.Length() < 1e-8f)
        {
            return true;
        }

        // Check if the direction to the target is collinear with the line direction (within a small angle)
        float dot = System.Numerics.Vector3.Dot(lineDir, toTarget);
        if (dot <= 0)
        {
            // Only allow extension, not retraction
            return false;
        }

        // Check per-axis tolerances
        float dx = MathF.Abs(target.X - end.X);
        float dy = MathF.Abs(target.Y - end.Y);
        float dz = MathF.Abs(target.Z - end.Z);
        if (dx > (float)xAxisLengthTolerance.Meters)
        {
            return false;
        }
        if (dy > (float)yAxisLengthTolerance.Meters)
        {
            return false;
        }
        if (dz > (float)zAxisLengthTolerance.Meters)
        {
            return false;
        }

        // Check overall tolerance
        float dist = toTarget.Length();
        if (dist > (float)overallLengthTolerance.Meters)
        {
            return false;
        }

        return true;
    }
}
