using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

// [TestType(TestType.ModelRepair)]
[MethodDataSource(typeof(AssemblySetup), nameof(AssemblySetup.GetStructuralAnalysisApiClientV1))]
public class ModelRepairerTests(IStructuralAnalysisApiClientV1 apiClient)
{
    [Before(HookType.Test)]
    public void BeforeClass()
    {
        // This is a workaround to ensure that the API client is initialized before any tests run.
        apiClient ??= AssemblySetup.StructuralAnalysisApiClient;
    }

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

        await builder.CreateOnly(apiClient);

        var proposal = await apiClient.RepairModelAsync(modelId, "this doesn't do anything yet");

        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }

    [Test]
    public async Task NodesVeryCloseToColumn_ShouldSnapToColumn()
    {
        Guid modelId = Guid.NewGuid();
        var settings = CreateDefaultModelSettings();
        var builder = new BeamOsDynamicModelBuilder(modelId.ToString(), settings, "Test", "Test");

        builder.AddSectionProfileFromLibrary(1, "w12x26", StructuralCode.AISC_360_16);
        builder.AddMaterial(1, 345e6, 200e9);

        // add a column
        builder.AddNode(1, 0, 0, 0);
        builder.AddNode(2, 0, 0, 10);
        builder.AddElement1d(1, 1, 2, 1, 1);

        // beam that has a node which is very close to the column
        builder.AddNode(3, 0.1, 0, 5);
        builder.AddNode(4, 5, 0, 5);
        builder.AddElement1d(2, 3, 4, 1, 1);

        await builder.CreateOnly(apiClient);

        var proposal = await apiClient.RepairModelAsync(modelId, "this doesn't do anything yet");

        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }
    // [Test]
    // public async Task ProposeRepairs_NoCloseNodes_NoNodeProposals()
    // {
    //     var modelId = Guid.NewGuid();
    //     var settings = CreateDefaultModelSettings();
    //     var builder = new BeamOsDynamicModelBuilder(modelId.ToString(), settings, "Test", "Test");
    //     builder.AddNode(1, 0, 0, 0);
    //     builder.AddNode(2, 10, 0, 0);

    //     await builder.CreateOnly(apiClient);

    //     var proposal = await apiClient.RepairModelAsync(modelId, "this doesn't do anything yet");

    //     await TestUtils.Asserter.VerifyModelProposal(proposal);
    // }
}
