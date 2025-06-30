using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;

public class CreateLoadCaseCommandHandler(
    ILoadCaseRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateLoadCaseCommand, LoadCaseContract>
{
    public async Task<Result<LoadCaseContract>> ExecuteAsync(
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

    public static partial LoadCaseContract ToResponse(
        this Domain.PhysicalModel.LoadCases.LoadCase entity
    );

    public static partial Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        this PutLoadCaseCommand entity
    );
}

public readonly struct CreateLoadCaseCommand : IModelResourceRequest<LoadCaseData>
{
    public Guid ModelId { get; init; }
    public LoadCaseData Body { get; init; }
    public string Name => this.Body.Name;
}
