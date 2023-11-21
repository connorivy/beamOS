using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Model;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

[Mapper]
public partial class CreateModelRequestMapper : IMapper<CreateModelRequest, CreateModelCommand>
{
    public CreateModelCommand Map(CreateModelRequest from) => this.ToCommand(from);
    public partial CreateModelCommand ToCommand(CreateModelRequest request);
}
