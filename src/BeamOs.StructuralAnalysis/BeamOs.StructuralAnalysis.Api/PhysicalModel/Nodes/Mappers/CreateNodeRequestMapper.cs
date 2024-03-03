using BeamOs.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Application.Nodes.Commands;
using BeamOS.PhysicalModel.Contracts.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Nodes.Mappers;

[Mapper]
public partial class CreateNodeRequestMapper : IMapper<CreateNodeRequest, CreateNodeCommand>
{
    public CreateNodeCommand Map(CreateNodeRequest from) => this.ToResponse(from);

    public partial CreateNodeCommand ToResponse(CreateNodeRequest model);
}
