using BeamOs.Application.PhysicalModel.Models.Commands;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.PhysicalModel.Models.Mappers;

[Mapper]
public partial class CreateModelRequestMapper : IMapper<CreateModelRequest, CreateModelCommand>
{
    public CreateModelCommand Map(CreateModelRequest from) => this.ToCommand(from);

    public partial CreateModelCommand ToCommand(CreateModelRequest request);
}
