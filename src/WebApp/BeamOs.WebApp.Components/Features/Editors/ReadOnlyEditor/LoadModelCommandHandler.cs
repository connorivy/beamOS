using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.WebApp.Components.Features.Common;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.Editors.ReadOnlyEditor;

public class LoadModelCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    IState<AllEditorComponentState> allEditorComponentState
) : CommandHandlerBase<LoadModelCommand>
{
    protected override async Task<Result> ExecuteCommandAsync(
        LoadModelCommand command,
        CancellationToken ct = default
    )
    {
        if (
            !allEditorComponentState
                .Value
                .EditorState
                .TryGetValue(command.CanvasId, out var editorComponentState)
        )
        {
            return BeamOsError.NotFound(
                description: $"could not find canvas with Id = {command.CanvasId}"
            );
        }

        if (editorComponentState.EditorApi is null)
        {
            return BeamOsError.Failure(
                description: "Failed to load model because canvas editor api is null"
            );
        }

        var modelResponse = await structuralAnalysisApiClient.GetModelAsync(command.ModelId, ct);

        if (modelResponse.IsError)
        {
            return modelResponse.Error;
        }

        await editorComponentState.EditorApi.ClearAsync(ct);

        await editorComponentState.EditorApi.SetSettingsAsync(modelResponse.Value.Settings, ct);

        await editorComponentState
            .EditorApi
            .CreateNodesAsync(modelResponse.Value.Nodes.Select(e => e.ToEditorUnits()), ct);

        await editorComponentState
            .EditorApi
            .CreatePointLoadsAsync(
                modelResponse.Value.PointLoads.Select(e => e.ToEditorUnits()),
                ct
            );

        await editorComponentState
            .EditorApi
            .CreateElement1dsAsync(modelResponse.Value.Element1ds, ct);

        //await editorComponentState.EditorApi.SetModelResultsAsync(modelResponse.Value.AnalyticalResults, ct);

        //editorComponentState.EditorApi

        return Result.Success;
    }
}

public record struct LoadModelCommand(string CanvasId, Guid ModelId);
