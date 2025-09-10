using System.Numerics;
using BeamOs.StructuralAnalysis.Domain.Common;
using UnitsNet;

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
        Vector3 startA,
        Vector3 endA,
        Vector3 startB,
        Vector3 endB
    )
    {
        // Compute direction vectors for each segment
        Vector3 dirA = endA - startA;
        // Vector3 dirB = endB - startB;

        // Compute the normal vector to the plane defined by segment A
        Vector3 normalA = Vector3.Cross(dirA, startB - startA);
        // If the normal is close to zero, the points are colinear, treat as coplanar
        if (normalA.Length() < 1e-6f)
        {
            return true;
        }

        // Compute the distance from endB to the plane defined by segment A
        Vector3 toEndB = endB - startA;
        float dist = MathF.Abs(Vector3.Dot(normalA, toEndB)) / normalA.Length();

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
        Vector3 startA = new(
            (float)startPointA.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );
        Vector3 endA = new(
            (float)endPointA.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );
        Vector3 planeA = new(
            (float)planePointA.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );
        Vector3 planeB = new(
            (float)planePointB.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );
        Vector3 planeC = new(
            (float)planePointC.X.Meters,
            (float)startPointA.Y.Meters,
            (float)startPointA.Z.Meters
        );

        // Compute the plane normal
        Vector3 v1 = planeB - planeA;
        Vector3 v2 = planeC - planeA;
        Vector3 normal = Vector3.Cross(v1, v2);

        // Line direction
        Vector3 dir = endA - startA;

        // Compute denominator (dot product of normal and line direction)
        float denom = Vector3.Dot(normal, dir);
        if (MathF.Abs(denom) < 1e-8f)
        {
            // Line is parallel to the plane
            t = double.NaN;
            return null!; // Or throw, or return a special value
        }

        // Compute t for the intersection point
        float num = Vector3.Dot(normal, planeA - startA);
        t = num / denom;

        // Intersection point
        Vector3 intersection = startA + (dir * (float)t);

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
    //     var v1 = new Vector3(
    //         (float)locationPoint1.X.Meters,
    //         (float)locationPoint1.Y.Meters,
    //         (float)locationPoint1.Z.Meters
    //     );
    //     var v2 = new Vector3(
    //         (float)locationPoint2.X.Meters,
    //         (float)locationPoint2.Y.Meters,
    //         (float)locationPoint2.Z.Meters
    //     );
    //     var v3 = new Vector3(
    //         (float)locationPoint3.X.Meters,
    //         (float)locationPoint3.Y.Meters,
    //         (float)locationPoint3.Z.Meters
    //     );

    //     // Direction vectors
    //     Vector3 dir1 = v2 - v1;
    //     Vector3 dir2 = v3 - v2;

    //     // Project onto each axis and check angle tolerances
    //     // X axis
    //     Vector2 dir1X = new Vector2(dir1.Y, dir1.Z);
    //     Vector2 dir2X = new Vector2(dir2.Y, dir2.Z);
    //     float angleX = MathF.Acos(
    //         Vector2.Dot(dir1X, dir2X) / (dir1X.Length() * dir2X.Length())
    //     );
    //     // Y axis
    //     Vector2 dir1Y = new Vector2(dir1.X, dir1.Z);
    //     Vector2 dir2Y = new Vector2(dir2.X, dir2.Z);
    //     float angleY = MathF.Acos(
    //         Vector2.Dot(dir1Y, dir2Y) / (dir1Y.Length() * dir2Y.Length())
    //     );
    //     // Z axis
    //     Vector2 dir1Z = new Vector2(dir1.X, dir1.Y);
    //     Vector2 dir2Z = new Vector2(dir2.X, dir2.Y);
    //     float angleZ = MathF.Acos(
    //         Vector2.Dot(dir1Z, dir2Z) / (dir1Z.Length() * dir2Z.Length())
    //     );

    //     // Compute the overall 3D angle between the two direction vectors
    //     float overallAngle = MathF.Acos(
    //         Vector3.Dot(dir1, dir2) / (dir1.Length() * dir2.Length())
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
        Vector3 start = new Vector3(
            (float)lineStartPoint.X.Meters,
            (float)lineStartPoint.Y.Meters,
            (float)lineStartPoint.Z.Meters
        );
        Vector3 end = new Vector3(
            (float)lineEndPoint.X.Meters,
            (float)lineEndPoint.Y.Meters,
            (float)lineEndPoint.Z.Meters
        );
        Vector3 target = new Vector3(
            (float)point.X.Meters,
            (float)point.Y.Meters,
            (float)point.Z.Meters
        );

        Vector3 lineDir = end - start;
        Vector3 toTarget = target - end;

        // If the target is at the endpoint, it's trivially extendable
        if (toTarget.Length() < 1e-8f)
        {
            return true;
        }

        // Check if the direction to the target is collinear with the line direction (within a small angle)
        float dot = Vector3.Dot(lineDir, toTarget);
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

    public static bool TryFindApproximateIntersection(
        Point start1,
        Point end1,
        Point start2,
        Point end2,
        out Vector3 intersection
    )
    {
        Vector3 a1 = new Vector3(
            (float)start1.X.Meters,
            (float)start1.Y.Meters,
            (float)start1.Z.Meters
        );
        Vector3 a2 = new Vector3((float)end1.X.Meters, (float)end1.Y.Meters, (float)end1.Z.Meters);
        Vector3 b1 = new Vector3(
            (float)start2.X.Meters,
            (float)start2.Y.Meters,
            (float)start2.Z.Meters
        );
        Vector3 b2 = new Vector3((float)end2.X.Meters, (float)end2.Y.Meters, (float)end2.Z.Meters);

        Vector3 d1 = a2 - a1;
        Vector3 d2 = b2 - b1;
        Vector3 r = a1 - b1;

        var a = Vector3.Dot(d1, d1);
        var b = Vector3.Dot(d1, d2);
        var c = Vector3.Dot(d2, d2);
        var d = Vector3.Dot(d1, r);
        var e = Vector3.Dot(d2, r);

        float denom = a * c - b * b;
        float s,
            t;

        if (Math.Abs(denom) < 1e-6f)
        {
            // Lines are nearly parallel. Check if they are collinear by direction alignment
            float d1Len = d1.Length();
            float d2Len = d2.Length();
            if (d1Len < 1e-12f || d2Len < 1e-12f)
            {
                intersection = Vector3.Zero;
                return false; // Degenerate segment
            }
            float alignment = Math.Abs(Vector3.Dot(Vector3.Normalize(d1), Vector3.Normalize(d2)));
            if (alignment > 1.0f - 1e-6f) // directions are aligned (collinear)
            {
                // Return midpoint of closest endpoints as intersection
                float tCol = Vector3.Dot(a1 - b1, d2) / (d2Len * d2Len);
                Vector3 closestOnB = b1 + (tCol * d2);
                float sCol = Vector3.Dot(b1 - a1, d1) / (d1Len * d1Len);
                Vector3 closestOnA = a1 + (sCol * d1);
                intersection = (closestOnA + closestOnB) / 2;
                return true;
            }
            intersection = Vector3.Zero;
            return false; // Parallel but not collinear
        }
        else
        {
            s = ((b * e) - (c * d)) / denom;
            t = ((a * e) - (b * d)) / denom;
        }

        Vector3 closestPointLine1 = a1 + (s * d1);
        Vector3 closestPointLine2 = b1 + (t * d2);
        intersection = (closestPointLine1 + closestPointLine2) / 2;

        return true;
    }

    public static bool PointWithinTolerances(
        Point point,
        Vector3 target,
        Length xAxisLengthTolerance,
        Length yAxisLengthTolerance,
        Length zAxisLengthTolerance,
        Length overallLengthTolerance
    )
    {
        var pointVector = point.ToVector3InMeters();
        // Check per-axis tolerances
        float dx = MathF.Abs(target.X - pointVector.X);
        float dy = MathF.Abs(target.Y - pointVector.Y);
        float dz = MathF.Abs(target.Z - pointVector.Z);
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
        float dist = (target - pointVector).Length();
        if (dist > (float)overallLengthTolerance.Meters)
        {
            return false;
        }
        return true;
    }

    public static double GetToleranceValue(
        AxisAlignmentToleranceLevel level,
        ModelRepairOperationParameters p
    )
    {
        return level switch
        {
            AxisAlignmentToleranceLevel.VeryStrict => p.VeryStrictTolerance.Meters,
            AxisAlignmentToleranceLevel.Strict => p.StrictTolerance.Meters,
            AxisAlignmentToleranceLevel.Standard => p.StandardTolerance.Meters,
            AxisAlignmentToleranceLevel.Relaxed => p.RelaxedTolerance.Meters,
            AxisAlignmentToleranceLevel.VeryRelaxed => p.VeryRelaxedTolerance.Meters,
            _ => p.StandardTolerance.Meters,
        };
    }

    public static AxisAlignmentTolerance GetAxisAlignmentTolerance(
        Point startNodeLocation,
        Point endNodeLocation,
        ModelRepairOperationParameters modelRepairOperationParameters
    )
    {
        return new(
            GetAxisAlignmentToleranceLevel(
                startNodeLocation.X - endNodeLocation.X,
                modelRepairOperationParameters
            ),
            GetAxisAlignmentToleranceLevel(
                startNodeLocation.Y - endNodeLocation.Y,
                modelRepairOperationParameters
            ),
            GetAxisAlignmentToleranceLevel(
                startNodeLocation.Z - endNodeLocation.Z,
                modelRepairOperationParameters
            )
        );
    }

    public static AxisAlignmentToleranceLevel GetAxisAlignmentToleranceLevel(
        Length length,
        ModelRepairOperationParameters modelRepairOperationParameters
    )
    {
        length = length.Abs();
        if (length < modelRepairOperationParameters.VeryStrictTolerance)
        {
            return AxisAlignmentToleranceLevel.VeryStrict;
        }
        if (length < modelRepairOperationParameters.StrictTolerance)
        {
            return AxisAlignmentToleranceLevel.Strict;
        }
        if (length < modelRepairOperationParameters.StandardTolerance)
        {
            return AxisAlignmentToleranceLevel.Standard;
        }
        if (length < modelRepairOperationParameters.RelaxedTolerance)
        {
            return AxisAlignmentToleranceLevel.Relaxed;
        }
        return AxisAlignmentToleranceLevel.VeryRelaxed;
    }
}
