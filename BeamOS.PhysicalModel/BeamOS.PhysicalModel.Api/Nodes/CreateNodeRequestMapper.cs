using BeamOS.PhysicalModel.Application.Nodes.Commands;
using BeamOS.PhysicalModel.Contracts.Node;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

[Mapper]
public static partial class CreateNodeRequestMapper
{
    public static partial CreateNodeCommand ToCommand(this CreateNodeRequest request);
}
