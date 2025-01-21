using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editors.ReadOnlyEditor;
using Fluxor;
using Microsoft.Extensions.Caching.Hybrid;

namespace BeamOs.WebApp.Components.Features.Editor;

public class LoadModelCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    IState<AllEditorComponentState> allEditorComponentState,
    HybridCache cache
) : CommandHandlerBase<LoadModelCommand, CachedModelResponse>
{
    protected override async Task<Result<CachedModelResponse>> ExecuteCommandAsync(
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

        var modelCacheResponse = await cache.GetOrCreateAsync<Result<CachedModelResponse>>(
            command.ModelId.ToString(),
            async ct =>
            {
                var modelResponse = await structuralAnalysisApiClient.GetModelAsync(
                    command.ModelId,
                    ct
                );
                if (modelResponse.IsError)
                {
                    return modelResponse.Error;
                }
                else
                {
                    return new CachedModelResponse(modelResponse.Value);
                }
            },
            cancellationToken: ct
        );

        if (modelCacheResponse.IsError)
        {
            return modelCacheResponse.Error;
        }

        await editorComponentState.EditorApi.ClearAsync(ct);

        await editorComponentState
            .EditorApi
            .SetSettingsAsync(modelCacheResponse.Value.Settings, ct);

        await editorComponentState
            .EditorApi
            .CreateNodesAsync(
                modelCacheResponse.Value.Nodes.Values.Select(e => e.ToEditorUnits()),
                ct
            );

        await editorComponentState
            .EditorApi
            .CreatePointLoadsAsync(
                modelCacheResponse.Value.PointLoads.Values.Select(e => e.ToEditorUnits()),
                ct
            );

        await editorComponentState
            .EditorApi
            .CreateElement1dsAsync(modelCacheResponse.Value.Element1ds.Values, ct);

        //await editorComponentState.EditorApi.SetModelResultsAsync(momodelCacheResponsedelResponse.Value.AnalyticalResults, ct);

        //editorComponentState.EditorApi

        return modelCacheResponse.Value;
    }
}

public record struct LoadModelCommand(string CanvasId, Guid ModelId);
