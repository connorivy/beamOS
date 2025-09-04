using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ProposalInfo;

public sealed class ServerSideUndoActionClientCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    LoadBeamOsEntityCommandHandler loadBeamOsEntityCommandHandler,
    CacheModelResponseCommandHandler cacheModelResponseCommandHandler,
    IState<EditorComponentState> editorComponentState,
    IDispatcher dispatcher,
    ISnackbar snackbar,
    ILogger<ServerSideUndoActionClientCommandHandler> logger
) : ClientCommandHandlerBase<ServerSideAction, ModelResponse>(logger, snackbar)
{
    protected override async ValueTask<Result<ModelResponse>> UpdateServer(
        ServerSideAction command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClient.ModelRestoreAsync(
            command.ModelId,
            command.ActionTime - TimeSpan.FromSeconds(1),
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        ServerSideAction command,
        Result<ModelResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess && editorComponentState.Value.LoadedModelId == command.ModelId)
        {
            return await loadBeamOsEntityCommandHandler.ExecuteAsync(
                new LoadModelResponseHydratedCommand(
                    serverResponse.Value,
                    editorComponentState.Value.EditorApi
                ),
                CancellationToken.None
            );
        }
        return serverResponse;
    }

    protected override async ValueTask<Result> UpdateClient(
        ServerSideAction command,
        Result<ModelResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            return await cacheModelResponseCommandHandler.ExecuteAsync(
                new CacheModelCommand(editorComponentState.Value.CanvasId, serverResponse.Value),
                CancellationToken.None
            );
        }
        return serverResponse;
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

public sealed class ServerSideActionClientCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    LoadBeamOsEntityCommandHandler loadBeamOsEntityCommandHandler,
    CacheModelResponseCommandHandler cacheModelResponseCommandHandler,
    IState<EditorComponentState> editorComponentState,
    IDispatcher dispatcher,
    ISnackbar snackbar,
    ILogger<ServerSideActionClientCommandHandler> logger
) : ClientCommandHandlerBase<ServerSideAction, ModelResponse>(logger, snackbar)
{
    protected override async ValueTask<Result<ModelResponse>> UpdateServer(
        ServerSideAction command,
        CancellationToken ct = default
    )
    {
        return await structuralAnalysisApiClient.ModelRestoreAsync(
            command.ModelId,
            command.ActionTime,
            ct
        );
    }

    protected override async ValueTask<Result> UpdateEditorAfterServerResponse(
        ServerSideAction command,
        Result<ModelResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess && editorComponentState.Value.LoadedModelId == command.ModelId)
        {
            return await loadBeamOsEntityCommandHandler.ExecuteAsync(
                new LoadModelResponseHydratedCommand(
                    serverResponse.Value,
                    editorComponentState.Value.EditorApi
                ),
                CancellationToken.None
            );
        }
        return serverResponse;
    }

    protected override async ValueTask<Result> UpdateClient(
        ServerSideAction command,
        Result<ModelResponse> serverResponse
    )
    {
        if (serverResponse.IsSuccess)
        {
            return await cacheModelResponseCommandHandler.ExecuteAsync(
                new CacheModelCommand(editorComponentState.Value.CanvasId, serverResponse.Value),
                CancellationToken.None
            );
        }
        return serverResponse;
    }
}
