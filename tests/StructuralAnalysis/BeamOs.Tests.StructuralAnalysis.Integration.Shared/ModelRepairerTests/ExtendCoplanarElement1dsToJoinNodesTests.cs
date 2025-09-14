using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.ModelRepairerTests;

[MethodDataSource(typeof(AssemblySetup), nameof(AssemblySetup.GetStructuralAnalysisApiClientV1))]
public class ExtendCoplanarElement1dsToJoinNodesTests(BeamOsResultApiClient apiClient)
{
    [Before(TUnitHookType.Test)]
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
        BeamOsDynamicModel builder = new(modelId, settings, "ExtendElement1dToNodeRule", "Test");

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
        var modelClient = apiClient.Models[modelId];

        var proposal = await modelClient.Repair.RepairModelAsync("snap beam node to column");

        var repairedModel = await ModelRepairerTestUtil.EnsureGlobalGeometricContraints(
            modelClient,
            modelId,
            proposal.Value?.Id ?? throw new InvalidOperationException("Proposal is null")
        );

        repairedModel
            .Nodes.Count.Should()
            .Be(3, "Nodes should be merged or snapped to the center node");

        var centerNode = repairedModel.Nodes.SingleOrDefault(n => n.Id is 2 or 3);
        centerNode.Should().NotBeNull();

        centerNode.LocationPoint.X.Should().BeApproximately(5, 1e-6);
        centerNode.LocationPoint.Y.Should().BeApproximately(0.0, 1e-6);

        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }
}
