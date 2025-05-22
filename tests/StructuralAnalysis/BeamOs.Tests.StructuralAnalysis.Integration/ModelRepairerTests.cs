using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public class ModelRepairerTests
{
    private static ModelSettings CreateDefaultModelSettings()
    {
        var unitSettings = new UnitSettingsContract
        {
            LengthUnit = LengthUnitContract.Meter,
            ForceUnit = ForceUnitContract.Newton,
            AngleUnit = AngleUnitContract.Radian,
        };
        var analysisSettings = new AnalysisSettings();
        var modelSettings = new ModelSettings(unitSettings, analysisSettings, true);
        return modelSettings;
    }

    [Test]
    public async Task ProposeRepairs_MergesCloseNodes_AddsNodeProposal()
    {
        Guid modelId = Guid.NewGuid();
        var settings = CreateDefaultModelSettings();
        var builder = new BeamOsDynamicModelBuilder(modelId.ToString(), settings, "Test", "Test");
        builder.AddNode(1, 0, 0, 0);
        builder.AddNode(2, 5, 5, 0);
        builder.AddNode(3, 0, 5, 0);
        builder.AddNode(4, 0, 0.1, 0);

        builder.AddSectionProfileFromLibrary(1, "w12x26", StructuralCode.AISC_360_16);
        builder.AddMaterial(1, 345e6, 200e9);

        builder.AddElement1d(1, 1, 2, 1, 1);
        builder.AddElement1d(2, 2, 3, 1, 1);
        builder.AddElement1d(3, 3, 4, 1, 1);

        await builder.CreateOnly(AssemblySetup.StructuralAnalysisApiClient);

        var proposal = await AssemblySetup.StructuralAnalysisApiClient.RepairModelAsync(
            modelId,
            "this doesn't do anything yet"
        );

        await Verify(proposal);
    }

    [Test]
    public async Task ProposeRepairs_NoCloseNodes_NoNodeProposals()
    {
        var modelId = Guid.NewGuid();
        var settings = CreateDefaultModelSettings();
        var builder = new BeamOsDynamicModelBuilder(modelId.ToString(), settings, "Test", "Test");
        builder.AddNode(1, 0, 0, 0);
        builder.AddNode(2, 10, 0, 0);

        await builder.CreateOnly(AssemblySetup.StructuralAnalysisApiClient);

        var proposal = AssemblySetup.StructuralAnalysisApiClient.RepairModelAsync(
            modelId,
            "this doesn't do anything yet"
        );

        await Verify(proposal);
    }
}
