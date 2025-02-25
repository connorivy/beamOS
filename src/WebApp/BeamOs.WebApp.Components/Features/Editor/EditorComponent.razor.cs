using System.Collections.Immutable;
using System.Reflection.Metadata;
using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.StructuralApi;
using BeamOs.WebApp.Components.Features.UndoRedo;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Editor;

public partial class EditorComponent(
    IStructuralAnalysisApiClientV1 apiClient,
    IEditorApiProxyFactory editorApiProxyFactory,
    IState<EditorComponentState> state,
    IState<CachedModelState> cachedModelState,
    IDispatcher dispatcher,
    LoadModelCommandHandler loadModelCommandHandler,
    LoadBeamOsEntityCommandHandler loadBeamOsEntityCommandHandler,
    ILogger<EditorComponent> logger,
    UndoRedoFunctionality undoRedoFunctionality,
    IJSRuntime js,
    ISnackbar snackbar
) : FluxorComponent
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string CanvasId { get; init; } = CreateCanvasId();

    [Parameter]
    public Guid? ModelId { get; set; }

    [Parameter]
    public bool IsReadOnly { get; set; } = true;

    public static string CreateCanvasId() => "id" + Guid.NewGuid().ToString("N");

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.SubscribeToAction<MoveNodeCommand>(async command =>
        {
            if (command.CanvasId != this.CanvasId)
            {
                return;
            }

            if (!command.HandledByEditor)
            {
                await state.Value.EditorApi.ReduceMoveNodeCommandAsync(command);
            }

            var nodeResponse = await apiClient.UpdateNodeAsync(
                this.ModelId.Value,
                new UpdateNodeRequest(
                    command.NodeId,
                    new()
                    {
                        LengthUnit = LengthUnitContract.Meter,
                        X = command.NewLocation.X,
                        Y = command.NewLocation.Y,
                        Z = command.NewLocation.Z
                    }
                )
            );

            if (nodeResponse.IsSuccess)
            {
                //state.Value.CachedModelResponse.Nodes[command.NodeId] = nodeResponse.Value;
                dispatcher.Dispatch(new ModelEntityUpdated() { ModelEntity = nodeResponse.Value });
            }
        });

        this.SubscribeToAction<ModelEntityCreated>(async command =>
        {
            var stateSnapshot = state.Value;
            if (
                stateSnapshot.EditorApi is null
                || command.ModelEntity.ModelId != stateSnapshot.LoadedModelId
            )
            {
                return;
            }

            if (!command.HandledByEditor)
            {
                if (command.ModelEntity is NodeResponse nodeResponse)
                {
                    await stateSnapshot.EditorApi.CreateNodeAsync(nodeResponse);
                }
                else if (command.ModelEntity is Element1dResponse element1dResponse)
                {
                    await stateSnapshot.EditorApi.CreateElement1dAsync(element1dResponse);
                }
            }
        });

        this.SubscribeToAction<ModelEntityDeleted>(async command =>
        {
            var stateSnapshot = state.Value;
            if (
                stateSnapshot.EditorApi is null
                || command.ModelEntity.ModelId != stateSnapshot.LoadedModelId
            )
            {
                return;
            }

            if (!command.HandledByEditor)
            {
                if (command.EntityType == nameof(Element1d))
                {
                    await stateSnapshot.EditorApi.DeleteElement1dAsync(command.ModelEntity);
                }
                else if (command.EntityType == nameof(Node))
                {
                    await stateSnapshot.EditorApi.DeleteNodeAsync(command.ModelEntity);
                }
            }
        });

        this.SubscribeToAction<AnalyticalResultsCreated>(async command =>
        {
            var stateSnapshot = state.Value;
            if (
                stateSnapshot.EditorApi is null
                || command.AnalyticalResults.ModelId != stateSnapshot.LoadedModelId
            )
            {
                return;
            }

            await stateSnapshot
                .EditorApi
                .SetGlobalStressesAsync(command.AnalyticalResults.GlobalStresses);
        });

        dispatcher.Dispatch(new EditorCreated(this.CanvasId));
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await js.InvokeVoidAsync("initializeCanvasById", this.CanvasId);
            var editorApi = await editorApiProxyFactory.Create(this.CanvasId, this.IsReadOnly);
            dispatcher.Dispatch(new EditorApiCreated(this.CanvasId, editorApi));

            if (this.ModelId is not null)
            {
                await this.LoadModelFromServer(this.ModelId.Value);
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task LoadModelFromServer(Guid modelId)
    {
        dispatcher.Dispatch(new EditorLoadingBegin(this.CanvasId, "Fetching Data"));
        var result = await loadModelCommandHandler.ExecuteAsync(
            new LoadModelCommand(this.CanvasId, modelId)
        );

        if (result.IsError)
        {
            logger.LogError(result.Error.ToString());
        }
        else
        {
            dispatcher.Dispatch(new EditorLoadingEnd(this.CanvasId, result.Value));
        }
    }

    public async Task LoadBeamOsEntity(IBeamOsEntityResponse entity, bool clearExisting = false)
    {
        if (clearExisting)
        {
            await state.Value.EditorApi.ClearAsync();
        }

        dispatcher.Dispatch(new EditorLoadingBegin(this.CanvasId, "Loading"));
        var result = await loadBeamOsEntityCommandHandler.ExecuteAsync(
            new LoadEntityCommand(entity, state.Value.EditorApi)
        );

        if (result.IsError)
        {
            logger.LogError(result.Error.ToString());
        }
        else
        {
            dispatcher.Dispatch(new EditorLoadingEnd(this.CanvasId, result.Value));
        }
    }

    public async Task Test()
    {
        throw new Exception("hahaha");
    }

    public async Task ShowShear()
    {
        cachedModelState.Value.Models.TryGetValue(this.ModelId.Value, out var model);
        if (model.ShearDiagrams is null)
        {
            snackbar.Add("Analysis must be run before showing diagrams", Severity.Info);
            return;
        }
        await state.Value.EditorApi.ClearCurrentOverlayAsync();
        await state.Value.EditorApi.CreateShearDiagramsAsync(model.ShearDiagrams.Values);
    }

    public async Task ShowMoment()
    {
        cachedModelState.Value.Models.TryGetValue(this.ModelId.Value, out var model);
        if (model.MomentDiagrams is null)
        {
            snackbar.Add("Analysis must be run before showing diagrams", Severity.Info);
            return;
        }
        await state.Value.EditorApi.ClearCurrentOverlayAsync();
        await state.Value.EditorApi.CreateMomentDiagramsAsync(model.MomentDiagrams.Values);
    }

    public async Task ShowDeflection()
    {
        cachedModelState.Value.Models.TryGetValue(this.ModelId.Value, out var model);
        if (model.DeflectionDiagrams is null)
        {
            snackbar.Add("Analysis must be run before showing diagrams", Severity.Info);
            return;
        }
        await state.Value.EditorApi.ClearCurrentOverlayAsync();
        await state.Value.EditorApi.CreateDeflectionDiagramsAsync(model.DeflectionDiagrams.Values);
    }

    public async Task Clear() => await state.Value.EditorApi.ClearAsync();

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        await base.DisposeAsyncCore(disposing);
        undoRedoFunctionality.Dispose();

        if (state.Value.EditorApi is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync();
        }
        dispatcher.Dispatch(new EditorDisposed(this.CanvasId));
    }
}

public record struct EditorApiCreated(string CanvasId, IEditorApiAlpha EditorApiAlpha);

public record struct EditorCreated(string CanvasId);

public record struct EditorDisposed(string CanvasId);

public record struct EditorLoadingBegin(string CanvasId, string LoadingText);

public record struct EditorLoadingEnd(string CanvasId, CachedModelResponse CachedModelResponse);

//public record struct SelectionChanged(string CanvasId, IModelEntity[] SelectedObjects);

public record struct ModelCacheKey(Guid ModelId, string TypeName, int Id);

[FeatureState]
public record CachedModelState(ImmutableDictionary<Guid, CachedModelResponse> Models)
{
    public CachedModelState()
        : this(ImmutableDictionary<Guid, CachedModelResponse>.Empty) { }

    public IModelEntity? GetEntityFromCacheOrDefault(ModelCacheKey modelCacheKey)
    {
        if (!this.Models.TryGetValue(modelCacheKey.ModelId, out var model))
        {
            return null;
        }

        return modelCacheKey.TypeName switch
        {
            "Node" => model.Nodes.GetValueOrDefault(modelCacheKey.Id),
            "Element1d" => model.Element1ds.GetValueOrDefault(modelCacheKey.Id),
            "PointLoad" => model.PointLoads.GetValueOrDefault(modelCacheKey.Id),
            _ => null
        };
    }

    public IHasModelId? GetEntityResultsFromCacheOrDefault(
        ModelCacheKey modelCacheKey,
        int resultSetId
    )
    {
        if (!this.Models.TryGetValue(modelCacheKey.ModelId, out var model))
        {
            return null;
        }

        return modelCacheKey.TypeName switch
        {
            "Node"
                => model
                    .NodeResults
                    .GetValueOrDefault(resultSetId)
                    ?.GetValueOrDefault(modelCacheKey.Id),
            "Element1d" => null,
            "PointLoad" => null,
            _ => null
        };
    }
}
