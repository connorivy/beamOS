using BeamOS.PhysicalModel.Contracts;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

[Mapper]
public static partial class CreateModelCommandMapper
{
    public static partial CreateModelCommand ToCommand(this CreateModelRequest request);
}
