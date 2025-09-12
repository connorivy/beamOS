using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.Common.Extensions;

internal static class AreaMomentOfInertiaExtensions
{
    public static Length DivideBy(this AreaMomentOfInertia areaMomentOfInertia, Volume volume)
    {
        return Length.FromMeters(areaMomentOfInertia.MetersToTheFourth / volume.CubicMeters);
    }

    public static Area DivideBy(this AreaMomentOfInertia areaMomentOfInertia, Area area)
    {
        return Area.FromSquareMeters(areaMomentOfInertia.MetersToTheFourth / area.SquareMeters);
    }
}
