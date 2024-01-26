using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UnitsNet;

namespace BeamOS.Common.Infrastructure.ValueConverters;

public class AngleValueConverter : ValueConverter<Angle, double>
{
    public AngleValueConverter()
        : base(
            x => x.As(UnitsNet.Units.AngleUnit.Radian),
            x => new(x, UnitsNet.Units.AngleUnit.Radian),
            null
        ) { }
}

public class LengthValueConverter : ValueConverter<Length, double>
{
    public LengthValueConverter()
        : base(
            x => x.As(UnitsNet.Units.LengthUnit.Meter),
            x => new(x, UnitsNet.Units.LengthUnit.Meter),
            null
        ) { }
}

public class AreaValueConverter : ValueConverter<Area, double>
{
    public AreaValueConverter()
        : base(
            x => x.As(UnitsNet.Units.AreaUnit.SquareMeter),
            x => new(x, UnitsNet.Units.AreaUnit.SquareMeter),
            null
        ) { }
}

public class VolumeValueConverter : ValueConverter<Volume, double>
{
    public VolumeValueConverter()
        : base(
            x => x.As(UnitsNet.Units.VolumeUnit.CubicMeter),
            x => new(x, UnitsNet.Units.VolumeUnit.CubicMeter),
            null
        ) { }
}

public class AreaMomentOfInertiaValueConverter : ValueConverter<AreaMomentOfInertia, double>
{
    public AreaMomentOfInertiaValueConverter()
        : base(
            x => x.As(UnitsNet.Units.AreaMomentOfInertiaUnit.MeterToTheFourth),
            x => new(x, UnitsNet.Units.AreaMomentOfInertiaUnit.MeterToTheFourth),
            null
        ) { }
}

public class ForceValueConverter : ValueConverter<Force, double>
{
    public ForceValueConverter()
        : base(
            x => x.As(UnitsNet.Units.ForceUnit.Newton),
            x => new(x, UnitsNet.Units.ForceUnit.Newton),
            null
        ) { }
}

public class ForcePerLengthValueConverter : ValueConverter<ForcePerLength, double>
{
    public ForcePerLengthValueConverter()
        : base(
            x => x.As(UnitsNet.Units.ForcePerLengthUnit.NewtonPerMeter),
            x => new(x, UnitsNet.Units.ForcePerLengthUnit.NewtonPerMeter),
            null
        ) { }
}

public class TorqueValueConverter : ValueConverter<Torque, double>
{
    public TorqueValueConverter()
        : base(
            x => x.As(UnitsNet.Units.TorqueUnit.NewtonMeter),
            x => new(x, UnitsNet.Units.TorqueUnit.NewtonMeter),
            null
        ) { }
}

public class PressureValueConverter : ValueConverter<Pressure, double>
{
    public PressureValueConverter()
        : base(
            x => x.As(UnitsNet.Units.PressureUnit.NewtonPerSquareMeter),
            x => new(x, UnitsNet.Units.PressureUnit.NewtonPerSquareMeter),
            null
        ) { }
}
