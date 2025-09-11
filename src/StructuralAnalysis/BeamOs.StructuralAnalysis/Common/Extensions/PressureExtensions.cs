using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.Common.Extensions;

public static class PressureExtensions
{
    public static ForcePerLength MultiplyBy(this Pressure pressure, Length length)
    {
        return ForcePerLength.FromNewtonsPerMeter(pressure.NewtonsPerSquareMeter * length.Meters);
    }

    public static Force MultiplyBy(this Pressure pressure, Area area)
    {
        return Force.FromNewtons(pressure.NewtonsPerSquareMeter * area.SquareMeters);
    }

    public static Torque MultiplyBy(this Pressure pressure, Volume volume)
    {
        return Torque.FromNewtonMeters(pressure.NewtonsPerSquareMeter * volume.CubicMeters);
    }
}

public static class TorqueExtensions
{
    public static Volume DivideBy(this Torque torque, Pressure pressure)
    {
        return Volume.FromCubicMeters(torque.NewtonMeters / pressure.NewtonsPerSquareMeter);
    }

    public static Pressure DivideBy(this Torque torque, Length length)
    {
        return Pressure.FromNewtonsPerSquareMeter(torque.NewtonMeters / length.Meters);
    }
}
