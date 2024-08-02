using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class ModelResponseMapper : IMapper<Model, ModelResponse>
{
    public ModelResponse Map(Model from) => this.ToResponse(from);

    public partial ModelResponse ToResponse(Model model);
}
