using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

internal class CreateElement1dCommandHandler(
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<CreateElement1dRequest>, Element1dResponse>
{
    public async Task<Result<Element1dResponse>> ExecuteAsync(
        ModelResourceRequest<CreateElement1dRequest> command,
        CancellationToken ct = default
    )
    {
        Element1d element1d = command.ToDomainObject();
        element1dRepository.Add(element1d);
        await unitOfWork.SaveChangesAsync(ct);

        return element1d.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreateElement1dCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial Element1d ToDomainObject(
        this ModelResourceRequest<CreateElement1dRequest> command
    );

    public static partial Element1dResponse ToResponse(this Element1d entity);
}
