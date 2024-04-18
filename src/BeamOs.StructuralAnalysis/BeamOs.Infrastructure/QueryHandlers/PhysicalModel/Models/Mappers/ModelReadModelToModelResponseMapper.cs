using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Infrastructure.Data.Models;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models.Mappers;

internal partial class ModelReadModelToModelResponseMapper(
    IFlattenedModelSettingsToModelSettingsResponseMapper modelSettingsCommandMapper,
    NodeReadModelToHydratedResponseMapper nodeReadModelToHydratedResponseMapper
) : IMapper<ModelReadModel, ModelResponse>
{
    public ModelResponse Map(ModelReadModel source)
    {
        return new(
            source.Id.ToString(),
            source.Name,
            source.Description,
            modelSettingsCommandMapper.Map(source),
            Nodes: nodeReadModelToHydratedResponseMapper.Map(source.Nodes).ToList()
        );
    }
}
