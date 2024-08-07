using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Application.PhysicalModel.Nodes.Commands;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Nodes.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class CreateNodeRequestMapper : IMapper<CreateNodeRequest, CreateNodeCommand>
{
    public CreateNodeCommand Map(CreateNodeRequest from) => this.ToResponse(from);

    public partial CreateNodeCommand ToResponse(CreateNodeRequest model);
}
