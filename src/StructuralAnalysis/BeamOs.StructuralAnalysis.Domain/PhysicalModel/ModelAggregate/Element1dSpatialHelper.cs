using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public static class Element1dSpatialHelper
{
    public static List<Element1d> FindElement1dsWithin(
        IEnumerable<Element1d> element1ds,
        Point searchPoint,
        double toleranceMeters
    )
    {
        Point p = searchPoint;
        List<Element1d> result = [];
        foreach (Element1d element in element1ds)
        {
            if (element.StartNode is null || element.EndNode is null)
            {
                continue;
            }
            Point a = element.StartNode.LocationPoint;
            Point b = element.EndNode.LocationPoint;
            if (DistancePointToSegment(p, a, b) <= toleranceMeters)
            {
                result.Add(element);
            }
        }
        return result;
    }

    private static double DistancePointToSegment(Point p, Point a, Point b)
    {
        double abX = b.X.Meters - a.X.Meters;
        double abY = b.Y.Meters - a.Y.Meters;
        double abZ = b.Z.Meters - a.Z.Meters;
        double apX = p.X.Meters - a.X.Meters;
        double apY = p.Y.Meters - a.Y.Meters;
        double apZ = p.Z.Meters - a.Z.Meters;
        double abLenSq = (abX * abX) + (abY * abY) + (abZ * abZ);
        double t = abLenSq > 0 ? ((abX * apX) + (abY * apY) + (abZ * apZ)) / abLenSq : 0.0;
        t = Math.Max(0, Math.Min(1, t));
        double closestX = a.X.Meters + (t * abX);
        double closestY = a.Y.Meters + (t * abY);
        double closestZ = a.Z.Meters + (t * abZ);
        double dx = p.X.Meters - closestX;
        double dy = p.Y.Meters - closestY;
        double dz = p.Z.Meters - closestZ;
        return Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
    }
}
