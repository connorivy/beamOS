using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Element1dAggregate;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Application.PhysicalModel.Nodes.Commands;
using BeamOs.Application.PhysicalModel.PointLoads;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Commands;

public class CreateShearDiagramCommandHandler(
    IModelRepository modelRepository,
    //IShearDiagramRepository shearDiagramRepository,
    IPointLoadRepository pointLoadRepository,
    IElement1dRepository element1DRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateShearDiagramCommand, ShearForceDiagram>
{
    public async Task<ShearForceDiagram> ExecuteAsync(
        CreateShearDiagramCommand command,
        CancellationToken ct = default
    )
    {
        return null;
        //Element1D? element1D = await element1DRepository.GetById(
        //    new(Guid.Parse(command.Element1dId)),
        //    ct
        //);
        //var unitSettings = (await modelRepository.GetById(element1D.ModelId)).Settings.UnitSettings;

        //var pointLoads = await pointLoadRepository.GetPointLoadsBelongingToNodes(
        //    [element1D.StartNodeId, element1D.EndNodeId]
        //);

        //return ShearForceDiagram.Create(
        //    element1D,
        //    unitSettings.LengthUnit,
        //    unitSettings.ForceUnit,
        //    unitSettings.TorqueUnit,
        //    command.LinearCoordinateDirection3D
        //);
    }
}

[Mapper]
public static partial class CreateNodeCommandMapper
{
    public static partial Node ToDomainObject(this CreateNodeCommand command);
}
