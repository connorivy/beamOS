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

public sealed class ServerSideUndoActionClientCommandHandler(
    IDispatcher dispatcher,
    ISnackbar snackbar,
    ILogger<ServerSideUndoActionClientCommandHandler> logger
) : ClientCommandHandlerBase<ServerSideAction, ServerSideUndoAction>(logger, snackbar)
{
    protected override ValueTask<Result<ServerSideUndoAction>> UpdateServer(
        ServerSideAction command,
        CancellationToken ct = default
    )
    {
        // No server-side action needed for undo
        return default;
    }

    protected override ValueTask<Result> UpdateEditorAfterServerResponse(
        ServerSideAction command,
        Result<ServerSideUndoAction> serverResponse
    )
    {
        // No editor update needed for undo
        return ValueTask.FromResult(Result.Success);
    }

    protected override ValueTask<Result> UpdateClient(
        ServerSideAction command,
        Result<ServerSideUndoAction> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            dispatcher.Dispatch(serverResponse.Value);
        }
        return ValueTask.FromResult(Result.Success);
    }
}

public readonly record struct ServerSideAction(Guid ModelId, DateTimeOffset ActionTime)
    : IBeamOsUndoableClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new ServerSideUndoAction(this.ModelId, this.ActionTime) with
        {
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}

public readonly record struct ServerSideUndoAction(Guid ModelId, DateTimeOffset ActionTime)
    : IBeamOsUndoableClientCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        new ServerSideAction(this.ModelId, this.ActionTime) with
        {
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByServer = args?.HandledByServer ?? this.HandledByServer,
        };
}
