using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Application.Common;

public abstract class AbstractMapperProvidedUnits<TFrom, TTo>(
    LengthUnit lengthUnit,
    AreaUnit areaUnit,
    VolumeUnit volumeUnit,
    ForceUnit forceUnit,
    ForcePerLengthUnit forcePerLengthUnit,
    TorqueUnit torqueUnit,
    AngleUnit angleUnit,
    PressureUnit pressureUnit,
    AreaMomentOfInertiaUnit areaMomentOfInertiaUnit
)
//: AbstractMapper<TFrom, TTo>
{
    public AbstractMapperProvidedUnits(UnitSettings unitSettings)
        : this(
            unitSettings.LengthUnit,
            unitSettings.AreaUnit,
            unitSettings.VolumeUnit,
            unitSettings.ForceUnit,
            unitSettings.ForcePerLengthUnit,
            unitSettings.TorqueUnit,
            unitSettings.AngleUnit,
            unitSettings.PressureUnit,
            unitSettings.AreaMomentOfInertiaUnit
        ) { }

    protected LengthUnit LengthUnit { get; } = lengthUnit;
    protected AreaUnit AreaUnit { get; } = areaUnit;
    protected VolumeUnit VolumeUnit { get; } = volumeUnit;
    protected ForceUnit ForceUnit { get; } = forceUnit;
    protected ForcePerLengthUnit ForcePerLengthUnit { get; } = forcePerLengthUnit;
    protected TorqueUnit TorqueUnit { get; } = torqueUnit;
    protected AngleUnit AngleUnit { get; } = angleUnit;
    protected PressureUnit PressureUnit { get; } = pressureUnit;
    protected AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit { get; } = areaMomentOfInertiaUnit;

    protected AngleContract UnitsNetToContractMapper(Angle unit) =>
        new(unit.As(this.AngleUnit), this.AngleUnit.MapToContract());

    protected AreaContract UnitsNetToContractMapper(Area unit) =>
        new(unit.As(this.AreaUnit), this.AreaUnit.MapToContract());

    protected AreaMomentOfInertiaContract UnitsNetToContractMapper(AreaMomentOfInertia unit) =>
        new(unit.As(this.AreaMomentOfInertiaUnit), this.AreaMomentOfInertiaUnit.MapToContract());

    protected ForceContract UnitsNetToContractMapper(Force unit) =>
        new(unit.As(this.ForceUnit), this.ForceUnit.MapToContract());

    protected ForcePerLengthContract UnitsNetToContractMapper(ForcePerLength unit) =>
        new(unit.As(this.ForcePerLengthUnit), this.ForcePerLengthUnit.MapToContract());

    protected LengthContract UnitsNetToContractMapper(Length unit) =>
        new(unit.As(this.LengthUnit), this.LengthUnit.MapToContract());

    protected PressureContract UnitsNetToContractMapper(Pressure unit) =>
        new(unit.As(this.PressureUnit), this.PressureUnit.MapToContract());

    protected TorqueContract UnitsNetToContractMapper(Torque unit) =>
        new(unit.As(this.TorqueUnit), this.TorqueUnit.MapToContract());

    protected VolumeContract UnitsNetToContractMapper(Volume unit) =>
        new(unit.As(this.VolumeUnit), this.VolumeUnit.MapToContract());

    //protected BeamOs.StructuralAnalysis.Contracts.Common.Point ToContract(
    //    BeamOs.StructuralAnalysis.Domain.Common.Point source
    //)
    //{
    //    return new(
    //        this.UnitsNetToContractMapper(source.X),
    //        this.UnitsNetToContractMapper(source.Y),
    //        this.UnitsNetToContractMapper(source.Z)
    //    );
    //}
}
