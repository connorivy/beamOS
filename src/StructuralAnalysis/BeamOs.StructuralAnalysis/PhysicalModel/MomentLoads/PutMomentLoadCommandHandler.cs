using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

internal class PutMomentLoadCommandHandler(
    IMomentLoadRepository momentLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PutMomentLoadCommand, MomentLoadResponse>
{
    public async Task<Result<MomentLoadResponse>> ExecuteAsync(
        PutMomentLoadCommand command,
        CancellationToken ct = default
    )
    {
        MomentLoad momentLoad = command.ToDomainObject();
        await momentLoadRepository.Put(momentLoad);
        await unitOfWork.SaveChangesAsync(ct);

        return momentLoad.ToResponse();
    }
}

internal class BatchPutMomentLoadCommandHandler(
    IMomentLoadRepository momentLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        MomentLoadId,
        MomentLoad,
        BatchPutMomentLoadRequest,
        PutMomentLoadRequest
    >(momentLoadRepository, unitOfWork)
{
    protected override MomentLoad ToDomainObject(
        ModelId modelId,
        PutMomentLoadRequest putRequest
    ) => new PutMomentLoadCommand(modelId, putRequest).ToDomainObject();
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class PutMomentLoadCommandMapper
{
    public static partial MomentLoad ToDomainObject(this PutMomentLoadCommand command);

    public static partial MomentLoadResponse ToResponse(this PutMomentLoadCommand command);
}

internal readonly struct PutMomentLoadCommand : IModelResourceWithIntIdRequest<MomentLoadData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public MomentLoadData Body { get; init; }
    public int NodeId => this.Body.NodeId;
    public int LoadCaseId => this.Body.LoadCaseId;
    public TorqueContract Torque => this.Body.Torque;
    public Contracts.Common.Vector3 AxisDirection => this.Body.AxisDirection;

    public PutMomentLoadCommand() { }

    public PutMomentLoadCommand(ModelId modelId, PutMomentLoadRequest putMomentLoadRequest)
    {
        this.Id = putMomentLoadRequest.Id;
        this.ModelId = modelId;
        this.Body = putMomentLoadRequest;
    }
}
