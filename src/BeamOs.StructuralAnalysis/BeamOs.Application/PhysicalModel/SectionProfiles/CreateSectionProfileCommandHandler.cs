using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.SectionProfiles;

public class CreateSectionProfileCommandHandler(
    IRepository<SectionProfileId, SectionProfile> sectionProfileRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateSectionProfileRequest, SectionProfile>
{
    public async Task<SectionProfile> ExecuteAsync(
        CreateSectionProfileRequest command,
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
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsIdMappers))]
public static partial class CreateSectionProfileCommandMapper
{
    public static partial SectionProfile ToDomainObject(this CreateSectionProfileRequest command);
}
