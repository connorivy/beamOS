using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
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
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.CodeGen.StructuralAnalysisApiClient;

public partial class InMemoryApiClient
    : BeamOs.CodeGen.StructuralAnalysisApiClient.IStructuralAnalysisApiClientV1
{
    private partial ModelResourceRequest<SectionProfileFromLibraryData> CreateSectionProfileFromLibraryData_Command(
        Guid modelId,
        SectionProfileFromLibraryData body,
        CancellationToken cancellationToken
    ) => new(modelId, body);

    private partial BatchPutSectionProfileFromLibraryCommand CreateBatchPutSectionProfileFromLibraryCommandCommand(
        Guid modelId,
        IEnumerable<SectionProfileFromLibrary> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial CreateSectionProfileCommand CreateCreateSectionProfileCommandCommand(
        Guid modelId,
        CreateSectionProfileRequest body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial BatchPutSectionProfileCommand CreateBatchPutSectionProfileCommandCommand(
        Guid modelId,
        IEnumerable<PutSectionProfileRequest> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial IModelEntity CreateIModelEntityCommand(
        Guid modelId,
        int id,
        CancellationToken cancellationToken
    ) => new ModelEntityCommand { ModelId = modelId, Id = id };

    private partial PutSectionProfileCommand CreatePutSectionProfileCommandCommand(
        int id,
        Guid modelId,
        SectionProfileData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial PutSectionProfileFromLibraryCommand CreatePutSectionProfileFromLibraryCommandCommand(
        int id,
        Guid modelId,
        SectionProfileFromLibraryData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial CreatePointLoadCommand CreateCreatePointLoadCommandCommand(
        Guid modelId,
        CreatePointLoadRequest body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial BatchPutPointLoadCommand CreateBatchPutPointLoadCommandCommand(
        Guid modelId,
        IEnumerable<PutPointLoadRequest> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial PutPointLoadCommand CreatePutPointLoadCommandCommand(
        int id,
        Guid modelId,
        PointLoadData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial CreateNodeCommand CreateCreateNodeCommandCommand(
        Guid modelId,
        CreateNodeRequest body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial PatchNodeCommand CreatePatchNodeCommandCommand(
        Guid modelId,
        UpdateNodeRequest body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial BatchPutNodeCommand CreateBatchPutNodeCommandCommand(
        Guid modelId,
        IEnumerable<PutNodeRequest> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial ModelResourceRequest<CreateInternalNodeRequest> CreateCreateInternalNodeRequest_Command(
        Guid modelId,
        CreateInternalNodeRequest body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial ModelResourceRequest<InternalNode[]> CreateInternalNodeArray_Command(
        Guid modelId,
        IEnumerable<InternalNode> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial ModelResourceWithIntIdRequest<InternalNodeData> CreateInternalNodeData_Command(
        Guid modelId,
        int id,
        InternalNodeData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial PutNodeCommand CreatePutNodeCommandCommand(
        int id,
        Guid modelId,
        NodeData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial CreateMomentLoadCommand CreateCreateMomentLoadCommandCommand(
        Guid modelId,
        CreateMomentLoadRequest body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial BatchPutMomentLoadCommand CreateBatchPutMomentLoadCommandCommand(
        Guid modelId,
        IEnumerable<PutMomentLoadRequest> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial PutMomentLoadCommand CreatePutMomentLoadCommandCommand(
        int id,
        Guid modelId,
        MomentLoadData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial CreateModelRequest CreateCreateModelRequestCommand(
        CreateModelRequest body,
        CancellationToken cancellationToken
    ) => body;

    private partial EmptyRequest CreateEmptyRequestCommand(CancellationToken cancellationToken) =>
        new();

    private partial ModelResourceRequest<ModelProposalData> CreateModelProposalData_Command(
        Guid modelId,
        ModelProposalData body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial Guid CreateGuidCommand(Guid modelId, CancellationToken cancellationToken) =>
        modelId;

    private partial ModelResourceRequest<ModelInfoData> CreateModelInfoData_Command(
        Guid modelId,
        ModelInfoData body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial ModelResourceRequest<string> CreateModelResourceRequest_string_Command(
        Guid modelId,
        string body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial CreateMaterialCommand CreateCreateMaterialCommandCommand(
        Guid modelId,
        CreateMaterialRequest body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial BatchPutMaterialCommand CreateBatchPutMaterialCommandCommand(
        Guid modelId,
        IEnumerable<PutMaterialRequest> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial PutMaterialCommand CreatePutMaterialCommandCommand(
        int id,
        Guid modelId,
        MaterialData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial BatchPutLoadCombinationCommand CreateBatchPutLoadCombinationCommandCommand(
        Guid modelId,
        IEnumerable<LoadCombination> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial CreateLoadCombinationCommand CreateCreateLoadCombinationCommandCommand(
        Guid modelId,
        LoadCombinationData body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial PutLoadCombinationCommand CreatePutLoadCombinationCommandCommand(
        Guid modelId,
        int id,
        LoadCombinationData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial BatchPutLoadCaseCommand CreateBatchPutLoadCaseCommandCommand(
        Guid modelId,
        IEnumerable<LoadCase> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial CreateLoadCaseCommand CreateCreateLoadCaseCommandCommand(
        Guid modelId,
        LoadCaseData body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial PutLoadCaseCommand CreatePutLoadCaseCommandCommand(
        Guid modelId,
        int id,
        LoadCaseData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial CreateElement1dCommand CreateCreateElement1dCommandCommand(
        Guid modelId,
        CreateElement1dRequest body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };

    private partial BatchPutElement1dCommand CreateBatchPutElement1dCommandCommand(
        Guid modelId,
        IEnumerable<PutElement1dRequest> body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body.ToArray() };

    private partial PutElement1dCommand CreatePutElement1dCommandCommand(
        int id,
        Guid modelId,
        Element1dData body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            Id = id,
            ModelId = modelId,
            Body = body,
        };

    private partial RunDsmCommand CreateRunDsmCommandCommand(
        Guid modelId,
        RunDsmRequest body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            ModelId = modelId,
            LoadCombinationIds = body.LoadCombinationIds,
            UnitsOverride = body.UnitsOverride,
        };

    // private partial ModelId CreateModelIdCommand(
    //     Guid modelId,
    //     CancellationToken cancellationToken
    // ) => new(modelId);

    private partial GetDiagramsCommand CreateGetDiagramsCommandCommand(
        Guid modelId,
        int id,
        string unitsOverride,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            ModelId = modelId,
            Id = id,
            UnitsOverride = unitsOverride,
        };

    private partial GetAnalyticalResultResourceQuery CreateGetAnalyticalResultResourceQueryCommand(
        Guid modelId,
        int resultSetId,
        int id,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            ModelId = modelId,
            ResultSetId = resultSetId,
            Id = id,
        };

    private partial ModelResourceWithIntIdRequest<List<EntityProposal>?> CreateEntityProposal_Nullable_Command(
        Guid modelId,
        int id,
        IEnumerable<EntityProposal>? body,
        CancellationToken cancellationToken
    ) =>
        new()
        {
            ModelId = modelId,
            Id = id,
            Body = body?.ToList(),
        };

    private partial ModelResourceRequest<DateTimeOffset> CreateDateTimeOffset_Command(
        Guid modelId,
        DateTimeOffset body,
        CancellationToken cancellationToken
    ) => new() { ModelId = modelId, Body = body };
}
