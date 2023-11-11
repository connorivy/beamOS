using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Application.Common.Interfaces;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

public class GetModelCommandHandler(IRepository<ModelId, Model> modelRepository)
    : ICommandHandler<GetModelCommand, Model?>
{
    public async Task<Model?> ExecuteAsync(GetModelCommand command, CancellationToken ct = default)
    {
        return await modelRepository.GetById(new ModelId(command.Id.Id));
    }
}
