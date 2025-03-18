using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.EditorCommands.Interfaces;
using Fluxor;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelObjectEditor;

public sealed class PutNodeCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClientV1,
    IState<EditorComponentState> editorState,
    IDispatcher dispatcher,
    ISnackbar snackbar
) : CommandHandlerBase<PutNodeCommand, NodeResponse>(snackbar)
{
    protected override async Task<Result<NodeResponse>> ExecuteCommandAsync(PutNodeCommand command, CancellationToken ct = default)
    {
        dispatcher.Dispatch(new NodeObjectEditorIsLoading(true));
        var request = command.ToRequest();
        return await structuralAnalysisApiClientV1.PutNodeAsync(command.Id, command.ModelId, request, ct);
    }

    protected override void PostProcess(PutNodeCommand command, Result<NodeResponse> response)
    {
        dispatcher.Dispatch(new NodeObjectEditorIsLoading(false));
        if (response.IsSuccess)
        {
            var cachedModel = editorState.Value.CachedModelResponse;
            var currentNodeValue = cachedModel.Nodes.GetValueOrDefault(command.Id);

            dispatcher.Dispatch(new PutObjectCommand<NodeResponse>(currentNodeValue, command.ToResponse()));
        }
    }
}

public record PutObjectCommand<TObject>(TObject? Previous, TObject? New) : IBeamOsClientCommand where TObject : IBeamOsEntityResponse
{
    public Guid Id { get; } = Guid.NewGuid();
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) => this with
    {
        New = this.Previous,
        Previous = this.New,
        HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
        HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
        HandledByServer = args?.HandledByServer ?? this.HandledByServer
    };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) => this with
    {
        HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
        HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
        HandledByServer = args?.HandledByServer ?? this.HandledByServer
    };
}

public record struct PutModelEntityCommand(BeamOsObjectType BeamOsObjectType, int Id, Guid ModelId);
