using BeamOs.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

[Mapper]
public partial class ModelResponseMapper : IMapper<Model, ModelResponse>
{
    public ModelResponse Map(Model from) => this.ToResponse(from);

    public partial ModelResponse ToResponse(Model model);
}
