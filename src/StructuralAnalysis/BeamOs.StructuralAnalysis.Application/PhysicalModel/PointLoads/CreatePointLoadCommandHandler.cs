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

public class CreatePointLoadCommandHandler(
    IPointLoadRepository pointLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreatePointLoadCommand, PointLoadResponse>
{
    public async Task<Result<PointLoadResponse>> ExecuteAsync(
        CreatePointLoadCommand command,
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
public static partial class CreatePointLoadCommandMapper
{
    public static partial PointLoad ToDomainObject(this CreatePointLoadCommand command);

    public static partial PointLoadResponse ToResponse(this PointLoad entity);
}

public readonly struct CreatePointLoadCommand : IModelResourceRequest<CreatePointLoadRequest>
{
    public Guid ModelId { get; init; }
    public CreatePointLoadRequest Body { get; init; }
    public int NodeId => this.Body.NodeId;
    public int LoadCaseId => this.Body.LoadCaseId;
    public ForceContract Force => this.Body.Force;
    public Contracts.Common.Vector3 Direction => this.Body.Direction;
    public int? Id => this.Body.Id;
}
