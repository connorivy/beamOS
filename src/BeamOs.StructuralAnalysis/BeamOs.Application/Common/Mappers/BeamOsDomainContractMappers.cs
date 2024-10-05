using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.Common.UnitOperators;
using BeamOs.Common.Domain.Models;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using UnitsNet.Units;

namespace BeamOs.Application.Common.Mappers;

public static class BeamOsDomainContractMappers
{
    public static ModelId ToModelId(string modelId) => new(Guid.Parse(modelId));

    public static NodeId ToNodeId(string id) => new(Guid.Parse(id));

    public static Element1DId ToElement1DId(string id) => new(Guid.Parse(id));

    public static MaterialId ToMaterialId(string id) => new(Guid.Parse(id));

    public static SectionProfileId ToSectionProfileId(string id) => new(Guid.Parse(id));

    public static PointLoadId ToPointLoadId(string id) => new(Guid.Parse(id));

    public static MomentLoadId ToMomentLoadId(string id) => new(Guid.Parse(id));

    public static BeamOs.Contracts.PhysicalModel.Node.Point ToContract(
        BeamOs.Domain.Common.ValueObjects.Point source
    )
    {
        return new(
            source.XCoordinate.MapToContract2(),
            source.YCoordinate.MapToContract2(),
            source.ZCoordinate.MapToContract2()
        );
    }

    public static Dictionary<string, object> ToDict(CustomData customData) => customData.AsDict();

    public static UnitSettings ToDomain(this UnitSettingsContract source)
    {
        LengthUnit lengthUnit = source.LengthUnit.MapToLengthUnit();
        ForceUnit forceUnit = source.ForceUnit.MapToForceUnit();
        //AngleUnit angleUnit = source.AngleUnit.MapToAngleUnit();

        return new UnitSettings(
            lengthUnit,
            lengthUnit.ToArea(),
            lengthUnit.ToVolume(),
            forceUnit,
            forceUnit.DivideBy(lengthUnit),
            forceUnit.MultiplyBy(lengthUnit),
            forceUnit.GetPressure(lengthUnit),
            lengthUnit.ToAreaMomentOfInertiaUnit()
        );
        //LengthUnit lengthUnit,
        //AreaUnit areaUnit,
        //VolumeUnit volumeUnit,
        //ForceUnit forceUnit,
        //ForcePerLengthUnit forcePerLengthUnit,
        //TorqueUnit torqueUnit,
        //PressureUnit pressureUnit,
        //AreaMomentOfInertiaUnit areaMomentOfInertiaUnit
    }

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
