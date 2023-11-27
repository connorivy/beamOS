using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Application.Materials;
public class CreateMaterialCommandHandler()
    : ICommandHandler<CreateMaterialCommand, Material>
{
    public Task<Material> ExecuteAsync(CreateMaterialCommand command, CancellationToken ct)
    {
        Material node = command.ToDomainObject();

        return Task.FromResult(node);
    }
}

[Mapper]
public static partial class CreateMaterialCommandMapper
{
    public static partial Material ToDomainObject(this CreateMaterialCommand command);
}
