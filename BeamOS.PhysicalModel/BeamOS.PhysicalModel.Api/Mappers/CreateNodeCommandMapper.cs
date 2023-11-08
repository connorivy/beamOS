using BeamOS.PhysicalModel.Application.Nodes.Commands;
using BeamOS.PhysicalModel.Contracts.Nodes;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

[Mapper]
public static partial class CreateNodeCommandMapper
{
    public static partial CreateNodeCommand ToCommand(this CreateNodeRequest request);
}
