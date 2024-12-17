using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Application.Common;

public static class BeamOsDomainContractMappers
{
    public static BeamOs.StructuralAnalysis.Contracts.Common.Point ToContract(
        BeamOs.StructuralAnalysis.Domain.Common.Point source
    )
    {
        LengthUnit unit = source.X.Unit;
        return new(source.X.As(unit), source.Y.As(unit), source.Z.As(unit), unit.MapToContract());
    }

    //public static Dictionary<string, object> ToDict(CustomData customData) => customData.AsDict();

    //public static UnitSettings ToDomain(this UnitSettingsContract source)
    //{
    //    LengthUnit lengthUnit = source.LengthUnit.MapToLengthUnit();
    //    ForceUnit forceUnit = source.ForceUnit.MapToForceUnit();
    //    //AngleUnit angleUnit = source.AngleUnit.MapToAngleUnit();

    //    return new UnitSettings(
    //        lengthUnit,
    //        lengthUnit.ToArea(),
    //        lengthUnit.ToVolume(),
    //        forceUnit,
    //        forceUnit.DivideBy(lengthUnit),
    //        forceUnit.MultiplyBy(lengthUnit),
    //        forceUnit.GetPressure(lengthUnit),
    //        lengthUnit.ToAreaMomentOfInertiaUnit()
    //    );
    //    //LengthUnit lengthUnit,
    //    //AreaUnit areaUnit,
    //    //VolumeUnit volumeUnit,
    //    //ForceUnit forceUnit,
    //    //ForcePerLengthUnit forcePerLengthUnit,
    //    //TorqueUnit torqueUnit,
    //    //PressureUnit pressureUnit,
    //    //AreaMomentOfInertiaUnit areaMomentOfInertiaUnit
    //}

    //public static BeamOs.Domain.Common.ValueObjects.Point ToDomain(
    //    BeamOs.Domain.Common.ValueObjects.Point source
    //)
    //{
    //    return new(
    //        UnitsNetMappers.MapToContract(source.XCoordinate),
    //        UnitsNetMappers.MapToContract(source.YCoordinate),
    //        UnitsNetMappers.MapToContract(source.ZCoordinate)
    //    );
    //}
}
