using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

public class CreateMomentLoadCommandHandler(
    IMomentLoadRepository momentLoadRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateMomentLoadCommand, MomentLoadResponse>
{
    public async Task<Result<MomentLoadResponse>> ExecuteAsync(
        CreateMomentLoadCommand command,
        CancellationToken ct = default
    )
    {
        MomentLoad momentLoad = command.ToDomainObject();
        momentLoadRepository.Add(momentLoad);
        await unitOfWork.SaveChangesAsync(ct);

        return momentLoad.ToResponse();
    }
}

[Mapper(PreferParameterlessConstructors = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateMomentLoadCommandMapper
{
    public static partial MomentLoad ToDomainObject(this CreateMomentLoadCommand command);

    public static partial MomentLoadResponse ToResponse(this MomentLoad entity);
}

public readonly struct CreateMomentLoadCommand : IModelResourceRequest<CreateMomentLoadRequest>
{
    public Guid ModelId { get; init; }
    public CreateMomentLoadRequest Body { get; init; }
    public int NodeId => this.Body.NodeId;
    public int LoadCaseId => this.Body.LoadCaseId;
    public TorqueContract Torque => this.Body.Torque;
    public Contracts.Common.Vector3 AxisDirection => this.Body.AxisDirection;
    public int? Id => this.Body.Id;
}
