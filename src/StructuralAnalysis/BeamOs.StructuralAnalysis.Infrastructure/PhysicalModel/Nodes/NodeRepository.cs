using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;

internal sealed class NodeRepository(StructuralAnalysisDbContext dbContext)
    : ModelResourceRepositoryBase<NodeId, Node>(dbContext),
        INodeRepository
{
    public Task<List<Node>> GetAll()
    {
        this.DbContext.ChangeTracker.Clear();
        return this.DbContext.Nodes.ToListAsync();
    }

    // this works, but the local version of the entity doesn't reflect the db version, so you'd still ahve
    // to query again if you want the correct version.
    //public Node Update(PatchNodeCommand patchCommand)
    //{
    //    Node node =
    //        new(
    //            patchCommand.ModelId,
    //            (
    //                patchCommand.LocationPoint
    //                ?? new() { LengthUnit = LengthUnitContract.Undefined }
    //            ).ToDomainObject(),
    //            (patchCommand.Restraint ?? new()).ToDomainObject(),
    //            patchCommand.Id
    //        );

    //    EntityEntry<Node> attachedNode = this.DbContext.Nodes.Attach(node);
    //    var p = attachedNode.ComplexProperty(n => n.LocationPoint).CurrentValue;
    //    //EntityEntry<Node> attachedNode = this.DbContext.Entry(node);

    //    if (patchCommand.LocationPoint is not null)
    //    {
    //        var currentAttachedProp = attachedNode.ComplexProperty(n => n.LocationPoint);
    //        //currentAttachedProp.IsModified = true;
    //        //attachedNode.Property(n => n.LocationPoint).IsModified = true;
    //        //var currentAttachedProp = this.DbContext.Entry(node.LocationPoint);
    //        if (patchCommand.LocationPoint.Value.X is not null)
    //        {
    //            currentAttachedProp.Property(n => n.X).IsModified = true;
    //        }
    //        if (patchCommand.LocationPoint.Value.Y is not null)
    //        {
    //            currentAttachedProp.Property(n => n.Y).IsModified = true;
    //        }
    //        else
    //        {
    //            currentAttachedProp.Property(n => n.Y).IsTemporary = true;
    //        }
    //        if (patchCommand.LocationPoint.Value.Z is not null)
    //        {
    //            currentAttachedProp.Property(n => n.Z).IsModified = true;
    //        }
    //    }

    //    //this.DbContext.Nodes.Where(n => n.ModelId == new ModelId()).up

    //    if (patchCommand.Restraint is not null)
    //    {
    //        var currentAttachedProp = attachedNode.ComplexProperty(n => n.Restraint);
    //        //currentAttachedProp.IsModified = true;
    //        if (patchCommand.Restraint.Value.CanTranslateAlongX is not null)
    //        {
    //            currentAttachedProp.Property(n => n.CanTranslateAlongX).IsModified = true;
    //        }
    //        if (patchCommand.Restraint.Value.CanTranslateAlongY is not null)
    //        {
    //            currentAttachedProp.Property(n => n.CanTranslateAlongY).IsModified = true;
    //        }
    //        if (patchCommand.Restraint.Value.CanTranslateAlongZ is not null)
    //        {
    //            currentAttachedProp.Property(n => n.CanTranslateAlongZ).IsModified = true;
    //        }
    //        if (patchCommand.Restraint.Value.CanRotateAboutX is not null)
    //        {
    //            currentAttachedProp.Property(n => n.CanRotateAboutX).IsModified = true;
    //        }
    //        if (patchCommand.Restraint.Value.CanRotateAboutY is not null)
    //        {
    //            currentAttachedProp.Property(n => n.CanRotateAboutY).IsModified = true;
    //        }
    //        if (patchCommand.Restraint.Value.CanRotateAboutZ is not null)
    //        {
    //            currentAttachedProp.Property(n => n.CanRotateAboutZ).IsModified = true;
    //        }
    //    }

    //    return node;
    //}

    public async Task<Node> Update(PatchNodeCommand patchCommand)
    {
        Node node = await this.DbContext
            .Nodes
            .FirstAsync(n => n.ModelId.Equals(patchCommand.ModelId) && n.Id == patchCommand.Id);

        if (patchCommand.LocationPoint is not null)
        {
            node.LocationPoint = new(
                patchCommand.LocationPoint.Value.X ?? node.LocationPoint.X.Value,
                patchCommand.LocationPoint.Value.Y ?? node.LocationPoint.Y.Value,
                patchCommand.LocationPoint.Value.Z ?? node.LocationPoint.Z.Value,
                patchCommand.LocationPoint.Value.LengthUnit.MapToLengthUnit()
            );
        }

        if (patchCommand.Restraint is not null)
        {
            node.Restraint = new(
                patchCommand.Restraint.Value.CanTranslateAlongZ
                    ?? node.Restraint.CanTranslateAlongX,
                patchCommand.Restraint.Value.CanTranslateAlongY
                    ?? node.Restraint.CanTranslateAlongY,
                patchCommand.Restraint.Value.CanTranslateAlongZ
                    ?? node.Restraint.CanTranslateAlongZ,
                patchCommand.Restraint.Value.CanRotateAboutX ?? node.Restraint.CanRotateAboutX,
                patchCommand.Restraint.Value.CanRotateAboutY ?? node.Restraint.CanRotateAboutY,
                patchCommand.Restraint.Value.CanRotateAboutZ ?? node.Restraint.CanRotateAboutZ
            );
        }

        return node;
    }
}

[Mapper(ThrowOnMappingNullMismatch = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class CreateNodeCommandMapper
{
    public static partial Domain.Common.Point ToDomainObject(this PartialPoint command);

    public static partial Domain.Common.Restraint ToDomainObject(this PartialRestraint command);
}
