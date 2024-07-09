using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Models.Commands;

public class CreateModelCommandHandler(
    IRepository<ModelId, Model> modelRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateModelCommand, Model?>
{
    public async Task<Model?> ExecuteAsync(
        CreateModelCommand command,
        CancellationToken ct = default
    )
    {
        // todo: read before write can violate data integrity
        if (
            command.Id is not null
            && await modelRepository.GetById(new(Guid.Parse(command.Id)), ct) is not null
        )
        {
            return null;
        }
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

    public static ModelId ToId(string id)
    {
        return new(Guid.Parse(id));
    }
}
