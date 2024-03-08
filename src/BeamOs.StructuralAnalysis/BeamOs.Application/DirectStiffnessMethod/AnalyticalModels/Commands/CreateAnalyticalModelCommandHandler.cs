using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalElement1ds.Commands;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalModels.Commands;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalNodes.Commands;
using BeamOS.DirectStiffnessMethod.Application.Materials;
using BeamOS.DirectStiffnessMethod.Application.SectionProfiles;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate;

namespace BeamOs.Application.DirectStiffnessMethod.AnalyticalModels.Commands;

public class CreateAnalyticalModelCommandHandler(
    CreateAnalyticalModelSettingsCommandHandler settingsCommandHandler,
    CreateAnalyticalNodeCommandHandler createNodeHandler,
    CreateMaterialCommandHandler createMaterialHandler,
    CreateSectionProfileCommandHandler createSectionProfileHandler,
    CreateAnalyticalElement1dGivenEntitiesCommandHandler createEl1dHandler
) : ICommandHandler<CreateAnalyticalModelFromPhysicalModelCommand, AnalyticalModel?>
{
    public async Task<AnalyticalModel?> ExecuteAsync(
        CreateAnalyticalModelFromPhysicalModelCommand command,
        CancellationToken ct = default
    )
    {
        AnalyticalModelSettings settings = await settingsCommandHandler.ExecuteAsync(
            command.Settings,
            ct
        );

        Dictionary<string, AnalyticalNode> nodes = [];
        Dictionary<string, Material> materials = [];
        Dictionary<string, SectionProfile> sectionProfiles = [];
        List<AnalyticalElement1D> element1ds = [];

        foreach (var nodeCommand in command.Nodes)
        {
            AnalyticalNode node = await createNodeHandler.ExecuteAsync(nodeCommand, ct);
            nodes.Add(nodeCommand.Id, node);
        }

        foreach (var materialCommand in command.Materials)
        {
            if (materials.ContainsKey(materialCommand.Id))
            {
                continue;
            }
            Material material = await createMaterialHandler.ExecuteAsync(materialCommand, ct);
            materials.Add(materialCommand.Id, material);
        }

        foreach (var sectionProfileCommand in command.SectionProfiles)
        {
            if (sectionProfiles.ContainsKey(sectionProfileCommand.Id))
            {
                continue;
            }
            SectionProfile sectionProfile = await createSectionProfileHandler.ExecuteAsync(
                sectionProfileCommand,
                ct
            );
            sectionProfiles.Add(sectionProfileCommand.Id, sectionProfile);
        }

        foreach (var el1dCommand in command.Element1Ds)
        {
            CreateAnalyticalElement1dGivenEntitiesCommand commandWithEntities =
                new(
                    el1dCommand.SectionProfileRotation,
                    nodes[el1dCommand.StartNodeId.Id.ToString()],
                    nodes[el1dCommand.EndNodeId.Id.ToString()],
                    materials[el1dCommand.MaterialId],
                    sectionProfiles[el1dCommand.SectionProfileId]
                );
            AnalyticalElement1D element1d = await createEl1dHandler.ExecuteAsync(
                commandWithEntities,
                ct
            );
            element1ds.Add(element1d);
        }

        var model = AnalyticalModel.RunAnalysis(settings.UnitSettings, element1ds, nodes.Values);

        return model;
    }
}
