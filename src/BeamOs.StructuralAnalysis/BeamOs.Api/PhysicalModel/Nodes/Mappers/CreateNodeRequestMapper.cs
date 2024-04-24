using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Nodes.Commands;
using BeamOs.Contracts.PhysicalModel.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Nodes.Mappers;

[Mapper]
public partial class CreateNodeRequestMapper : IMapper<CreateNodeRequest, CreateNodeCommand>
{
    public CreateNodeCommand Map(CreateNodeRequest from) => this.ToResponse(from);

    public partial CreateNodeCommand ToResponse(CreateNodeRequest model);
}
