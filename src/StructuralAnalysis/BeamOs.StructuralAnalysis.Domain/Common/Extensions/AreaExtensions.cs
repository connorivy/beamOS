using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.Common.Extensions;

public static class AreaExtensions
{
    public static AreaMomentOfInertia MultiplyBy(this Area area1, Area area2)
    {
        return AreaMomentOfInertia.FromMetersToTheFourth(area1.SquareMeters * area2.SquareMeters);
    }
}
