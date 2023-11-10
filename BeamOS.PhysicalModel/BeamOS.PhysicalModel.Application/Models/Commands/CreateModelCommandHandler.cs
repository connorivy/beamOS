using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

public class CreateModelCommandHandler : ICommandHandler<CreateModelCommand, Model>
{
    public async Task<Model> ExecuteAsync(CreateModelCommand command, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        Model model = command.ToDomainObject();

        // TODO : persist model

        return model;
    }
}

[Mapper]
public static partial class CreateModelCommandMapper
{
    public static partial Model ToDomainObject(this CreateModelCommand command);
}
