using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Application.Nodes.Commands;
using BeamOS.PhysicalModel.Contracts.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

[Mapper]
public partial class CreateNodeRequestMapper : IMapper<CreateNodeRequest, CreateNodeCommand>
{
    public CreateNodeCommand Map(CreateNodeRequest from) => this.ToResponse(from);
    public partial CreateNodeCommand ToResponse(CreateNodeRequest model);
}
