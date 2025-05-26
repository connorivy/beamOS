using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.Tests.Runtime.TestRunner;

public class InMemoryApiClient(
    InMemoryAcceptProposalCommandHandler inMemoryAcceptProposalCommandHandler,
    InMemoryBatchPutNodeCommandHandler inMemoryBatchPutNodeCommandHandler,
    InMemoryAddSectionProfileFromLibraryCommandHandler inMemoryAddSectionProfileFromLibraryCommandHandler,
    InMemoryBatchPutElement1dCommandHandler inMemoryBatchPutElement1dCommandHandler,
    InMemoryBatchPutLoadCaseCommandHandler inMemoryBatchPutLoadCaseCommandHandler,
    InMemoryBatchPutLoadCombinationCommandHandler inMemoryBatchPutLoadCombinationCommandHandler,
    InMemoryBatchPutMaterialCommandHandler inMemoryBatchPutMaterialCommandHandler,
    InMemoryBatchPutMomentLoadCommandHandler inMemoryBatchPutMomentLoadCommandHandler,
    InMemoryBatchPutPointLoadCommandHandler inMemoryBatchPutPointLoadCommandHandler,
    InMemoryBatchPutSectionProfileCommandHandler inMemoryBatchPutSectionProfileCommandHandler,
    InMemoryBatchPutSectionProfileFromLibraryCommandHandler inMemoryBatchPutSectionProfileFromLibraryCommandHandler,
    InMemoryCreateElement1dCommandHandler inMemoryCreateElement1dCommandHandler,
    InMemoryCreateLoadCaseCommandHandler inMemoryCreateLoadCaseCommandHandler,
    InMemoryCreateLoadCombinationCommandHandler inMemoryCreateLoadCombinationCommandHandler,
    InMemoryCreateMaterialCommandHandler inMemoryCreateMaterialCommandHandler,
    InMemoryCreateModelCommandHandler inMemoryCreateModelCommandHandler,
    // InMemoryCreateModelProposalCommandHandler inMemoryCreateModelProposalCommandHandler,
    InMemoryCreateMomentLoadCommandHandler inMemoryCreateMomentLoadCommandHandler,
    InMemoryCreateNodeCommandHandler inMemoryCreateNodeCommandHandler,
    InMemoryCreatePointLoadCommandHandler inMemoryCreatePointLoadCommandHandler,
    InMemoryCreateSectionProfileCommandHandler inMemoryCreateSectionProfileCommandHandler,
    // InMemoryDeleteAllResultSetsCommandHandler inMemoryDeleteAllResultSetsCommandHandler,
    InMemoryDeleteElement1dCommandHandler inMemoryDeleteElement1dCommandHandler,
    InMemoryDeleteLoadCaseCommandHandler inMemoryDeleteLoadCaseCommandHandler,
    InMemoryDeleteLoadCombinationCommandHandler inMemoryDeleteLoadCombinationCommandHandler,
    InMemoryDeleteMomentLoadCommandHandler inMemoryDeleteMomentLoadCommandHandler,
    InMemoryDeleteNodeCommandHandler inMemoryDeleteNodeCommandHandler,
    InMemoryDeletePointLoadCommandHandler inMemoryDeletePointLoadCommandHandler,
    InMemoryDeleteSectionProfileCommandHandler inMemoryDeleteSectionProfileCommandHandler,
    InMemoryGetDiagramsCommandHandler inMemoryGetDiagramsCommandHandler,
    // InMemoryGetElement1dCommandHandler inMemoryGetElement1dCommandHandler,
    InMemoryGetLoadCaseCommandHandler inMemoryGetLoadCaseCommandHandler,
    InMemoryGetLoadCombinationCommandHandler inMemoryGetLoadCombinationCommandHandler,
    // InMemoryGetModelCommandHandler inMemoryGetModelCommandHandler,
    // InMemoryGetModelProposalCommandHandler inMemoryGetModelProposalCommandHandler,
    // InMemoryGetModelProposalsCommandHandler inMemoryGetModelProposalsCommandHandler,
    // InMemoryGetModelsCommandHandler inMemoryGetModelsCommandHandler,
    // InMemoryGetNodeResultCommandHandler inMemoryGetNodeResultCommandHandler,
    // InMemoryGetResultSetCommandHandler inMemoryGetResultSetCommandHandler,
    // InMemoryGithubModelsChatCommandHandler inMemoryGithubModelsChatCommandHandler,
    InMemoryPutElement1dCommandHandler inMemoryPutElement1dCommandHandler,
    // InMemoryPutLoadCaseCommandHandler inMemoryPutLoadCaseCommandHandler,
    // InMemoryPutLoadCombinationCommandHandler inMemoryPutLoadCombinationCommandHandler,
    InMemoryPutMaterialCommandHandler inMemoryPutMaterialCommandHandler,
    InMemoryPutModelCommandHandler inMemoryPutModelCommandHandler,
    InMemoryPutMomentLoadCommandHandler inMemoryPutMomentLoadCommandHandler,
    InMemoryPutNodeCommandHandler inMemoryPutNodeCommandHandler,
    InMemoryPutPointLoadCommandHandler inMemoryPutPointLoadCommandHandler,
    InMemoryPutSectionProfileCommandHandler inMemoryPutSectionProfileCommandHandler,
    InMemoryPutSectionProfileFromLibraryCommandHandler inMemoryPutSectionProfileFromLibraryCommandHandler,
    // InMemoryRejectModelProposalCommandHandler inMemoryRejectModelProposalCommandHandler,
    InMemoryRepairModelCommandHandler inMemoryRepairModelCommandHandler
// InMemoryRunDirectStiffnessMethodCommandHandler inMemoryRunDirectStiffnessMethodCommandHandler,
// InMemoryRunOpenSeesAnalysisCommandHandler inMemoryRunOpenSeesAnalysisCommandHandler,
// InMemorySpeckleReceiveOperationCommandHandler inMemorySpeckleReceiveOperationCommandHandler,
// InMemoryUpdateNodeCommandHandler inMemoryUpdateNodeCommandHandler
) : IStructuralAnalysisApiClientV1
{
    public async Task<Result<ModelResponse>> AcceptModelProposalAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryAcceptProposalCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<SectionProfileResponse>> AddSectionProfileFromLibraryAsync(
        Guid modelId,
        SectionProfileFromLibraryData body = null,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryAddSectionProfileFromLibraryCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutElement1dAsync(
        Guid modelId,
        IEnumerable<PutElement1dRequest> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutElement1dCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutLoadCaseAsync(
        Guid modelId,
        IEnumerable<LoadCase> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutLoadCaseCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutLoadCombinationAsync(
        Guid modelId,
        IEnumerable<LoadCombination> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutLoadCombinationCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutMaterialAsync(
        Guid modelId,
        IEnumerable<PutMaterialRequest> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutMaterialCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutMomentLoadAsync(
        Guid modelId,
        IEnumerable<PutMomentLoadRequest> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutMomentLoadCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutNodeAsync(
        Guid modelId,
        IEnumerable<PutNodeRequest> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutNodeCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutPointLoadAsync(
        Guid modelId,
        IEnumerable<PutPointLoadRequest> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutPointLoadCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutSectionProfileAsync(
        Guid modelId,
        IEnumerable<PutSectionProfileRequest> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutSectionProfileCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<BatchResponse>> BatchPutSectionProfileFromLibraryAsync(
        Guid modelId,
        IEnumerable<SectionProfileFromLibrary> body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryBatchPutSectionProfileFromLibraryCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body.ToArray() },
            cancellationToken
        );

    public async Task<Result<Element1dResponse>> CreateElement1dAsync(
        Guid modelId,
        CreateElement1dRequest body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreateElement1dCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<LoadCase>> CreateLoadCaseAsync(
        Guid modelId,
        LoadCaseData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreateLoadCaseCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<LoadCombination>> CreateLoadCombinationAsync(
        Guid modelId,
        LoadCombinationData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreateLoadCombinationCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<MaterialResponse>> CreateMaterialAsync(
        Guid modelId,
        CreateMaterialRequest body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreateMaterialCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<ModelResponse>> CreateModelAsync(
        CreateModelRequest body = null,
        CancellationToken cancellationToken = default
    ) => await inMemoryCreateModelCommandHandler.ExecuteAsync(body, cancellationToken);

    public async Task<Result<ModelProposalResponse>> CreateModelProposalAsync(
        Guid modelId,
        ModelProposalData body = null,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreateModelProposalCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<MomentLoadResponse>> CreateMomentLoadAsync(
        Guid modelId,
        CreateMomentLoadRequest body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreateMomentLoadCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<NodeResponse>> CreateNodeAsync(
        Guid modelId,
        CreateNodeRequest body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreateNodeCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<PointLoadResponse>> CreatePointLoadAsync(
        Guid modelId,
        CreatePointLoadRequest body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreatePointLoadCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<SectionProfileResponse>> CreateSectionProfileAsync(
        Guid modelId,
        CreateSectionProfileRequest body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryCreateSectionProfileCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<int>> DeleteAllResultSetsAsync(
        Guid modelId,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryDeleteAllResultSetsCommandHandler.ExecuteAsync(
            new() { ModelId = modelId },
            cancellationToken
        );

    public async Task<Result<ModelEntityResponse>> DeleteElement1dAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryDeleteElement1dCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<ModelEntityResponse>> DeleteLoadCaseAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryDeleteLoadCaseCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<ModelEntityResponse>> DeleteLoadCombinationAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryDeleteLoadCombinationCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<ModelEntityResponse>> DeleteMomentLoadAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryDeleteMomentLoadCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<ModelEntityResponse>> DeleteNodeAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryDeleteNodeCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<ModelEntityResponse>> DeletePointLoadAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryDeletePointLoadCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<ModelEntityResponse>> DeleteSectionProfileAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryDeleteSectionProfileCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<AnalyticalResultsResponse>> GetDiagramsAsync(
        Guid modelId,
        int id,
        string? unitsOverride = null,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryGetDiagramsCommandHandler.ExecuteAsync(
            new()
            {
                ModelId = modelId,
                Id = id,
                UnitsOverride = unitsOverride,
            },
            cancellationToken
        );

    public async Task<Result<NodeResultResponse>> GetNodeResultAsync(
        Guid modelId,
        int resultSetId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryGetNodeResultCommandHandler.ExecuteAsync(
            new()
            {
                ModelId = modelId,
                ResultSetId = resultSetId,
                Id = id,
            },
            cancellationToken
        );

    public async Task<Result<Element1dResponse>> PutElement1dAsync(
        int id,
        Guid modelId,
        Element1dData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutElement1dCommandHandler.ExecuteAsync(
            new PutElement1dCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<LoadCase>> PutLoadCaseAsync(
        Guid modelId,
        int id,
        LoadCaseData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutLoadCaseCommandHandler.ExecuteAsync(
            new PutLoadCaseCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<LoadCombination>> PutLoadCombinationAsync(
        Guid modelId,
        int id,
        LoadCombinationData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutLoadCombinationCommandHandler.ExecuteAsync(
            new PutLoadCombinationCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<MaterialResponse>> PutMaterialAsync(
        int id,
        Guid modelId,
        MaterialRequestData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutMaterialCommandHandler.ExecuteAsync(
            new PutMaterialCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<MomentLoadResponse>> PutMomentLoadAsync(
        int id,
        Guid modelId,
        MomentLoadData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutMomentLoadCommandHandler.ExecuteAsync(
            new PutMomentLoadCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<NodeResponse>> PutNodeAsync(
        int id,
        Guid modelId,
        NodeData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutNodeCommandHandler.ExecuteAsync(
            new PutNodeCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<PointLoadResponse>> PutPointLoadAsync(
        int id,
        Guid modelId,
        PointLoadData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutPointLoadCommandHandler.ExecuteAsync(
            new PutPointLoadCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<SectionProfileResponse>> PutSectionProfileAsync(
        int id,
        Guid modelId,
        SectionProfileData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutSectionProfileCommandHandler.ExecuteAsync(
            new PutSectionProfileCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<SectionProfileFromLibrary>> PutSectionProfileFromLibraryAsync(
        int id,
        Guid modelId,
        SectionProfileFromLibraryData body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryPutSectionProfileFromLibraryCommandHandler.ExecuteAsync(
            new PutSectionProfileFromLibraryCommand
            {
                Id = id,
                ModelId = modelId,
                Body = body,
            },
            cancellationToken
        );

    public async Task<Result<bool>> RejectModelProposalAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryRejectModelProposalCommandHandler.ExecuteAsync(
            new ModelEntityRequest(id, modelId),
            cancellationToken
        );

    public async Task<Result<ModelProposalResponse>> RepairModelAsync(
        Guid modelId,
        string body = null,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryRepairModelCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<AnalyticalResultsResponse>> RunDirectStiffnessMethodAsync(
        Guid modelId,
        RunDsmRequest body = null,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryRunDirectStiffnessMethodCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<AnalyticalResultsResponse>> RunOpenSeesAnalysisAsync(
        Guid modelId,
        RunDsmRequest body = null,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryRunOpenSeesAnalysisCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<ModelProposalResponse>> SpeckleRecieveOperationAsync(
        Guid modelId,
        SpeckleReceiveParameters body = null,
        CancellationToken cancellationToken = default
    ) =>
        await inMemorySpeckleReceiveOperationCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );

    public async Task<Result<NodeResponse>> UpdateNodeAsync(
        Guid modelId,
        UpdateNodeRequest body,
        CancellationToken cancellationToken = default
    ) =>
        await inMemoryUpdateNodeCommandHandler.ExecuteAsync(
            new() { ModelId = modelId, Body = body },
            cancellationToken
        );
}
