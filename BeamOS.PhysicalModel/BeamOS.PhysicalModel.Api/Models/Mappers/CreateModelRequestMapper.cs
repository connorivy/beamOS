using BeamOS.PhysicalModel.Contracts.Model;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

[Mapper]
public static partial class CreateModelRequestMapper
{
    public static partial CreateModelCommand ToCommand(this CreateModelRequest request);
}
