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

        foreach (CreateAnalyticalNodeCommand nodeCommand in command.Nodes)
        {
            AnalyticalNode node = await createNodeHandler.ExecuteAsync(nodeCommand, ct);
            nodes.Add(nodeCommand.Id, node);
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
            SectionProfile sectionProfile = await createSectionProfileHandler.ExecuteAsync(
                sectionProfileCommand,
                ct
            );
            sectionProfiles.Add(sectionProfileCommand.Id, sectionProfile);
        }

        foreach (CreateAnalyticalElement1dCommand el1dCommand in command.Element1Ds)
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
