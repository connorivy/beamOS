using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.ModelRepairerTests;

[MethodDataSource(typeof(AssemblySetup), nameof(AssemblySetup.GetStructuralAnalysisApiClientV1))]
public class ExtendElement1dToNodeRuleTests(BeamOsResultApiClient apiClient)
{
    [Before(TUnitHookType.Test)]
    public void BeforeClass()
    {
        // This is a workaround to ensure that the API client is initialized before any tests run.
        apiClient ??= AssemblySetup.StructuralAnalysisApiClient;
    }

    [Test]
    public async Task PerpendicularBeams_ShouldExtendToMeet()
    {
        // Arrange: Element1d A is horizontal, Element1d B is diagonal and nearly collinear/coplanar with A
        Guid modelId = Guid.NewGuid();
        ModelSettings settings = ModelRepairerTestUtil.CreateDefaultModelSettings(false);
        BeamOsDynamicModel builder = new(modelId, settings, "ExtendElement1dToNodeRule", "Test");

        builder.AddSectionProfileFromLibrary(1, "w12x26", StructuralCode.AISC_360_16);
        builder.AddMaterial(1, 345e6, 200e9);

        // Element1d A: horizontal
        builder.AddNode(1, 0, 0, 0);
        builder.AddNode(2, 5, 0, 0);
        builder.AddElement1d(1, 1, 2, 1, 1);

        // Element1d B: diagonal, endpoint nearly collinear/coplanar with A's end
        builder.AddNode(3, 0, .2, 0); // Should merge with node 1 after repair
        builder.AddNode(4, 0, 2, 0);
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
        var centerNode = repairedModel.Nodes.SingleOrDefault(n => n.Id is 1 or 3);

        repairedModel
            .Element1ds.First(el => el.Id == 1)
            .StartNodeId.Should()
            .Be(centerNode.Id, "Element1d A should extend to the center node");

        repairedModel
            .Element1ds.First(el => el.Id == 2)
            .StartNodeId.Should()
            .Be(centerNode.Id, "Element1d B should extend to the center node");

        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }

    [Test]
    public async Task BeamAtAngleFromOther_ShouldExtendToMeetOtherNode()
    {
        // Arrange: Element1d A is horizontal, Element1d B is diagonal and nearly collinear/coplanar with A
        Guid modelId = Guid.NewGuid();
        ModelSettings settings = ModelRepairerTestUtil.CreateDefaultModelSettings(false);
        BeamOsDynamicModel builder = new(modelId, settings, "ExtendElement1dToNodeRule", "Test");

        builder.AddSectionProfileFromLibrary(1, "w12x26", StructuralCode.AISC_360_16);
        builder.AddMaterial(1, 345e6, 200e9);

        // Element1d A: horizontal
        builder.AddNode(1, 0, 0, 0);
        builder.AddNode(2, 5, 0, 0);
        builder.AddElement1d(1, 1, 2, 1, 1);

        // Element1d B: diagonal, endpoint nearly collinear/coplanar with A's end
        builder.AddNode(3, 5.1, .2, 0); // Should merge with node 2 after repair
        builder.AddNode(4, 7, 2, 0);
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

        repairedModel
            .Element1ds.First(el => el.Id == 1)
            .EndNodeId.Should()
            .Be(centerNode.Id, "Element1d A should extend to the center node");

        repairedModel
            .Element1ds.First(el => el.Id == 2)
            .StartNodeId.Should()
            .Be(centerNode.Id, "Element1d B should extend to the center node");

        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }
}
