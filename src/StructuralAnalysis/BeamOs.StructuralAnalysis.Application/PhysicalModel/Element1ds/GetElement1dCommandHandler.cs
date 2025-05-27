using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public class GetElement1dCommandHandler(IElement1dRepository repository)
    : ICommandHandler<IModelEntity, Element1dResponse>
{
    public async Task<Result<Element1dResponse>> ExecuteAsync(
        IModelEntity query,
        CancellationToken ct = default
    )
    {
        var settingsAndElement = await repository.GetSingleWithModelSettings(
            query.ModelId,
            query.Id,
            ct
        );

        if (settingsAndElement is null)
        {
            return BeamOsError.NotFound(
                description: $"Element1d with id {query.Id} not found on model with id {query.ModelId}."
            );
        }

        var mapper = Element1dToResponseMapper.Create(
            settingsAndElement.Value.ModelSettings.UnitSettings
        );
        return mapper.Map(settingsAndElement.Value.Entity);
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappersJustEnums))]
internal partial class Element1dToResponseMapper
    : AbstractMapperProvidedUnits<Element1d, Element1dResponse>
{
    [Obsolete()]
    public Element1dToResponseMapper()
        : base(null) { }

    private Element1dToResponseMapper(UnitSettings unitSettings)
        : base(unitSettings) { }

    public static Element1dToResponseMapper Create(UnitSettings unitSettings)
    {
        return new(unitSettings);
    }

    public Element1dResponse Map(Element1d source) => this.ToResponse(source);

    private partial Element1dResponse ToResponse(Element1d source);
}
