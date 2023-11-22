using BeamOS.Common.Application.Interfaces;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;
using BeamOS.DirectStiffnessMethod.Application.Materials;
using BeamOS.DirectStiffnessMethod.Application.SectionProfiles;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;

namespace BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;

public class CreateAnalyticalModelCommandHandler(
    ICommandHandler<ModelSettingsCommand, AnalyticalModelSettings> settingsCommandHandler,
    ICommandHandler<CreateAnalyticalNodeCommand, AnalyticalNode> createNodeHandler,
    ICommandHandler<CreateMaterialCommand, Material> createMaterialHandler,
    ICommandHandler<CreateSectionProfileCommand, SectionProfile> createSectionProfileHandler,
    ICommandHandler<CreateAnalyticalElement1dGivenEntitiesCommand, AnalyticalElement1D> createEl1dHandler)
    : ICommandHandler<CreateAnalyticalModelCommand, AnalyticalModel>
{
    public async Task<AnalyticalModel?> ExecuteAsync(CreateAnalyticalModelCommand command, CancellationToken ct = default)
    {
        AnalyticalModelSettings settings = await settingsCommandHandler.ExecuteAsync(command.Settings, ct);

        Dictionary<Guid, AnalyticalNode> nodes = [];
        Dictionary<string, Material> materials = [];
        Dictionary<string, SectionProfile> sectionProfiles = [];
        Dictionary<Guid, AnalyticalElement1D> element1ds = [];

        foreach (CreateAnalyticalNodeCommand nodeCommand in command.Nodes)
        {
            AnalyticalNode node = await createNodeHandler.ExecuteAsync(nodeCommand, ct);
            nodes.Add(node.Id.Value, node);
        }

        foreach (CreateMaterialCommand materialCommand in command.Materials)
        {
            if (materials.ContainsKey(materialCommand.Id))
            {
                continue;
            }
            Material material = await createMaterialHandler.ExecuteAsync(materialCommand, ct);
            materials.Add(materialCommand.Id, material);
        }

        foreach (CreateSectionProfileCommand sectionProfileCommand in command.SectionProfiles)
        {
            if (sectionProfiles.ContainsKey(sectionProfileCommand.Id))
            {
                continue;
            }
            SectionProfile sectionProfile = await createSectionProfileHandler.ExecuteAsync(sectionProfileCommand, ct);
            sectionProfiles.Add(sectionProfileCommand.Id, sectionProfile);
        }

        foreach (CreateAnalyticalElement1dCommand el1dCommand in command.Element1Ds)
        {
            CreateAnalyticalElement1dGivenEntitiesCommand commandWithEntities = new(
                el1dCommand.SectionProfileRotation,
                nodes[el1dCommand.StartNodeId.Id],
                nodes[el1dCommand.EndNodeId.Id],
                materials[el1dCommand.MaterialId],
                sectionProfiles[el1dCommand.SectionProfileId]);
            AnalyticalElement1D element1d = await createEl1dHandler.ExecuteAsync(commandWithEntities, ct);
            element1ds.Add(element1d.Id.Value, element1d);
        }

        var model = AnalyticalModel.RunAnalysis(settings.UnitSettings, element1ds.Values, nodes.Values);

        return model;
    }
}
