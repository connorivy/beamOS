using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

internal class CreateMomentLoadCommandHandler(
    IMomentLoadRepository momentLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<CreateMomentLoadRequest>, MomentLoadResponse>
{
    public async Task<Result<MomentLoadResponse>> ExecuteAsync(
        ModelResourceRequest<CreateMomentLoadRequest> command,
        CancellationToken ct = default
    )
    {
        MomentLoad momentLoad = command.ToDomainObject();
        momentLoadRepository.Add(momentLoad);
        await unitOfWork.SaveChangesAsync(ct);

        return momentLoad.ToResponse();
    }
}

[Mapper(PreferParameterlessConstructors = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreateMomentLoadCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial MomentLoad ToDomainObject(
        this ModelResourceRequest<CreateMomentLoadRequest> command
    );

    public static partial MomentLoadResponse ToResponse(this MomentLoad entity);
}
