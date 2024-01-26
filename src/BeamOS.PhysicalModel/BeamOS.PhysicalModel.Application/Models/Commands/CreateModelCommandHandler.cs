using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

public class CreateModelCommandHandler(IRepository<ModelId, Model> modelRepository)
    : ICommandHandler<CreateModelCommand, Model>
{
    public async Task<Model> ExecuteAsync(
        CreateModelCommand command,
        CancellationToken ct = default
    )
    {
        Model model = command.ToDomainObject();

        await modelRepository.Add(model);

        return model;
    }
}

[Mapper]
public static partial class CreateModelCommandMapper
{
    public static partial Model ToDomainObject(this CreateModelCommand command);
}
