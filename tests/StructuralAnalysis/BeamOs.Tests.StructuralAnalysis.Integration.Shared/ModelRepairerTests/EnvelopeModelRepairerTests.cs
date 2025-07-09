using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;
using FluentAssertions;

namespace BeamOs.Tests.StructuralAnalysis.Integration.ModelRepairerTests;

// [TestType(TestType.ModelRepair)]
[MethodDataSource(typeof(AssemblySetup), nameof(AssemblySetup.GetStructuralAnalysisApiClientV1))]
public class EnvelopeModelRepairerTests(IStructuralAnalysisApiClientV1 apiClient)
{
    [Before(HookType.Test)]
    public void BeforeClass()
    {
        // This is a workaround to ensure that the API client is initialized before any tests run.
        apiClient ??= AssemblySetup.StructuralAnalysisApiClient;
    }

    [Test]
    public async Task ProposeRepairs_MergesCloseNodes_AddsNodeProposal()
    {
        Guid modelId = Guid.NewGuid();
        var settings = ModelRepairerTestUtil.CreateDefaultModelSettings();
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
        proposal.ThrowIfError();

        var repairedModel = await ModelRepairerTestUtil.EnsureGlobalGeometricContraints(
            apiClient,
            modelId,
            proposal.Value?.Id ?? throw new InvalidOperationException("Proposal is null")
        );

        repairedModel.Nodes.Count.Should().Be(3);
        repairedModel
            .Nodes.First(n => n.Id is not 2 and not 3)
            .LocationPoint.Y.Should()
            .BeApproximately(0, 1e-6);
        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }

    [Test]
    public async Task NodesVeryCloseToColumn_ShouldSnapToColumn()
    {
        Guid modelId = Guid.NewGuid();
        var settings = ModelRepairerTestUtil.CreateDefaultModelSettings(false);
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
        proposal.ThrowIfError();

        var repairedModel = await ModelRepairerTestUtil.EnsureGlobalGeometricContraints(
            apiClient,
            modelId,
            proposal.Value?.Id ?? throw new InvalidOperationException("Proposal is null")
        );

        var internalNode = repairedModel.InternalNodes.FirstOrDefault(n => n.Id == 3);
        internalNode.Should().NotBeNull();
        internalNode.RatioAlongElement1d.Value.Should().BeApproximately(0.5, 1e-6);
        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }

    [Test]
    public async Task NearlyConvergingNodesInXYPlane_ShouldMergeOrSnapNodes()
    {
        Guid modelId = Guid.NewGuid();
        var settings = ModelRepairerTestUtil.CreateDefaultModelSettings();
        var builder = new BeamOsDynamicModelBuilder(modelId.ToString(), settings, "Test", "Test");

        builder.AddSectionProfileFromLibrary(1, "w12x26", StructuralCode.AISC_360_16);
        builder.AddMaterial(1, 345e6, 200e9);

        // Nodes very close to the central node
        builder.AddNode(2, 5.1, 5, 0);
        builder.AddNode(3, 5, 5.1, 0);
        builder.AddNode(4, 4.9, 5, 0);
        builder.AddNode(5, 5, 4.9, 0);
        // Outer nodes
        builder.AddNode(6, 10, 5, 0);
        builder.AddNode(7, 5, 10, 0);
        builder.AddNode(8, 0, 5, 0);
        builder.AddNode(9, 5, 0, 0);

        // Elements radiating from near the center
        builder.AddElement1d(1, 2, 6, 1, 1);
        builder.AddElement1d(2, 3, 7, 1, 1);
        builder.AddElement1d(3, 4, 8, 1, 1);
        builder.AddElement1d(4, 5, 9, 1, 1);

        await builder.CreateOnly(apiClient);

        var proposal = await apiClient.RepairModelAsync(
            modelId,
            "test nearly converging nodes in xy plane"
        );
        proposal.ThrowIfError();

        var repairedModel = await ModelRepairerTestUtil.EnsureGlobalGeometricContraints(
            apiClient,
            modelId,
            proposal.Value?.Id ?? throw new InvalidOperationException("Proposal is null")
        );

        repairedModel
            .Nodes.Count.Should()
            .Be(5, "Nodes should be merged or snapped to the center node");

        var centerNode = repairedModel.Nodes.SingleOrDefault(n =>
            n.Id is not 6 and not 7 and not 8 and not 9
        );
        centerNode.Should().NotBeNull();
        centerNode.LocationPoint.X.Should().BeApproximately(5, 1e-6);
        centerNode.LocationPoint.Y.Should().BeApproximately(5, 1e-6);
        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }

    // [Test]
    // public async Task ColumnWithNearbyBeam_ShouldSnapBeamNodeToColumn()
    // {
    //     Guid modelId = Guid.NewGuid();
    //     var settings = ModelRepairerTestUtil.CreateDefaultModelSettings(false);
    //     var builder = new BeamOsDynamicModelBuilder(modelId.ToString(), settings, "Test", "Test");

    //     builder.AddSectionProfileFromLibrary(1, "w12x26", StructuralCode.AISC_360_16);
    //     builder.AddMaterial(1, 345e6, 200e9);

    //     // Add a column
    //     builder.AddNode(1, 0, 0, 0);
    //     builder.AddNode(2, 0, 0, 9.9);
    //     builder.AddElement1d(1, 1, 2, 1, 1);

    //     // Add a beam with a node very close to the column
    //     builder.AddNode(3, -0.1, 0, 10); // Close to the column
    //     builder.AddNode(4, -5, 0, 10);
    //     builder.AddElement1d(2, 3, 4, 1, 1);

    //     // second beam on the other side
    //     builder.AddNode(5, 0.1, 0, 10); // Close to the column
    //     builder.AddNode(6, 5, 0, 10);
    //     builder.AddElement1d(3, 5, 6, 1, 1);

    //     await builder.CreateOnly(apiClient);

    //     var proposal = await apiClient.RepairModelAsync(modelId, "snap beam node to column");

    //     await ModelRepairerTestUtil.EnsureGlobalGeometricContraints(
    //         apiClient,
    //         modelId,
    //         proposal.Value?.Id ?? throw new InvalidOperationException("Proposal is null")
    //     );
    //     await TestUtils.Asserter.VerifyModelProposal(proposal);
    // }

    [Test]
    public async Task BraceBetweenTwoColumns_ButSlightlyOutOfPlane_ShouldSnapIntoPlane()
    {
        Guid modelId = Guid.NewGuid();
        var settings = ModelRepairerTestUtil.CreateDefaultModelSettings(false);
        var builder = new BeamOsDynamicModelBuilder(modelId.ToString(), settings, "Test", "Test");

        builder.AddSectionProfileFromLibrary(1, "w12x26", StructuralCode.AISC_360_16);
        builder.AddMaterial(1, 345e6, 200e9);

        // Add a column
        builder.AddNode(1, 0, 0, 0);
        builder.AddNode(2, 0, 0, 10);
        builder.AddElement1d(1, 1, 2, 1, 1);

        // second column
        builder.AddNode(3, 5, 0, 0);
        builder.AddNode(4, 5, 0, 10);
        builder.AddElement1d(2, 3, 4, 1, 1);

        // Add a brace that is slightly out of plane
        builder.AddNode(5, .1, 0.15, 7); // Slightly out of plane
        builder.AddNode(6, 4.9, 0.1, 1);
        builder.AddElement1d(3, 5, 6, 1, 1);

        await builder.CreateOnly(apiClient);

        var proposal = await apiClient.RepairModelAsync(modelId, "snap beam node to column");
        proposal.ThrowIfError();

        var repairedModel = await ModelRepairerTestUtil.EnsureGlobalGeometricContraints(
            apiClient,
            modelId,
            proposal.Value?.Id ?? throw new InvalidOperationException("Proposal is null")
        );

        repairedModel
            .InternalNodes.Count.Should()
            .Be(2, "Brace endpoints should be snapped to the columns");
        await TestUtils.Asserter.VerifyModelProposal(proposal);
    }
}

public static class ModelRepairerTestUtil
{
    public static ModelSettings CreateDefaultModelSettings(bool yAxisUp = true)
    {
        var unitSettings = new UnitSettingsContract
        {
            LengthUnit = LengthUnitContract.Meter,
            ForceUnit = ForceUnitContract.Newton,
            AngleUnit = AngleUnitContract.Radian,
        };
        var analysisSettings = new AnalysisSettings();
        var modelSettings = new ModelSettings(unitSettings, analysisSettings, yAxisUp);
        return modelSettings;
    }

    public static async Task<ModelResponse> EnsureGlobalGeometricContraints(
        IStructuralAnalysisApiClientV1 apiClient,
        Guid modelId,
        int modelProposalId
    )
    {
#if !RUNTIME
        var originalModel = await apiClient.GetModelAsync(modelId);
#endif
        await apiClient.AcceptModelProposalAsync(modelId, modelProposalId);
        var repairedModel = await apiClient.GetModelAsync(modelId);
        repairedModel.ThrowIfError();
#if !RUNTIME
        // Ensure that the repaired model maintains global geometric constraints
        EnsureGlobalGeometricContraints(
            originalModel.Value ?? throw new InvalidOperationException("Original model is null"),
            repairedModel.Value ?? throw new InvalidOperationException("Repaired model is null")
        );
#endif
        return repairedModel.Value;
    }

    public static void EnsureGlobalGeometricContraints(
        ModelResponse originalModel,
        ModelResponse repairedModel
    )
    {
        // elements whose start and end nodes have nearly the same x, y, or z coordinates should
        // still have the same x, y, or z coordinates after repair
        foreach (var element in originalModel.Element1ds)
        {
            var repairedElement = repairedModel.Element1ds.FirstOrDefault(e => e.Id == element.Id);
            if (repairedElement == null)
                continue;

            var startNode = originalModel.Nodes.FirstOrDefault(n => n.Id == element.StartNodeId);
            var endNode = originalModel.Nodes.FirstOrDefault(n => n.Id == element.EndNodeId);
            var repairedStartNode = repairedModel.Nodes.FirstOrDefault(n =>
                n.Id == repairedElement.StartNodeId
            );
            var repairedEndNode = repairedModel.Nodes.FirstOrDefault(n =>
                n.Id == repairedElement.EndNodeId
            );

            if (
                startNode == null
                || endNode == null
                || repairedStartNode == null
                || repairedEndNode == null
            )
            {
                // if any nodes are null, that means it is an internal node. I should probably make a way to check the location of
                // internal nodes via the api responses, but I also think the response objects should be dumb continue;
                // todo: fix the following logic to test internal node geometric constraints as well
                return;
            }

            if (Math.Abs(startNode.LocationPoint.X - endNode.LocationPoint.X) < 1e-6)
            {
                repairedEndNode
                    .LocationPoint.X.Should()
                    .BeApproximately(repairedStartNode.LocationPoint.X, 1e-6);
            }
            if (Math.Abs(startNode.LocationPoint.Y - endNode.LocationPoint.Y) < 1e-6)
            {
                repairedEndNode
                    .LocationPoint.Y.Should()
                    .BeApproximately(repairedStartNode.LocationPoint.Y, 1e-6);
            }
            if (Math.Abs(startNode.LocationPoint.Z - endNode.LocationPoint.Z) < 1e-6)
            {
                repairedEndNode
                    .LocationPoint.Z.Should()
                    .BeApproximately(repairedStartNode.LocationPoint.Z, 1e-6);
            }
        }
    }
}
