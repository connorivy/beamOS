using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal class PatchNodeCommandHandler(
    INodeRepository nodeRepository,
    // INodeDefinitionRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<UpdateNodeRequest>, NodeResponse>
{
    public async Task<Result<NodeResponse>> ExecuteAsync(
        ModelResourceRequest<UpdateNodeRequest> command,
        CancellationToken ct = default
    )
    {
        var node = await nodeRepository.GetSingle(command.ModelId, command.Body.Id, ct);
        if (node is null)
        {
            return BeamOsError.NotFound(
                description: $"Node with ID {command.Body.Id} not found in model {command.ModelId}."
            );
        }

        if (command.Body.LocationPoint is not null)
        {
            var lengthUnit = command.Body.LocationPoint.Value.LengthUnit.MapToLengthUnit();
            node.LocationPoint = new(
                command.Body.LocationPoint.Value.X ?? node.LocationPoint.X.As(lengthUnit),
                command.Body.LocationPoint.Value.Y ?? node.LocationPoint.Y.As(lengthUnit),
                command.Body.LocationPoint.Value.Z ?? node.LocationPoint.Z.As(lengthUnit),
                lengthUnit
            );
        }
        if (command.Body.Restraint is not null)
        {
            node.Restraint = new(
                command.Body.Restraint.Value.CanTranslateAlongX
                    ?? node.Restraint.CanTranslateAlongX,
                command.Body.Restraint.Value.CanTranslateAlongY
                    ?? node.Restraint.CanTranslateAlongY,
                command.Body.Restraint.Value.CanTranslateAlongZ
                    ?? node.Restraint.CanTranslateAlongZ,
                command.Body.Restraint.Value.CanRotateAboutX ?? node.Restraint.CanRotateAboutX,
                command.Body.Restraint.Value.CanRotateAboutY ?? node.Restraint.CanRotateAboutY,
                command.Body.Restraint.Value.CanRotateAboutZ ?? node.Restraint.CanRotateAboutZ
            );
        }

        await unitOfWork.SaveChangesAsync(ct);

        return node.ToResponse();
    }
}
