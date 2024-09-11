using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models.Mappers;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

public class ModelResponseMapper : IMapper<Model, ModelResponse>
{
    public ModelResponse Map(Model from) => from.ToResponse();
}
