#if !CODEGEN
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
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

namespace BeamOs.StructuralAnalysis.Api.Endpoints;

internal sealed class StructuralAnalysisApiClientV2 : IStructuralAnalysisApiClientV2
{
    private readonly StructuralAnalysisApiClientV1 apiClientV1;

    private StructuralAnalysisApiClientV2(StructuralAnalysisApiClientV1 apiClientV1)
    {
        this.apiClientV1 = apiClientV1;
    }

    public StructuralAnalysisApiClientV2(HttpClient httpClient)
        : this(new StructuralAnalysisApiClientV1(httpClient)) { }

    public Task<ApiResponse<ModelResponse>> AcceptModelProposal(
        ModelResourceWithIntIdRequest<List<EntityProposal>?> request,
        CancellationToken ct = default
    ) => apiClientV1.AcceptModelProposalAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<SectionProfileResponse>> AddSectionProfileFromLibrary(
        ModelResourceRequest<SectionProfileFromLibraryData> request,
        CancellationToken ct = default
    ) => apiClientV1.AddSectionProfileFromLibraryAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutElement1d(
        ModelResourceRequest<PutElement1dRequest[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutElement1dAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutInternalNode(
        ModelResourceRequest<InternalNodeContract[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutInternalNodeAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutLoadCase(
        ModelResourceRequest<LoadCaseContract[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutLoadCaseAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutLoadCombination(
        ModelResourceRequest<LoadCombinationContract[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutLoadCombinationAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutMaterial(
        ModelResourceRequest<PutMaterialRequest[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutMaterialAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutMomentLoad(
        ModelResourceRequest<PutMomentLoadRequest[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutMomentLoadAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutNode(
        ModelResourceRequest<PutNodeRequest[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutNodeAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutPointLoad(
        ModelResourceRequest<PutPointLoadRequest[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutPointLoadAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutSectionProfile(
        ModelResourceRequest<PutSectionProfileRequest[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutSectionProfileAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<BatchResponse>> BatchPutSectionProfileFromLibrary(
        ModelResourceRequest<SectionProfileFromLibraryContract[]> request,
        CancellationToken ct = default
    ) => apiClientV1.BatchPutSectionProfileFromLibraryAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<int>> ClearResults(
        ModelResourceRequest request,
        CancellationToken ct = default
    ) => apiClientV1.ClearResultsAsync(request.ModelId, ct);

    public Task<ApiResponse<Element1dResponse>> CreateElement1d(
        ModelResourceRequest<CreateElement1dRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateElement1dAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<InternalNodeContract>> CreateInternalNode(
        ModelResourceRequest<CreateInternalNodeRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateInternalNodeAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<LoadCaseContract>> CreateLoadCase(
        ModelResourceRequest<LoadCaseData> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateLoadCaseAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<LoadCombinationContract>> CreateLoadCombination(
        ModelResourceRequest<LoadCombinationData> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateLoadCombinationAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<MaterialResponse>> CreateMaterial(
        ModelResourceRequest<CreateMaterialRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateMaterialAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<ModelResponse>> CreateModel(
        CreateModelRequest request,
        CancellationToken ct = default
    ) => apiClientV1.CreateModelAsync(request, ct);

    public Task<ApiResponse<ModelProposalResponse>> CreateModelProposal(
        ModelResourceRequest<ModelProposalData> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateModelProposalAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<MomentLoadResponse>> CreateMomentLoad(
        ModelResourceRequest<CreateMomentLoadRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateMomentLoadAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<NodeResponse>> CreateNode(
        ModelResourceRequest<CreateNodeRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateNodeAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<PointLoadResponse>> CreatePointLoad(
        ModelResourceRequest<CreatePointLoadRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.CreatePointLoadAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<SectionProfileResponse>> CreateSectionProfile(
        ModelResourceRequest<CreateSectionProfileRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.CreateSectionProfileAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<ModelEntityResponse>> DeleteElement1d(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.DeleteElement1dAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelEntityResponse>> DeleteLoadCase(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.DeleteLoadCaseAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelEntityResponse>> DeleteLoadCombination(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.DeleteLoadCombinationAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelEntityResponse>> DeleteMomentLoad(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.DeleteMomentLoadAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelEntityResponse>> DeleteNode(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.DeleteNodeAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelEntityResponse>> DeletePointLoad(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.DeletePointLoadAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelEntityResponse>> DeleteSectionProfile(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.DeleteSectionProfileAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<AnalyticalResultsResponse>> GetDiagrams(
        GetDiagramsRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetDiagramsAsync(request.ModelId, request.Id, request.UnitsOverride, ct);

    public Task<ApiResponse<Element1dResponse>> GetElement1d(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetElement1dAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<InternalNodeContract>> GetInternalNode(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetInternalNodeAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<LoadCaseContract>> GetLoadCase(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetLoadCaseAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<LoadCombinationContract>> GetLoadCombination(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetLoadCombinationAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelResponse>> GetModel(
        ModelResourceRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetModelAsync(request.ModelId, ct);

    public Task<ApiResponse<ModelProposalResponse>> GetModelProposal(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetModelProposalAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ICollection<ModelProposalInfo>>> GetModelProposals(
        ModelResourceRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetModelProposalsAsync(request.ModelId, ct);

    public Task<ApiResponse<ICollection<ModelInfoResponse>>> GetModels(
        EmptyRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetModelsAsync(ct);

    public Task<ApiResponse<NodeResultResponse>> GetNodeResult(
        GetAnalyticalResultResourceQuery request,
        CancellationToken ct = default
    ) => apiClientV1.GetNodeResultAsync(request.ModelId, request.LoadCombinationId, request.Id, ct);

    public async Task<ApiResponse<IDictionary<int, NodeResultResponse>>> GetNodeResults(
        GetAnalyticalResultQuery request,
        CancellationToken ct = default
    )
    {
        // throw new NotImplementedException();
        var result = await apiClientV1.GetNodeResultsAsync(
            request.ModelId,
            request.LoadCombinationId,
            ct
        );
        if (result.IsError)
        {
            return result.Error;
        }

        return result.Value.ToDictionary(key => int.Parse(key.Key), kvp => kvp.Value);
    }

    public Task<ApiResponse<ResultSetResponse>> GetResultSet(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.GetResultSetAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelResponse>> ModelRestore(
        ModelResourceRequest<DateTimeOffset> request,
        CancellationToken ct = default
    ) => apiClientV1.ModelRestoreAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<NodeResponse>> PatchNode(
        ModelResourceRequest<UpdateNodeRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.PatchNodeAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<Element1dResponse>> PutElement1d(
        ModelResourceWithIntIdRequest<Element1dData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutElement1dAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<InternalNodeContract>> PutInternalNode(
        ModelResourceWithIntIdRequest<InternalNodeData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutInternalNodeAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<LoadCaseContract>> PutLoadCase(
        ModelResourceWithIntIdRequest<LoadCaseData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutLoadCaseAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<LoadCombinationContract>> PutLoadCombination(
        ModelResourceWithIntIdRequest<LoadCombinationData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutLoadCombinationAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<MaterialResponse>> PutMaterial(
        ModelResourceWithIntIdRequest<MaterialData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutMaterialAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<ModelResponse>> PutModel(
        ModelResourceRequest<ModelInfoData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutModelAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<MomentLoadResponse>> PutMomentLoad(
        ModelResourceWithIntIdRequest<MomentLoadData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutMomentLoadAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<NodeResponse>> PutNode(
        ModelResourceWithIntIdRequest<NodeData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutNodeAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<PointLoadResponse>> PutPointLoad(
        ModelResourceWithIntIdRequest<PointLoadData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutPointLoadAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<SectionProfileResponse>> PutSectionProfile(
        ModelResourceWithIntIdRequest<SectionProfileData> request,
        CancellationToken ct = default
    ) => apiClientV1.PutSectionProfileAsync(request.ModelId, request.Id, request.Body, ct);

    public Task<ApiResponse<SectionProfileFromLibraryContract>> PutSectionProfileFromLibrary(
        ModelResourceWithIntIdRequest<SectionProfileFromLibraryData> request,
        CancellationToken ct = default
    ) =>
        apiClientV1.PutSectionProfileFromLibraryAsync(
            request.ModelId,
            request.Id,
            request.Body,
            ct
        );

    public Task<ApiResponse<bool>> RejectModelProposal(
        ModelResourceWithIntIdRequest request,
        CancellationToken ct = default
    ) => apiClientV1.RejectModelProposalAsync(request.ModelId, request.Id, ct);

    public Task<ApiResponse<ModelProposalResponse>> RepairModel(
        ModelResourceRequest<string> request,
        CancellationToken ct = default
    ) => apiClientV1.RepairModelAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<AnalyticalResultsResponse>> RunDirectStiffnessMethod(
        ModelResourceRequest<RunDsmRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.RunDirectStiffnessMethodAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<AnalyticalResultsResponse>> RunOpenSeesAnalysis(
        ModelResourceRequest<RunDsmRequest> request,
        CancellationToken ct = default
    ) => apiClientV1.RunOpenSeesAnalysisAsync(request.ModelId, request.Body, ct);

    public Task<ApiResponse<bool>> DeleteModel(
        ModelResourceRequest request,
        CancellationToken ct = default
    ) => apiClientV1.DeleteModelAsync(request.ModelId, ct);
}
#endif
