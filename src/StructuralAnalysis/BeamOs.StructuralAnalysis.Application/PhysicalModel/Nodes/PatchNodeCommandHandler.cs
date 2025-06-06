using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public class PatchNodeCommandHandler(
    INodeDefinitionRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PatchNodeCommand, NodeResponse>
{
    public async Task<Result<NodeResponse>> ExecuteAsync(
        PatchNodeCommand command,
        CancellationToken ct = default
    )
    {
        var nodeDefinition = await nodeRepository.GetSingle(command.ModelId, command.Id, ct);
        if (nodeDefinition is null)
        {
            return BeamOsError.NotFound(
                description: $"Node with ID {command.Id} not found in model {command.ModelId}."
            );
        }
        if (nodeDefinition.CastToNodeIfApplicable() is not Node node)
        {
            return BeamOsError.InvalidOperation(description: "Cannot patch a non-node entity.");
        }

        if (command.LocationPoint is not null)
        {
            var lengthUnit = command.LocationPoint.Value.LengthUnit.MapToLengthUnit();
            node.LocationPoint = new(
                command.LocationPoint.Value.X ?? node.LocationPoint.X.As(lengthUnit),
                command.LocationPoint.Value.Y ?? node.LocationPoint.Y.As(lengthUnit),
                command.LocationPoint.Value.Z ?? node.LocationPoint.Z.As(lengthUnit),
                lengthUnit
            );
        }
        if (command.Restraint is not null)
        {
            node.Restraint = new(
                command.Restraint.Value.CanTranslateAlongX ?? node.Restraint.CanTranslateAlongX,
                command.Restraint.Value.CanTranslateAlongY ?? node.Restraint.CanTranslateAlongY,
                command.Restraint.Value.CanTranslateAlongZ ?? node.Restraint.CanTranslateAlongZ,
                command.Restraint.Value.CanRotateAboutX ?? node.Restraint.CanRotateAboutX,
                command.Restraint.Value.CanRotateAboutY ?? node.Restraint.CanRotateAboutY,
                command.Restraint.Value.CanRotateAboutZ ?? node.Restraint.CanRotateAboutZ
            );
        }

        await unitOfWork.SaveChangesAsync(ct);

        return node.ToResponse();
    }
}

public readonly struct PatchNodeCommand : IModelResourceRequest<UpdateNodeRequest>
{
    public Guid ModelId { get; init; }
    public UpdateNodeRequest Body { get; init; }
    public int Id => this.Body.Id;
    public PartialPoint? LocationPoint => this.Body.LocationPoint;
    public PartialRestraint? Restraint => this.Body.Restraint;
}
