using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

public class CreateLoadCombinationCommandHandler(
    ILoadCombinationRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateLoadCombinationCommand, LoadCombinationContract>
{
    public async Task<Result<LoadCombinationContract>> ExecuteAsync(
        CreateLoadCombinationCommand command,
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
public static partial class CreateLoadCombinationCommandMapper
{
    public static partial LoadCombination ToDomainObject(this CreateLoadCombinationCommand command);

    public static partial LoadCombinationContract ToResponse(this LoadCombination entity);

    public static partial LoadCombination ToDomainObject(this PutLoadCombinationCommand entity);
}
