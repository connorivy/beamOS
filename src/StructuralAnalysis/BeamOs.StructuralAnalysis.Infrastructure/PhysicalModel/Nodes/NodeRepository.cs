using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;

[Mapper(ThrowOnMappingNullMismatch = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class CreateNodeCommandMapper
{
    public static partial Domain.Common.Point ToDomainObject(this PartialPoint command);

    public static partial Domain.Common.Restraint ToDomainObject(this PartialRestraint command);
}

internal sealed class NodeDefinitionRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<NodeId, NodeDefinition>(dbContext),
        INodeDefinitionRepository
{
    public override ValueTask Put(NodeDefinition aggregate)
    {
        this.DbContext.NodeDefinitions.Update(aggregate);
        return ValueTask.CompletedTask;
    }
}

internal sealed class NodeRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<NodeId, Node>(dbContext),
        INodeRepository { }

internal sealed class InternalNodeRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<NodeId, InternalNode>(dbContext),
        IInternalNodeRepository { }

// internal sealed class InternalNodeRepository(StructuralAnalysisDbContext dbContext)
//     : ModelResourceRepositoryInBase<NodeId, NodeDefinition>(dbContext),
//         IInternalNodeRepository
// {
//     public void Add(InternalNode aggregate) => this.Add((NodeDefinition)aggregate);

//     public void Put(InternalNode aggregate) => this.Put((NodeDefinition)aggregate);

//     public Task ReloadEntity(InternalNode entity, CancellationToken ct = default) =>
//         this.DbContext.Entry(entity).ReloadAsync(ct);

//     public void Remove(InternalNode aggregate) => this.Remove((NodeDefinition)aggregate);
// }

// internal sealed class NodeRepository(StructuralAnalysisDbContext dbContext)
//     : ModelResourceRepositoryInBase<NodeId, NodeDefinition>(dbContext),
//         INodeRepository
// {
//     public void Add(Node aggregate) => this.Add((NodeDefinition)aggregate);

//     public void Put(Node aggregate) => this.Put((NodeDefinition)aggregate);

//     public Task ReloadEntity(Node entity, CancellationToken ct = default) =>
//         this.DbContext.Entry(entity).ReloadAsync(ct);

//     public void Remove(Node aggregate) => this.Remove((NodeDefinition)aggregate);
// }
