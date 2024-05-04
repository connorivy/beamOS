using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Models.Commands;

public class CreateModelCommandHandler(
    IRepository<ModelId, Model> modelRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateModelCommand, Model>
{
    public async Task<Model> ExecuteAsync(
        CreateModelCommand command,
        CancellationToken ct = default
    )
    {
        var model = command.ToDomainObject();

        modelRepository.Add(model);

        await unitOfWork.SaveChangesAsync(ct);

        return model;
    }
}

[Mapper]
public static partial class CreateModelCommandMapper
{
    public static partial Model ToDomainObject(this CreateModelCommand command);
}
