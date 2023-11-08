using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Models.Commands;

public class CreateModelCommandHandler : ICommandHandler<CreateModelCommand, AnalyticalModel>
{
    public async Task<AnalyticalModel> ExecuteAsync(CreateModelCommand command, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        AnalyticalModel model = command.ToDomainObject();

        // TODO : persist model

        return model;
    }
}

[Mapper]
public static partial class CreateModelCommandMapper
{
    public static partial AnalyticalModel ToDomainObject(this CreateModelCommand command);
}
