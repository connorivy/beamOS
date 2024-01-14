using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.PhysicalModel.Application.Materials;

public class CreateMaterialCommandHandler(IRepository<MaterialId, Material> materialRepository)
    : ICommandHandler<CreateMaterialCommand, Material>
{
    public async Task<Material> ExecuteAsync(CreateMaterialCommand command, CancellationToken ct)
    {
        Material load = command.ToDomainObject();

        await materialRepository.Add(load);

        return load;
    }
}

[Mapper]
public static partial class CreateMaterialCommandMapper
{
    public static partial Material ToDomainObject(this CreateMaterialCommand command);
}
