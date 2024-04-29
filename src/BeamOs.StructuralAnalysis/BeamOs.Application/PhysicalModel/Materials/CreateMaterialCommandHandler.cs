using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Materials;

public class CreateMaterialCommandHandler(IRepository<MaterialId, Material> materialRepository)
    : ICommandHandler<CreateMaterialCommand, Material>
{
    public Task<Material> ExecuteAsync(CreateMaterialCommand command, CancellationToken ct)
    {
        var load = command.ToDomainObject();

        materialRepository.Add(load);

        return Task.FromResult(load);
    }
}

[Mapper]
public static partial class CreateMaterialCommandMapper
{
    public static partial Material ToDomainObject(this CreateMaterialCommand command);
}
