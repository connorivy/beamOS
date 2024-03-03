using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Materials;

public class CreateMaterialCommandHandler(IRepository<MaterialId, Material> materialRepository)
    : ICommandHandler<CreateMaterialCommand, Material>
{
    public async Task<Material> ExecuteAsync(CreateMaterialCommand command, CancellationToken ct)
    {
        var load = command.ToDomainObject();

        await materialRepository.Add(load);

        return load;
    }
}

[Mapper]
public static partial class CreateMaterialCommandMapper
{
    public static partial Material ToDomainObject(this CreateMaterialCommand command);
}
