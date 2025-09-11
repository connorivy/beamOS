using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;

public class PutPointLoadCommandHandler(
    IPointLoadRepository pointLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PutPointLoadCommand, PointLoadResponse>
{
    public async Task<Result<PointLoadResponse>> ExecuteAsync(
        PutPointLoadCommand command,
        CancellationToken ct = default
    )
    {
        PointLoad pointLoad = command.ToDomainObject();
        await pointLoadRepository.Put(pointLoad);
        await unitOfWork.SaveChangesAsync(ct);

        return pointLoad.ToResponse();
    }
}

public class BatchPutPointLoadCommandHandler(
    IPointLoadRepository pointLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<PointLoadId, PointLoad, BatchPutPointLoadCommand, PutPointLoadRequest>(
        pointLoadRepository,
        unitOfWork
    )
{
    protected override PointLoad ToDomainObject(ModelId modelId, PutPointLoadRequest putRequest) =>
        new PutPointLoadCommand(modelId, putRequest).ToDomainObject();
}

[Mapper(PreferParameterlessConstructors = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class PutPointLoadCommandMapper
{
    public static partial PointLoad ToDomainObject(this PutPointLoadCommand command);
}

public readonly struct PutPointLoadCommand : IModelResourceWithIntIdRequest<PointLoadData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public PointLoadData Body { get; init; }
    public int NodeId => this.Body.NodeId;
    public int LoadCaseId => this.Body.LoadCaseId;
    public ForceContract Force => this.Body.Force;
    public Contracts.Common.Vector3 Direction => this.Body.Direction;

    public PutPointLoadCommand() { }

    public PutPointLoadCommand(ModelId modelId, PutPointLoadRequest putPointLoadRequest)
    {
        this.Id = putPointLoadRequest.Id;
        this.ModelId = modelId;
        this.Body = putPointLoadRequest;
    }
}

public readonly struct BatchPutPointLoadCommand : IModelResourceRequest<PutPointLoadRequest[]>
{
    public Guid ModelId { get; init; }
    public PutPointLoadRequest[] Body { get; init; }
}
