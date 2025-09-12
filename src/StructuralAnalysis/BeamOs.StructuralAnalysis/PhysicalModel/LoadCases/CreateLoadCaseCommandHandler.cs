using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;

internal class CreateLoadCaseCommandHandler(
    ILoadCaseRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<LoadCaseData>, LoadCaseContract>
{
    public async Task<Result<LoadCaseContract>> ExecuteAsync(
        ModelResourceRequest<LoadCaseData> command,
        CancellationToken ct = default
    )
    {
        Domain.PhysicalModel.LoadCases.LoadCase element1d = command.ToDomainObject();
        element1dRepository.Add(element1d);
        await unitOfWork.SaveChangesAsync(ct);

        return element1d.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreateLoadCaseCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        this ModelResourceRequest<LoadCaseData> command
    );

    public static partial LoadCaseContract ToResponse(
        this Domain.PhysicalModel.LoadCases.LoadCase entity
    );

    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        this ModelResourceWithIntIdRequest<LoadCaseData> entity
    );
}
