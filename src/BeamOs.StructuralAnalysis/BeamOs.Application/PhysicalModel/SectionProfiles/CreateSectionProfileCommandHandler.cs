using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.SectionProfiles;

public class CreateSectionProfileCommandHandler(
    IRepository<SectionProfileId, SectionProfile> sectionProfileRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateSectionProfileCommand, SectionProfile>
{
    public async Task<SectionProfile> ExecuteAsync(
        CreateSectionProfileCommand command,
        CancellationToken ct = default
    )
    {
        var load = command.ToDomainObject();

        sectionProfileRepository.Add(load);

        await unitOfWork.SaveChangesAsync(ct);

        return load;
    }
}

[Mapper]
public static partial class CreateSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this CreateSectionProfileCommand command);
}
