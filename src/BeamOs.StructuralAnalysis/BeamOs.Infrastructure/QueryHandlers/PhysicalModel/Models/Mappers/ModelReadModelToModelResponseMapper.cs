using BeamOs.Application.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

[Mapper]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
internal partial class ModelToModelResponseMapper
    : AbstractMapperProvidedUnits<Model, ModelResponse>
{
    [Obsolete("This is just here to make DI registration work. I'm too lazy to change it.", true)]
    public ModelToModelResponseMapper()
        : base(null) { }

    private ModelToModelResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static ModelToModelResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public override ModelResponse Map(Model source)
    {
        return this.ToResponse(source);
    }

    private partial ModelResponse ToResponse(Model source);
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
