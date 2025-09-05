using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ProposalInfo;

public sealed class AcceptModelProposalClientCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    IDispatcher dispatcher,
    ISnackbar snackbar,
    IState<EditorComponentState> editorState,
    ILogger<AcceptModelProposalClientCommandHandler> logger
) : ClientCommandHandlerBase<AcceptModelProposalClientCommand, ModelResponse>(logger, snackbar)
{
    protected override async ValueTask<Result<ModelResponse>> UpdateServer(
        AcceptModelProposalClientCommand command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClient.AcceptModelProposalAsync(
            command.ModelId,
            command.ProposalId,
            command.EntitiesToIgnore,
            ct
        );
    }

    protected override ValueTask<Result> UpdateEditorAfterServerResponse(
        AcceptModelProposalClientCommand command,
        Result<ModelResponse> serverResponse
    )
    {
        // todo: add elements without replacing existing
        return ValueTask.FromResult(Result.Success);
    }

    protected override ValueTask<Result> UpdateClient(
        AcceptModelProposalClientCommand command,
        Result<ModelResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess && serverResponse.Value is not null)
        {
            var lastModifiedTime = serverResponse.Value.LastModified;
            dispatcher.Dispatch(new ServerSideUndoAction(command.ModelId, lastModifiedTime));
        }
        return ValueTask.FromResult(Result.Success);
    }
}

public readonly record struct AcceptModelProposalClientCommand(
    Guid ModelId,
    int ProposalId,
    List<EntityProposal> EntitiesToIgnore
) : IBeamOsClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
}
