using System.ComponentModel;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using Microsoft.SemanticKernel;

namespace BeamOs.Ai;

public class AiApiPlugin(IStructuralAnalysisApiClientV1 apiClient)
{
    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Node.BatchResponse>
    > BatchPutElement1dAsync(
        Guid modelId,
        IEnumerable<StructuralAnalysis.Contracts.PhysicalModel.Element1d.PutElement1dRequest> body,
        CancellationToken cancellationToken = default
    ) => apiClient.BatchPutElement1dAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Node.BatchResponse>
    > BatchPutMaterialAsync(
        Guid modelId,
        IEnumerable<StructuralAnalysis.Contracts.PhysicalModel.Material.PutMaterialRequest> body,
        CancellationToken cancellationToken = default
    ) => apiClient.BatchPutMaterialAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Node.BatchResponse>
    > BatchPutMomentLoadAsync(
        Guid modelId,
        IEnumerable<StructuralAnalysis.Contracts.PhysicalModel.MomentLoad.PutMomentLoadRequest> body,
        CancellationToken cancellationToken = default
    ) => apiClient.BatchPutMomentLoadAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Node.BatchResponse>
    > BatchPutNodeAsync(
        Guid modelId,
        IEnumerable<StructuralAnalysis.Contracts.PhysicalModel.Node.PutNodeRequest> body,
        CancellationToken cancellationToken = default
    ) => apiClient.BatchPutNodeAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Node.BatchResponse>
    > BatchPutPointLoadAsync(
        Guid modelId,
        IEnumerable<StructuralAnalysis.Contracts.PhysicalModel.PointLoad.PutPointLoadRequest> body,
        CancellationToken cancellationToken = default
    ) => apiClient.BatchPutPointLoadAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Node.BatchResponse>
    > BatchPutSectionProfileAsync(
        Guid modelId,
        IEnumerable<StructuralAnalysis.Contracts.PhysicalModel.SectionProfile.PutSectionProfileRequest> body,
        CancellationToken cancellationToken = default
    ) => apiClient.BatchPutSectionProfileAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.Common.BeamOsModelBuilderDto>
    > ConvertToBeamOsAsync(
        StructuralAnalysis.Contracts.Common.SpeckleReceiveParameters body = null,
        CancellationToken cancellationToken = default
    ) => apiClient.ConvertToBeamOsAsync(body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Element1d.Element1dResponse>
    > CreateElement1dAsync(
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.Element1d.CreateElement1dRequest body,
        CancellationToken cancellationToken = default
    ) => apiClient.CreateElement1dAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Material.MaterialResponse>
    > CreateMaterialAsync(
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.Material.CreateMaterialRequest body,
        CancellationToken cancellationToken = default
    ) => apiClient.CreateMaterialAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Model.ModelResponse>
    > CreateModelAsync(
        StructuralAnalysis.Contracts.PhysicalModel.Model.CreateModelRequest body = null,
        CancellationToken cancellationToken = default
    ) => apiClient.CreateModelAsync(body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.MomentLoad.MomentLoadResponse>
    > CreateMomentLoadAsync(
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.MomentLoad.CreateMomentLoadRequest body,
        CancellationToken cancellationToken = default
    ) => apiClient.CreateMomentLoadAsync(modelId, body, cancellationToken);

    [KernelFunction]
    [Description("Create a node in the model with the provided modelId.")]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Node.NodeResponse>
    > CreateNodeAsync(
        [Description("Id of the model that the new node with be created in")] Guid modelId,
        [Description("X Coordinate of Node")] double locationPoint_x,
        [Description("Y Coordinate of Node")] double locationPoint_y,
        [Description("Z Coordinate of Node")] double locationPoint_z,
        string locationPoint_lengthUnit,
        bool restraint_canTranslateAlongX,
        bool restraint_canTranslateAlongY,
        bool restraint_canTranslateAlongZ,
        bool restraint_canRotateAboutX,
        bool restraint_canRotateAboutY,
        bool restraint_canRotateAboutZ,
        CancellationToken cancellationToken = default
    ) =>
        apiClient.CreateNodeAsync(
            modelId,
            new CreateNodeRequest(
                new Point(locationPoint_x, locationPoint_y, locationPoint_z, LengthUnit.Meter),
                new Restraint(
                    restraint_canTranslateAlongX,
                    restraint_canTranslateAlongY,
                    restraint_canTranslateAlongZ,
                    restraint_canRotateAboutX,
                    restraint_canRotateAboutY,
                    restraint_canRotateAboutZ
                )
            ),
            cancellationToken
        );

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.PointLoad.PointLoadResponse>
    > CreatePointLoadAsync(
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.PointLoad.CreatePointLoadRequest body,
        CancellationToken cancellationToken = default
    ) => apiClient.CreatePointLoadAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.SectionProfile.SectionProfileResponse>
    > CreateSectionProfileAsync(
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.SectionProfile.CreateSectionProfileRequest body,
        CancellationToken cancellationToken = default
    ) => apiClient.CreateSectionProfileAsync(modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<Result<int>> DeleteAllResultSetsAsync(
        Guid modelId,
        CancellationToken cancellationToken = default
    ) => apiClient.DeleteAllResultSetsAsync(modelId, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.Common.ModelEntityResponse>
    > DeleteElement1dAsync(Guid modelId, int id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteElement1dAsync(modelId, id, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.Common.ModelEntityResponse>
    > DeleteMomentLoadAsync(Guid modelId, int id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteMomentLoadAsync(modelId, id, cancellationToken);

    // [KernelFunction]
    public Task<Result<StructuralAnalysis.Contracts.Common.ModelEntityResponse>> DeleteNodeAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) => apiClient.DeleteNodeAsync(modelId, id, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.Common.ModelEntityResponse>
    > DeletePointLoadAsync(Guid modelId, int id, CancellationToken cancellationToken = default) =>
        apiClient.DeletePointLoadAsync(modelId, id, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.Common.ModelEntityResponse>
    > DeleteSectionProfileAsync(
        Guid modelId,
        int id,
        CancellationToken cancellationToken = default
    ) => apiClient.DeleteSectionProfileAsync(modelId, id, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.AnalyticalResults.Diagrams.AnalyticalResultsResponse>
    > GetDiagramsAsync(
        Guid modelId,
        int id,
        string unitsOverride = null,
        CancellationToken cancellationToken = default
    ) => apiClient.GetDiagramsAsync(modelId, id, unitsOverride, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Element1d.Element1dResponse>
    > GetElement1dAsync(Guid modelId, int id, CancellationToken cancellationToken = default) =>
        apiClient.GetElement1dAsync(modelId, id, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Model.ModelResponse>
    > GetModelAsync(Guid modelId, CancellationToken cancellationToken = default) =>
        apiClient.GetModelAsync(modelId, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<List<StructuralAnalysis.Contracts.PhysicalModel.Model.ModelInfoResponse>>
    > GetModelsAsync(CancellationToken cancellationToken = default) =>
        apiClient.GetModelsAsync(cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.AnalyticalResults.NodeResult.NodeResultResponse>
    > GetNodeResultAsync(
        Guid modelId,
        int resultSetId,
        int id,
        CancellationToken cancellationToken = default
    ) => apiClient.GetNodeResultAsync(modelId, resultSetId, id, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.AnalyticalResults.ResultSetResponse>
    > GetResultSetAsync(Guid modelId, int id, CancellationToken cancellationToken = default) =>
        apiClient.GetResultSetAsync(modelId, id, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Element1d.Element1dResponse>
    > PutElement1dAsync(
        int id,
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.Element1d.Element1dData body,
        CancellationToken cancellationToken = default
    ) => apiClient.PutElement1dAsync(id, modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Material.MaterialResponse>
    > PutMaterialAsync(
        int id,
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.Material.MaterialRequestData body,
        CancellationToken cancellationToken = default
    ) => apiClient.PutMaterialAsync(id, modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.MomentLoad.MomentLoadResponse>
    > PutMomentLoadAsync(
        int id,
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.MomentLoad.MomentLoadData body,
        CancellationToken cancellationToken = default
    ) => apiClient.PutMomentLoadAsync(id, modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<Result<StructuralAnalysis.Contracts.PhysicalModel.Node.NodeResponse>> PutNodeAsync(
        int id,
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.Node.NodeData body,
        CancellationToken cancellationToken = default
    ) => apiClient.PutNodeAsync(id, modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.PointLoad.PointLoadResponse>
    > PutPointLoadAsync(
        int id,
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.PointLoad.PointLoadData body,
        CancellationToken cancellationToken = default
    ) => apiClient.PutPointLoadAsync(id, modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.SectionProfile.SectionProfileResponse>
    > PutSectionProfileAsync(
        int id,
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.SectionProfile.SectionProfileData body,
        CancellationToken cancellationToken = default
    ) => apiClient.PutSectionProfileAsync(id, modelId, body, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.AnalyticalResults.Diagrams.AnalyticalResultsResponse>
    > RunDirectStiffnessMethodAsync(
        Guid modelId,
        string unitsOverride = null,
        CancellationToken cancellationToken = default
    ) => apiClient.RunDirectStiffnessMethodAsync(modelId, unitsOverride, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.AnalyticalResults.Diagrams.AnalyticalResultsResponse>
    > RunOpenSeesAnalysisAsync(
        Guid modelId,
        string unitsOverride = null,
        CancellationToken cancellationToken = default
    ) => apiClient.RunOpenSeesAnalysisAsync(modelId, unitsOverride, cancellationToken);

    // [KernelFunction]
    public Task<
        Result<StructuralAnalysis.Contracts.PhysicalModel.Node.NodeResponse>
    > UpdateNodeAsync(
        Guid modelId,
        StructuralAnalysis.Contracts.PhysicalModel.Node.UpdateNodeRequest body,
        CancellationToken cancellationToken = default
    ) => apiClient.UpdateNodeAsync(modelId, body, cancellationToken);
}
