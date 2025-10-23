using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal class InMemoryNodeRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage,
    IPointLoadRepository pointLoadRepository,
    IMomentLoadRepository momentLoadRepository,
    IServiceProvider serviceProvider
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
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage,
    INodeRepository nodeRepository,
    IInternalNodeRepository internalNodeRepository,
    IServiceProvider serviceProvider
)
    : InMemoryModelResourceRepository<NodeId, NodeDefinition>(inMemoryModelRepositoryStorage),
        INodeDefinitionRepository
{
    public override async Task RemoveById(
        ModelId modelId,
        NodeId id,
        CancellationToken ct = default
    )
    {
        await base.RemoveById(modelId, id, ct);
        await nodeRepository.RemoveById(modelId, id, ct);
        await internalNodeRepository.RemoveById(modelId, id, ct);

        var element1dRepository = serviceProvider.GetRequiredService<IElement1dRepository>();
        var els = await element1dRepository.GetMany(modelId, default(List<Element1dId>), ct);
        var elsToRemove = els.Where(el => el.StartNodeId == id || el.EndNodeId == id).ToList();
        foreach (var el in elsToRemove)
        {
            await element1dRepository.RemoveById(modelId, el.Id, ct);
        }
    }

    public override async ValueTask Put(NodeDefinition aggregate)
    {
        try
        {
            await base.Put(aggregate);
            return;
        }
        catch (KeyNotFoundException) { }

        if (aggregate is Node node)
        {
            try
            {
                await nodeRepository.Put(node);
            }
            catch (KeyNotFoundException)
            {
                // this means that the node was internal, but now is being changed to a regular node
                // so we need to remove it from the internal node repository
                nodeRepository.Add(node);
                await internalNodeRepository.RemoveById(node.ModelId, node.Id);
            }
        }
        else if (aggregate is InternalNode internalNode)
        {
            try
            {
                await internalNodeRepository.Put(internalNode);
            }
            catch (KeyNotFoundException)
            {
                // this means that the internal node was a regular node, but now is being changed to an internal node
                // so we need to remove it from the regular node repository
                internalNodeRepository.Add(internalNode);
                await nodeRepository.RemoveById(internalNode.ModelId, internalNode.Id);
            }
        }
        else
        {
            throw new NotSupportedException(
                $"NodeDefinition of type {aggregate.GetType().Name} is not supported."
            );
        }
    }
}

internal class InMemoryInternalNodeRepository(
    InMemoryModelRepositoryStorage inMemoryModelRepositoryStorage
)
    : InMemoryModelResourceRepository<NodeId, InternalNode>(inMemoryModelRepositoryStorage),
        IInternalNodeRepository { }
