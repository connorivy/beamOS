using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Element1dAggregate;
using BeamOs.Application.PhysicalModel.Materials;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Application.PhysicalModel.MomentLoads;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Application.PhysicalModel.PointLoads;
using BeamOs.Application.PhysicalModel.SectionProfiles;
using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.DirectStiffnessMethod.Services;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;

namespace BeamOs.Application.DirectStiffnessMethod.Commands;

public class RunDirectStiffnessMethodCommandHandler(
    IModelRepository modelRepository,
    INodeRepository nodeRepository,
    IElement1dRepository element1dRepository,
    IPointLoadRepository pointLoadRepository,
    IMomentLoadRepository momentLoadRepository,
    IMaterialRepository materialRepository,
    ISectionProfileRepository sectionProfileRepository
) : ICommandHandler<RunDirectStiffnessMethodCommand, ModelResults>
{
    public async Task<ModelResults> ExecuteAsync(
        RunDirectStiffnessMethodCommand command,
        CancellationToken ct = default
    )
    {
        UnitSettings unitSettings = (await modelRepository.GetById(command.ModelId, ct))
            .Settings
            .UnitSettings;

        List<Node> nodes = await nodeRepository.GetNodesInModel(command.ModelId, ct);
        List<Element1D> element1ds = await element1dRepository.GetElement1dsInModel(
            command.ModelId,
            ct
        );

        Dictionary<NodeId, Node> nodesMap = nodes.ToDictionary(n => n.Id, n => n);
        Dictionary<NodeId, List<PointLoad>> pointLoadsMap = (
            await pointLoadRepository.GetPointLoadsBelongingToNodes(nodes)
        )
            .GroupBy(x => x.NodeId)
            .ToDictionary(g => g.Key, g => g.ToList());
        Dictionary<NodeId, List<MomentLoad>> momentLoadsMap = (
            await momentLoadRepository.GetMomentLoadsBelongingToNodes(nodes)
        )
            .GroupBy(x => x.NodeId)
            .ToDictionary(g => g.Key, g => g.ToList());
        Dictionary<SectionProfileId, SectionProfile> sectionProfilesMap = (
            await sectionProfileRepository.GetSectionProfilesInModel(command.ModelId, ct)
        ).ToDictionary(s => s.Id, s => s);
        Dictionary<MaterialId, Material> materialsMap = (
            await materialRepository.GetMaterialsInModel(command.ModelId, ct)
        ).ToDictionary(m => m.Id, m => m);

        var dsmNodes = nodes.Select(
            n =>
                new DsmNodeVo(
                    n.Id,
                    n.LocationPoint,
                    n.Restraint,
                    pointLoadsMap.GetValueOrDefault(n.Id),
                    momentLoadsMap.GetValueOrDefault(n.Id)
                )
        );

        var dsmElement1ds = element1ds.Select(
            el =>
                new DsmElement1dVo(
                    el.SectionProfileRotation,
                    nodesMap[el.StartNodeId],
                    nodesMap[el.EndNodeId],
                    materialsMap[el.MaterialId],
                    sectionProfilesMap[el.SectionProfileId]
                )
        );

        return DirectStiffnessMethodSolver.RunAnalysis(unitSettings, dsmElement1ds, dsmNodes);
    }
}
