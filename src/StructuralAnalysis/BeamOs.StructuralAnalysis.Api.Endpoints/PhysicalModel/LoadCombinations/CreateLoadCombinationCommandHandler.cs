using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;
using LoadCombination = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombination;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

public class CreateLoadCombinationCommandHandler(
    ILoadCombinationRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateLoadCombinationCommand, LoadCombination>
{
    public async Task<Result<LoadCombination>> ExecuteAsync(
        CreateLoadCombinationCommand command,
        CancellationToken ct = default
    )
    {
        Domain.PhysicalModel.LoadCombinations.LoadCombination element1d = command.ToDomainObject();
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
    public static partial Domain.PhysicalModel.LoadCombinations.LoadCombination ToDomainObject(
        this CreateLoadCombinationCommand command
    );

    public static partial LoadCombination ToResponse(
        this Domain.PhysicalModel.LoadCombinations.LoadCombination entity
    );

    public static partial Domain.PhysicalModel.LoadCombinations.LoadCombination ToDomainObject(
        this PutLoadCombinationCommand entity
    );
}
