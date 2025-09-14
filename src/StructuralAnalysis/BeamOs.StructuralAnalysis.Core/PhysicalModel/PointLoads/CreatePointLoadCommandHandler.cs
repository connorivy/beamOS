using System.Numerics;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;

internal class CreatePointLoadCommandHandler(
    IPointLoadRepository pointLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<CreatePointLoadRequest>, PointLoadResponse>
{
    public async Task<Result<PointLoadResponse>> ExecuteAsync(
        ModelResourceRequest<CreatePointLoadRequest> command,
        CancellationToken ct = default
    )
    {
        PointLoad pointLoad = command.ToDomainObject();
        pointLoadRepository.Add(pointLoad);
        await unitOfWork.SaveChangesAsync(ct);

        return pointLoad.ToResponse();
    }
}

[Mapper(PreferParameterlessConstructors = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreatePointLoadCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial PointLoad ToDomainObject(
        this ModelResourceRequest<CreatePointLoadRequest> command
    );

    public static partial PointLoadResponse ToResponse(this PointLoad entity);
}
