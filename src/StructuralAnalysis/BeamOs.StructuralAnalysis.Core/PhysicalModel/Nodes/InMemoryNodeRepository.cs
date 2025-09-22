using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal class InMemoryNodeRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage,
    IPointLoadRepository pointLoadRepository,
    IMomentLoadRepository momentLoadRepository
) : InMemoryModelResourceRepository<NodeId, Node>(inMemoryModelRepositoryStorage), INodeRepository
{
    public override async Task<List<Node>> GetMany(
        ModelId modelId,
        IList<NodeId>? ids,
        CancellationToken ct = default
    )
    {
        var nodes = await base.GetMany(modelId, ids, ct);
        foreach (var node in nodes)
        {
            node.PointLoads =
            [
                .. (await pointLoadRepository.GetMany(modelId, null, ct)).Where(pl =>
                    pl.NodeId == node.Id
                ),
            ];
            node.MomentLoads =
            [
                .. (await momentLoadRepository.GetMany(modelId, null, ct)).Where(ml =>
                    ml.NodeId == node.Id
                ),
            ];
        }
        return nodes;
    }
    // public Task<Node> Update(PatchNodeCommand patchCommand)
    // {
    //     if (
    //         this.ModelResources.TryGetValue(patchCommand.ModelId, out var nodeCache)
    //         && nodeCache.TryGetValue(patchCommand.Id, out var nodeDefinition)
    //     )
    //     {
    //         if (nodeDefinition is Node node)
    //         {
    //             if (patchCommand.LocationPoint is not null)
    //             {
    //                 node.LocationPoint = new(
    //                     patchCommand.LocationPoint.Value.X ?? node.LocationPoint.X.Value,
    //                     patchCommand.LocationPoint.Value.Y ?? node.LocationPoint.Y.Value,
    //                     patchCommand.LocationPoint.Value.Z ?? node.LocationPoint.Z.Value,
    //                     patchCommand.LocationPoint.Value.LengthUnit.MapToLengthUnit()
    //                 );
    //             }

    //             if (patchCommand.Restraint is not null)
    //             {
    //                 node.Restraint = new(
    //                     patchCommand.Restraint.Value.CanTranslateAlongZ
    //                         ?? node.Restraint.CanTranslateAlongX,
    //                     patchCommand.Restraint.Value.CanTranslateAlongY
    //                         ?? node.Restraint.CanTranslateAlongY,
    //                     patchCommand.Restraint.Value.CanTranslateAlongZ
    //                         ?? node.Restraint.CanTranslateAlongZ,
    //                     patchCommand.Restraint.Value.CanRotateAboutX
    //                         ?? node.Restraint.CanRotateAboutX,
    //                     patchCommand.Restraint.Value.CanRotateAboutY
    //                         ?? node.Restraint.CanRotateAboutY,
    //                     patchCommand.Restraint.Value.CanRotateAboutZ
    //                         ?? node.Restraint.CanRotateAboutZ
    //                 );
    //             }
    //             return Task.FromResult(node);
    //         }
    //     }

    //     throw new KeyNotFoundException($"Node with ID {patchCommand.Id} not found.");
    // }

    // public Task<List<Node>> GetAll()
    // {
    //     return Task.FromResult(
    //         this.ModelResources.SelectMany(m => m.Value).Select(kvp => kvp.Value).ToList()
    //     );
    // }
}

internal class InMemoryNodeDefinitionRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<NodeId, NodeDefinition>(inMemoryModelRepositoryStorage),
        INodeDefinitionRepository { }

internal class InMemoryInternalNodeRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<NodeId, InternalNode>(inMemoryModelRepositoryStorage),
        IInternalNodeRepository { }
