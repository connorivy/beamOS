using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public class CreateNodeCommandHandler(
    INodeRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateNodeCommand, NodeResponse>
{
    public async Task<Result<NodeResponse>> ExecuteAsync(
        CreateNodeCommand command,
        CancellationToken ct = default
    )
    {
        Node node = command.ToDomainObject();
        nodeRepository.Add(node);
        await unitOfWork.SaveChangesAsync(ct);

        return node.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateNodeCommandMapper
{
    public static partial Node ToDomainObject(this CreateNodeCommand command);

    public static partial CreateNodeRequest ToRequest(this CreateNodeCommand entity);

    public static partial NodeResponse ToResponse(this Node entity);

    [MapperIgnoreTarget(nameof(CreateNodeRequest.Id))]
    public static partial CreateNodeRequest ToRequest(this NodeResponse entity);
}

public readonly struct CreateNodeCommand : IModelResourceRequest<CreateNodeRequest>
{
    public Guid ModelId { get; init; }
    public CreateNodeRequest Body { get; init; }
    public int? Id => this.Body.Id;
    public Point LocationPoint => this.Body.LocationPoint;
    public Restraint? Restraint => this.Body.Restraint;
}
