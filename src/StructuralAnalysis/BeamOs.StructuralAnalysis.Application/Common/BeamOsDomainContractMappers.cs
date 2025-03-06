using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.Common;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class BeamOsDomainContractMappers
{
    public static BeamOs.StructuralAnalysis.Contracts.Common.Point ToContract(
        BeamOs.StructuralAnalysis.Domain.Common.Point source
    )
    {
        LengthUnit unit = source.X.Unit;
        return new(source.X.As(unit), source.Y.As(unit), source.Z.As(unit), unit.MapToContract());
    }

    //public static Dictionary<string, object> ToDict(CustomData customData) => customData.AsDict();

    public static UnitSettings ToDomain(this UnitSettingsContract source)
    {
        LengthUnit lengthUnit = source.LengthUnit.MapToLengthUnit();
        ForceUnit forceUnit = source.ForceUnit.MapToForceUnit();

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
    }

    public static partial Contracts.PhysicalModel.Model.ModelSettings ToContract(
        this Domain.PhysicalModel.ModelAggregate.ModelSettings source
    );

    public static UnitSettingsContract ToContract(this UnitSettings source)
    {
        return new()
        {
            LengthUnit = source.LengthUnit.MapToContract(),
            ForceUnit = source.ForceUnit.MapToContract()
        };
    }

    public static partial Contracts.PhysicalModel.Model.AnalysisSettings ToContract(
        this Domain.PhysicalModel.ModelAggregate.AnalysisSettings source
    );

    //public static BeamOs.StructuralAnalysis.Domain.Common.Point ToDomain(
    //    BeamOs.StructuralAnalysis.Contracts.Common.Point source
    //)
    //{
    //    return new(
    //        UnitsNetMappers.MapToContract(source.X),
    //        UnitsNetMappers.MapToContract(source.Y),
    //        UnitsNetMappers.MapToContract(source.Z)
    //    );
    //}

    public static MathNet.Spatial.Euclidean.Vector3D ToDomain(
        this BeamOs.StructuralAnalysis.Contracts.Common.Vector3 source
    )
    {
        return new(source.X, source.Y, source.Z);
    }
}
