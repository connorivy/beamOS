using BeamOS.PhysicalModel.Application.Element1Ds;
using BeamOS.PhysicalModel.Contracts.Element1D;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

[Mapper]
public static partial class CreateElement1DCommandMapper
{
    public static partial CreateElement1DCommand ToCommand(this CreateElement1DRequest request);
}
