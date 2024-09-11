using BeamOs.CodeGen.Apis.StructuralAnalysisApi;

namespace BeamOs.ApiClient;

public class HttpApiClient : IStructuralAnalysisApiAlphaClient
{
    private readonly IApiAlphaClient apiAlphaClient;

    public HttpApiClient(IApiAlphaClient apiAlphaClient)
    {
        this.apiAlphaClient = apiAlphaClient;
    }

    public Task<Element1DResponse> CreateElement1dAsync(CreateElement1dRequest body) =>
        this.apiAlphaClient.CreateElement1dAsync(body);

    public Task<Element1DResponse> CreateElement1dAsync(
        CreateElement1dRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.CreateElement1dAsync(body, cancellationToken);

    public Task<MaterialResponse> CreateMaterialAsync(CreateMaterialRequest body) =>
        this.apiAlphaClient.CreateMaterialAsync(body);

    public Task<MaterialResponse> CreateMaterialAsync(
        CreateMaterialRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.CreateMaterialAsync(body, cancellationToken);

    public Task<ModelResponse> CreateModelAsync(CreateModelRequest body) =>
        this.apiAlphaClient.CreateModelAsync(body);

    public Task<ModelResponse> CreateModelAsync(
        CreateModelRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.CreateModelAsync(body, cancellationToken);

    public Task<MomentLoadResponse> CreateMomentLoadAsync(CreateMomentLoadRequest body) =>
        this.apiAlphaClient.CreateMomentLoadAsync(body);

    public Task<MomentLoadResponse> CreateMomentLoadAsync(
        CreateMomentLoadRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.CreateMomentLoadAsync(body, cancellationToken);

    public Task<NodeResponse> CreateNodeAsync(CreateNodeRequest body) =>
        this.apiAlphaClient.CreateNodeAsync(body);

    public Task<NodeResponse> CreateNodeAsync(
        CreateNodeRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.CreateNodeAsync(body, cancellationToken);

    public Task<PointLoadResponse> CreatePointLoadAsync(CreatePointLoadRequest body) =>
        this.apiAlphaClient.CreatePointLoadAsync(body);

    public Task<PointLoadResponse> CreatePointLoadAsync(
        CreatePointLoadRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.CreatePointLoadAsync(body, cancellationToken);

    public Task<SectionProfileResponse> CreateSectionProfileAsync(
        CreateSectionProfileRequest body
    ) => this.apiAlphaClient.CreateSectionProfileAsync(body);

    public Task<SectionProfileResponse> CreateSectionProfileAsync(
        CreateSectionProfileRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.CreateSectionProfileAsync(body, cancellationToken);

    public Task<bool> DeleteModelAsync(ModelIdRequest body) =>
        this.apiAlphaClient.DeleteModelAsync(body.ModelId);

    public Task<bool> DeleteModelAsync(ModelIdRequest body, CancellationToken cancellationToken) =>
        this.apiAlphaClient.DeleteModelAsync(body.ModelId, cancellationToken);

    public Task<ICollection<Element1DResponse>> GetElement1dsAsync(GetElement1dsRequest body) =>
        this.apiAlphaClient.GetElement1dsAsync(body.ModelId, body.Element1dIds);

    public Task<ICollection<Element1DResponse>> GetElement1dsAsync(
        GetElement1dsRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetElement1dsAsync(body.ModelId, body.Element1dIds, cancellationToken);

    public Task<ModelResponse> GetModelAsync(ModelIdRequestWithProperties body) =>
        this.apiAlphaClient.GetModelAsync(body.ModelId, body.Properties);

    public Task<ModelResponse> GetModelAsync(
        ModelIdRequestWithProperties body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetModelAsync(body.ModelId, body.Properties, cancellationToken);

    public Task<ModelResultResponse> GetModelResultsAsync(IdRequest body) =>
        this.apiAlphaClient.GetModelResultsAsync(body.Id);

    public Task<ModelResultResponse> GetModelResultsAsync(
        IdRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetModelResultsAsync(body.Id, cancellationToken);

    public Task<ICollection<ModelResponse>> GetModelsAsync(EmptyRequest body) =>
        this.apiAlphaClient.GetModelsAsync();

    public Task<ICollection<ModelResponse>> GetModelsAsync(
        EmptyRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetModelsAsync(cancellationToken);

    public Task<MomentDiagramResponse> GetMomentDiagramAsync(IdRequest body) =>
        this.apiAlphaClient.GetMomentDiagramAsync(body.Id);

    public Task<MomentDiagramResponse> GetMomentDiagramAsync(
        IdRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetMomentDiagramAsync(body.Id, cancellationToken);

    public Task<ICollection<MomentDiagramResponse>> GetMomentDiagramsAsync(ModelIdRequest body) =>
        this.apiAlphaClient.GetMomentDiagramsAsync(body.ModelId);

    public Task<ICollection<MomentDiagramResponse>> GetMomentDiagramsAsync(
        ModelIdRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetMomentDiagramsAsync(body.ModelId, cancellationToken);

    public Task<ICollection<MomentLoadResponse>> GetMomentLoadsAsync(GetMomentLoadRequest body) =>
        this.apiAlphaClient.GetMomentLoadsAsync(body.ModelId, body.MomentLoadIds);

    public Task<ICollection<MomentLoadResponse>> GetMomentLoadsAsync(
        GetMomentLoadRequest body,
        CancellationToken cancellationToken
    ) =>
        this.apiAlphaClient.GetMomentLoadsAsync(
            body.ModelId,
            body.MomentLoadIds,
            cancellationToken
        );

    public Task<ICollection<NodeResultResponse>> GetNodeResultsAsync(GetNodeResultsRequest body) =>
        this.apiAlphaClient.GetNodeResultsAsync(body.ModelId, body.NodeIds);

    public Task<ICollection<NodeResultResponse>> GetNodeResultsAsync(
        GetNodeResultsRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetNodeResultsAsync(body.ModelId, body.NodeIds, cancellationToken);

    public Task<ShearDiagramResponse> GetShearDiagramAsync(IdRequest body) =>
        this.apiAlphaClient.GetShearDiagramAsync(body.Id);

    public Task<ShearDiagramResponse> GetShearDiagramAsync(
        IdRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetShearDiagramAsync(body.Id, cancellationToken);

    public Task<ICollection<ShearDiagramResponse>> GetShearDiagramsAsync(ModelIdRequest body) =>
        this.apiAlphaClient.GetShearDiagramsAsync(body.ModelId);

    public Task<ICollection<ShearDiagramResponse>> GetShearDiagramsAsync(
        ModelIdRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetShearDiagramsAsync(body.ModelId, cancellationToken);

    public Task<Element1DResponse> GetSingleElement1dAsync(IdRequest body) =>
        this.apiAlphaClient.GetSingleElement1dAsync(body.Id);

    public Task<Element1DResponse> GetSingleElement1dAsync(
        IdRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetSingleElement1dAsync(body.Id, cancellationToken);

    public Task<ICollection<NodeResultResponse>> GetSingleNodeResultAsync(IdRequest body) =>
        this.apiAlphaClient.GetSingleNodeResultAsync(body.Id);

    public Task<ICollection<NodeResultResponse>> GetSingleNodeResultAsync(
        IdRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.GetSingleNodeResultAsync(body.Id, cancellationToken);

    public Task<NodeResponse> PatchNodeAsync(PatchNodeRequest body) =>
        this.apiAlphaClient.PatchNodeAsync(body, body.NodeId);

    public Task<NodeResponse> PatchNodeAsync(
        PatchNodeRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.PatchNodeAsync(body, body.NodeId, cancellationToken);

    public Task<AnalyticalModelResponse3> RunDirectStiffnessMethodAsync(ModelIdRequest body) =>
        this.apiAlphaClient.RunDirectStiffnessMethodAsync(body.ModelId);

    public Task<AnalyticalModelResponse3> RunDirectStiffnessMethodAsync(
        ModelIdRequest body,
        CancellationToken cancellationToken
    ) => this.apiAlphaClient.RunDirectStiffnessMethodAsync(body.ModelId, cancellationToken);
}
