using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

internal class CreateLoadCombinationCommandHandler(
    ILoadCombinationRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<LoadCombinationData>, LoadCombinationContract>
{
    public async Task<Result<LoadCombinationContract>> ExecuteAsync(
        ModelResourceRequest<LoadCombinationData> command,
        CancellationToken ct = default
    )
    {
        LoadCombination element1d = command.ToDomainObject();
        element1dRepository.Add(element1d);
        await unitOfWork.SaveChangesAsync(ct);

        return element1d.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreateLoadCombinationCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial LoadCombination ToDomainObject(
        this ModelResourceRequest<LoadCombinationData> command
    );

    public static partial LoadCombinationContract ToResponse(this LoadCombination entity);

    [MapNestedProperties(nameof(ModelResourceWithIntIdRequest<>.Body))]
    public static partial LoadCombination ToDomainObject(
        this ModelResourceWithIntIdRequest<LoadCombinationData> entity
    );
}
