using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Nodes.Commands;

public class CreateNodeCommandHandler(
    IRepository<NodeId, Node> nodeRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateNodeRequest, Node>
{
    public async Task<Node> ExecuteAsync(CreateNodeRequest command, CancellationToken ct)
    {
        var node = command.ToDomainObject();

        nodeRepository.Add(node);

        await unitOfWork.SaveChangesAsync(ct);

        return node;
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateNodeCommandMapper
{
    public static partial Node ToDomainObject(this CreateNodeRequest command);
}
