using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;
using LoadCase = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases.LoadCase;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

public class CreateLoadCaseCommandHandler(
    ILoadCaseRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateLoadCaseCommand, LoadCase>
{
    public async Task<Result<LoadCase>> ExecuteAsync(
        CreateLoadCaseCommand command,
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
public static partial class CreateLoadCaseCommandMapper
{
    public static partial Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        this CreateLoadCaseCommand command
    );

    public static partial LoadCase ToResponse(this Domain.PhysicalModel.LoadCases.LoadCase entity);

    public static partial Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        this PutLoadCaseCommand entity
    );
}
