using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

internal partial class ModelReadModelToModelResponseMapper
    : AbstractMapper<ModelReadModel, ModelResponse>
{
    public override ModelResponse Map(ModelReadModel source)
    {
        var internalMapper = ModelReadModelToModelResponseMapper2.Create(
            source.Settings.UnitSettings
        );
        return internalMapper.Map(source);
    }
}

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
internal partial class ModelReadModelToModelResponseMapper2
    : AbstractMapperProvidedUnits<ModelReadModel, ModelResponse>
{
    [Obsolete("This is just here to make DI registration work. I'm too lazy to change it.", true)]
    public ModelReadModelToModelResponseMapper2()
        : base(null) { }

    private ModelReadModelToModelResponseMapper2(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static ModelReadModelToModelResponseMapper2 Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public override ModelResponse Map(ModelReadModel source)
    {
        return this.ToResponse(source);
    }

    private partial ModelResponse ToResponse(ModelReadModel source);
}

// todo : consider this interface if we need mappers with more specific units
//public interface IMapperWithUnits<TUnitValue, TUnitType>
//    where TUnitValue : IQuantity<TUnitType>
//    where TUnitType : Enum
//{
//    public TUnitType UnitType { get; init; }
//    public string UnitToStringMapper(TUnitType unitType);
//    public UnitValueDto UnitToUnitValueDtoMapper1234(TUnitValue unitValue)
//    {
//        return new UnitValueDto(
//            unitValue.As(this.UnitType),
//            this.UnitToStringMapper(this.UnitType)
//        );
//    }
//}
