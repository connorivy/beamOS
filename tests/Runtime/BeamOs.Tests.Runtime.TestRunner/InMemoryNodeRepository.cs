using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.Tests.Runtime.TestRunner;

public class InMemoryNodeRepository : InMemoryModelResourceRepository<NodeId, Node>, INodeRepository
{
    public Task<Node> Update(PatchNodeCommand patchCommand)
    {
        if (this.Entities.TryGetValue(patchCommand.Id, out var node))
        {
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
            return Task.FromResult(node);
        }

        throw new KeyNotFoundException($"Node with ID {patchCommand.Id} not found.");
    }

    public Task<List<Node>> GetAll()
    {
        return Task.FromResult(this.Entities.Values.ToList());
    }
}
