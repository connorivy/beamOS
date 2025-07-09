using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;

namespace BeamOs.WebApp.Components.Features.Editor.ModelProposals;

public class ModelProposalViewer(
    IStructuralAnalysisApiClientV1 apiClient,
    IEditorApiAlpha editorApi
)
{
    // public async Task<IList<int>> GetModelProposalIds(Guid modelId, CancellationToken ct)
    // {
    //     var modelProposals = await apiClient.GetModelProposals(modelId, ct);
    //     if (modelProposals.IsError)
    //     {
    //         throw new Exception(modelProposals.Error.Description);
    //     }
    //
    //     return modelProposals.Value.Select(mp => mp.Id).ToList();
    // }
    //
    // public async Task<ModelProposal> GetModelProposal(Guid modelId, CancellationToken ct)
    // {
    //     var modelProposal = await apiClient.GetModelProposal(modelId, ct);
    //     if (modelProposal.IsError)
    //     {
    //         throw new Exception(modelProposal.Error.Description);
    //     }
    //
    //     return modelProposal.Value;
    // }
    //
    public async Task DisplayModelProposal(Guid modelId, int proposalId, CancellationToken ct)
    {
        var modelProposal = await apiClient.GetModelProposalAsync(modelId, proposalId, ct);
        if (modelProposal.IsError)
        {
            throw new Exception(modelProposal.Error.Description);
        }

        await editorApi.DisplayModelProposalAsync(modelProposal.Value, ct);
    }
}
