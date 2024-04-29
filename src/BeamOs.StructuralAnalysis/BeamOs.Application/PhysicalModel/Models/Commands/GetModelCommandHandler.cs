using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Models.Commands;

public class GetModelCommandHandler(IRepository<ModelId, Model> modelRepository)
    : ICommandHandler<GetModelCommand, Model?>
{
    public async Task<Model?> ExecuteAsync(GetModelCommand command, CancellationToken ct = default)
    {
        return await modelRepository.GetById(new ModelId(command.Id.Id));
    }
}
