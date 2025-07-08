using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration.ModelRepairerTests;

[MethodDataSource(typeof(AssemblySetup), nameof(AssemblySetup.GetStructuralAnalysisApiClientV1))]
public class ExtendCoplanarElement1dsToJoinNodesTests(IStructuralAnalysisApiClientV1 apiClient)
{
    [Before(HookType.Test)]
    public void BeforeClass()
    {
        // This is a workaround to ensure that the API client is initialized before any tests run.
        apiClient ??= AssemblySetup.StructuralAnalysisApiClient;
    }

    [Test]
    public async Task BeamsThatAlmostMeetAtAPoint_ButBothNeedToBeExtended_ShouldBeExtended()
    {
        // Arrange: Element1d A is horizontal, Element1d B is diagonal and nearly collinear/coplanar with A
        Guid modelId = Guid.NewGuid();
        ModelSettings settings = ModelRepairerTestUtil.CreateDefaultModelSettings(false);
        BeamOsDynamicModelBuilder builder = new(
            modelId.ToString(),
            settings,
            "ExtendElement1dToNodeRule",
            "Test"
        );

        builder.AddSectionProfileFromLibrary(1, "w12x26", StructuralCode.AISC_360_16);
        builder.AddMaterial(1, 345e6, 200e9);

        // element1d A: horizontal
        builder.AddNode(1, 0, 0, 0);
        builder.AddNode(2, 4.8, 0, 0);
        builder.AddElement1d(1, 1, 2, 1, 1);

        // element1d B: diagonal, endpoint nearly collinear/coplanar with A's end
        builder.AddNode(3, 5, .2, 0);
        builder.AddNode(4, 5, 5, 0);
        builder.AddElement1d(2, 3, 4, 1, 1);

        await builder.CreateOnly(apiClient);

        var proposal = await apiClient.RepairModelAsync(modelId, "snap beam node to column");

        await ModelRepairerTestUtil.EnsureGlobalGeometricContraints(
            apiClient,
            modelId,
            proposal.Value?.Id ?? throw new InvalidOperationException("Proposal is null")
        );
        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }
}
