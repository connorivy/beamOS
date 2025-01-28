using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editors.ReadOnlyEditor;
using Fluxor;
using Microsoft.Extensions.Caching.Hybrid;

namespace BeamOs.WebApp.Components.Features.Editor;

public class LoadModelCommandHandler(
    IStructuralAnalysisApiClientV1 structuralAnalysisApiClient,
    IState<AllEditorComponentState> allEditorComponentState,
    HybridCache cache,
    LoadBeamOsEntityCommandHandler loadBeamOsEntityCommandHandler
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

        var modelResponse = await structuralAnalysisApiClient.GetModelAsync(command.ModelId, ct);

        //var modelCacheResponse = await cache.GetOrCreateAsync<Result<CachedModelResponse>>(
        //    command.ModelId.ToString(),
        //    async ct =>
        //    {
        //        var modelResponse = await structuralAnalysisApiClient.GetModelAsync(
        //            command.ModelId,
        //            ct
        //        );
        //        if (modelResponse.IsError)
        //        {
        //            return modelResponse.Error;
        //        }
        //        else
        //        {
        //            return new CachedModelResponse(modelResponse.Value);
        //        }
        //    },
        //    cancellationToken: ct
        //);

        if (modelResponse.IsError)
        {
            return modelResponse.Error;
        }

        await loadBeamOsEntityCommandHandler.ExecuteAsync(
            new LoadModelResponseHydratedCommand(
                modelResponse.Value,
                editorComponentState.EditorApi
            ),
            ct
        );

        var modelCacheResponse = new CachedModelResponse(modelResponse.Value);

        //await editorComponentState.EditorApi.ClearAsync(ct);

        //await editorComponentState.EditorApi.SetSettingsAsync(modelCacheResponse.Settings, ct);

        //await editorComponentState
        //    .EditorApi
        //    .CreateNodesAsync(modelCacheResponse.Nodes.Values.Select(e => e.ToEditorUnits()), ct);

        //await editorComponentState
        //    .EditorApi
        //    .CreatePointLoadsAsync(
        //        modelCacheResponse.PointLoads.Values.Select(e => e.ToEditorUnits()),
        //        ct
        //    );

        //await editorComponentState
        //    .EditorApi
        //    .CreateElement1dsAsync(modelCacheResponse.Element1ds.Values, ct);

        //await editorComponentState.EditorApi.SetModelResultsAsync(momodelCacheResponsedelResponse.Value.AnalyticalResults, ct);

        //editorComponentState.EditorApi

        return modelCacheResponse;
    }
}

public record struct LoadModelCommand(string CanvasId, Guid ModelId);

public record struct LoadEntityCommand(
    IBeamOsEntityResponse EntityResponse,
    IEditorApiAlpha EditorApi
) : ILoadEntityResponseCommand;

public record struct LoadModelResponseCommand(
    ModelResponse EntityResponse,
    IEditorApiAlpha EditorApi
) : ILoadEntityResponseCommand
{
    IBeamOsEntityResponse ILoadEntityResponseCommand.EntityResponse => this.EntityResponse;
}

public record struct LoadModelResponseHydratedCommand(
    ModelResponseHydrated EntityResponse,
    IEditorApiAlpha EditorApi
) : ILoadEntityResponseCommand
{
    IBeamOsEntityResponse ILoadEntityResponseCommand.EntityResponse => this.EntityResponse;
}

public interface ILoadEntityResponseCommand
{
    public IBeamOsEntityResponse EntityResponse { get; }
    public IEditorApiAlpha EditorApi { get; }
}

public class LoadBeamOsEntityCommandHandler
    : CommandHandlerBase<ILoadEntityResponseCommand, CachedModelResponse>
{
    protected override async Task<Result<CachedModelResponse>> ExecuteCommandAsync(
        ILoadEntityResponseCommand command,
        CancellationToken ct = default
    )
    {
        if (command.EntityResponse is ModelResponse modelResponse)
        {
            await command.EditorApi.ClearAsync(ct);

            await command.EditorApi.SetSettingsAsync(modelResponse.Settings, ct);

            await command
                .EditorApi
                .CreateNodesAsync(modelResponse.Nodes.Select(e => e.ToEditorUnits()), ct);

            await command
                .EditorApi
                .CreatePointLoadsAsync(modelResponse.PointLoads.Select(e => e.ToEditorUnits()), ct);

            await command.EditorApi.CreateElement1dsAsync(modelResponse.Element1ds, ct);

            return new CachedModelResponse(modelResponse);
        }
        else if (command.EntityResponse is ModelResponseHydrated modelResponseH)
        {
            await command.EditorApi.ClearAsync(ct);

            await command.EditorApi.SetSettingsAsync(modelResponseH.Settings, ct);

            await command
                .EditorApi
                .CreateNodesAsync(modelResponseH.Nodes.Select(e => e.ToEditorUnits()), ct);

            await command
                .EditorApi
                .CreatePointLoadsAsync(
                    modelResponseH.PointLoads.Select(e => e.ToEditorUnits()),
                    ct
                );

            await command.EditorApi.CreateElement1dsAsync(modelResponseH.Element1ds, ct);

            return new CachedModelResponse(modelResponseH);
        }

        //if (command is LoadModelResponseCommand modelResponseCommand)
        //{
        //    var modelResponse = modelResponseCommand.EntityResponse;

        //    await command.EditorApi.ClearAsync(ct);

        //    await command.EditorApi.SetSettingsAsync(modelResponse.Settings, ct);

        //    await command
        //        .EditorApi
        //        .CreateNodesAsync(modelResponse.Nodes.Select(e => e.ToEditorUnits()), ct);

        //    await command
        //        .EditorApi
        //        .CreatePointLoadsAsync(modelResponse.PointLoads.Select(e => e.ToEditorUnits()), ct);

        //    await command.EditorApi.CreateElement1dsAsync(modelResponse.Element1ds, ct);

        //    return Result.Success;
        //}
        //else if (command is LoadModelResponseHydratedCommand modelResponseHydratedCommand)
        //{
        //    var modelResponse = modelResponseHydratedCommand.EntityResponse;

        //    await command.EditorApi.ClearAsync(ct);

        //    await command.EditorApi.SetSettingsAsync(modelResponse.Settings, ct);

        //    await command
        //        .EditorApi
        //        .CreateNodesAsync(modelResponse.Nodes.Select(e => e.ToEditorUnits()), ct);

        //    await command
        //        .EditorApi
        //        .CreatePointLoadsAsync(modelResponse.PointLoads.Select(e => e.ToEditorUnits()), ct);

        //    await command.EditorApi.CreateElement1dsAsync(modelResponse.Element1ds, ct);

        //    return Result.Success;
        //}

        throw new NotImplementedException(
            $"Type {command.EntityResponse.GetType()} is not supported in '{nameof(LoadBeamOsEntityCommandHandler)}'"
        );
    }
}
