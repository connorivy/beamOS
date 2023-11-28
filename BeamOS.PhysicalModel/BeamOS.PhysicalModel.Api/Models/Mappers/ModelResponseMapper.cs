using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Api.Mappers;
[Mapper]
public partial class ModelResponseMapper : IMapper<Model, ModelResponse>
{
    public ModelResponse Map(Model from) => this.ToResponse(from);
    public partial ModelResponse ToResponse(Model model);
}
