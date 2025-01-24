using System.Collections.Immutable;
using System.Reflection.Metadata;
using BeamOs.CodeGen.EditorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
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

namespace BeamOs.WebApp.Components.Features.Editor;

public partial class EditorComponent(
    IStructuralAnalysisApiClientV1 apiClient,
    IEditorApiProxyFactory editorApiProxyFactory,
    IStateSelection<AllEditorComponentState, EditorComponentState> state,
    IState<CachedModelState> modelState,
    IDispatcher dispatcher,
    LoadModelCommandHandler loadModelCommandHandler,
    ILogger<EditorComponent> logger,
    UndoRedoFunctionality undoRedoFunctionality
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

    public IStateSelection<AllEditorComponentState, EditorComponentState> State => state;

    public static string CreateCanvasId() => "id" + Guid.NewGuid().ToString("N");

    protected override void OnInitialized()
    {
        base.OnInitialized();
        dispatcher.Dispatch(new EditorCreated(this.CanvasId));
        state.Select(s => s.EditorState[this.CanvasId]);

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
            var state = this.State.Value;
            if (state.EditorApi is null || command.ModelEntity.ModelId != state.LoadedModelId)
            {
                return;
            }

            if (!command.HandledByEditor)
            {
                if (command.ModelEntity is NodeResponse nodeResponse)
                {
                    await state.EditorApi.CreateNodeAsync(nodeResponse);
                }
                else if (command.ModelEntity is Element1dResponse element1dResponse)
                {
                    await state.EditorApi.CreateElement1dAsync(element1dResponse);
                }
            }
        });

        this.SubscribeToAction<ModelEntityDeleted>(async command =>
        {
            var state = this.State.Value;
            if (state.EditorApi is null || command.ModelEntity.ModelId != state.LoadedModelId)
            {
                return;
            }

            //if (state.SelectedObjects.Length > 0)
            //{
            //    bool selectedObjectDeleted = false;
            //    List<SelectedObject> newSelectedObjects = new(state.SelectedObjects.Length);
            //    foreach (var selected in state.SelectedObjects)
            //    {
            //        if (
            //            selected.TypeName == command.EntityType
            //            && selected.Id == command.ModelEntity.Id
            //        )
            //        {
            //            selectedObjectDeleted = true;
            //        }
            //        else
            //        {
            //            newSelectedObjects.Add(selected);
            //        }
            //    }

            //    if (selectedObjectDeleted)
            //    {
            //        dispatcher.Dispatch(
            //            new ChangeSelectionCommand(this.CanvasId, newSelectedObjects.ToArray())
            //        );
            //    }
            //}

            if (!command.HandledByEditor)
            {
                if (command.EntityType == nameof(Element1d))
                {
                    await state.EditorApi.DeleteElement1dAsync(command.ModelEntity);
                }
                else if (command.EntityType == nameof(Node))
                {
                    await state.EditorApi.DeleteNodeAsync(command.ModelEntity);
                }
            }
        });

        //this.SubscribeToAction<ChangeSelectionCommand>(async command =>
        //{
        //    var state = this.State.Value;
        //    if (state.LoadedModelId is null || this.CanvasId != command.CanvasId)
        //    {
        //        return;
        //    }

        //    var cachedModelResponse = modelState.Value.Models[state.LoadedModelId.Value];

        //    IModelEntity[] modelEntities = new IModelEntity[command.SelectedObjects.Length];
        //    for (int i = 0; i < command.SelectedObjects.Length; i++)
        //    {
        //        var obj = command.SelectedObjects[i];
        //        modelEntities[i] = obj.TypeName switch
        //        {
        //            "Node" => cachedModelResponse.Nodes[obj.Id],
        //            "PointLoad" => cachedModelResponse.PointLoads[obj.Id],
        //            "Element1d" => cachedModelResponse.Element1ds[obj.Id],
        //            _
        //                => throw new NotImplementedException(
        //                    $"type name, {obj.TypeName}, is not implemented"
        //                )
        //        };
        //    }

        //    dispatcher.Dispatch(new SelectionChanged(this.CanvasId, modelEntities));
        //});
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var editorApi = await editorApiProxyFactory.Create(this.CanvasId, this.IsReadOnly);
            dispatcher.Dispatch(new EditorApiCreated(this.CanvasId, editorApi));

            if (this.ModelId is not null)
            {
                dispatcher.Dispatch(new EditorLoadingBegin(this.CanvasId, "Fetching Data"));
                var result = await loadModelCommandHandler.ExecuteAsync(
                    new LoadModelCommand(this.CanvasId, this.ModelId.Value)
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
            //await this.ChangeComponentState(state => state with { LoadingText = "Fetching Data" });
            //await this.LoadModel(physicalModelId);

            //await this.ChangeComponentState(state => state with { IsLoading = false });
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    //protected async Task<Result> ChangeComponentState(
    //    Func<EditorComponentState, EditorComponentState> stateMutation
    //)
    //{
    //    return await this.ChangeComponentStateCommandHandler.ExecuteAsync(
    //        new(this.CanvasId, stateMutation)
    //    );
    //}

    //protected override async ValueTask DisposeAsyncCore(bool disposing)
    //{
    //    EventEmitter.VisibleStateChanged -= this.EventEmitter_VisibleStateChanged;
    //    if (this.EditorComponentState.EditorApi is IAsyncDisposable asyncDisposable)
    //    {
    //        await asyncDisposable.DisposeAsync();
    //    }

    //    base.DisposeAsyncCore(disposing);
    //}

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

//public record struct UpdateNodeCacheItem(NodeResponse NodeResponse);

[FeatureState]
public record EditorComponentState(
    string? LoadingText,
    bool IsLoading,
    Guid? LoadedModelId,
    IEditorApiAlpha? EditorApi,
    //CachedModelResponse? CachedModelResponse,
    SelectedObject[] SelectedObjects
//IModelEntity[] SelectedObjects
)
{
    public EditorComponentState()
        : this(null, false, null, null, []) { }
}

//public static class EditorComponentStateReducers
//{
//    [ReducerMethod]
//    public static EditorComponentState EditorCreatedReducer(
//        EditorComponentState state,
//        EditorApiCreated action
//    )
//    {
//        return state with { EditorApi = action.EditorApiAlpha };
//    }
//}

public record struct SelectionChanged(string CanvasId, IModelEntity[] SelectedObjects);

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
            _
                => throw new NotImplementedException(
                    $"type {modelCacheKey.TypeName} is not implemented"
                )
        };
    }
}

[FeatureState]
public record AllEditorComponentState(ImmutableDictionary<string, EditorComponentState> EditorState)
{
    public AllEditorComponentState()
        : this(ImmutableDictionary<string, EditorComponentState>.Empty) { }
}

public static class AllEditorComponentStateReducers
{
    [ReducerMethod]
    public static AllEditorComponentState EditorCreatedReducer(
        AllEditorComponentState state,
        EditorCreated action
    )
    {
        return state with { EditorState = state.EditorState.Add(action.CanvasId, new()) };
    }

    [ReducerMethod]
    public static AllEditorComponentState EditorDisposedReducer(
        AllEditorComponentState state,
        EditorDisposed action
    )
    {
        return state with { EditorState = state.EditorState.Remove(action.CanvasId) };
    }

    [ReducerMethod]
    public static AllEditorComponentState EditorCreatedReducer(
        AllEditorComponentState state,
        EditorApiCreated action
    )
    {
        var currentEditorState = state.EditorState[action.CanvasId];
        return state with
        {
            EditorState = state
                .EditorState
                .Remove(action.CanvasId)
                .Add(action.CanvasId, currentEditorState with { EditorApi = action.EditorApiAlpha })
        };
    }

    [ReducerMethod]
    public static AllEditorComponentState EditorLoadingBeginReducer(
        AllEditorComponentState state,
        EditorLoadingBegin action
    )
    {
        var currentEditorState = state.EditorState[action.CanvasId];
        return state with
        {
            EditorState = state
                .EditorState
                .Remove(action.CanvasId)
                .Add(
                    action.CanvasId,
                    currentEditorState with
                    {
                        IsLoading = true,
                        LoadingText = action.LoadingText
                    }
                )
        };
    }

    [ReducerMethod]
    public static AllEditorComponentState EditorLoadingBeginReducer(
        AllEditorComponentState state,
        EditorLoadingEnd action
    )
    {
        var currentEditorState = state.EditorState[action.CanvasId];
        return state with
        {
            EditorState = state
                .EditorState
                .Remove(action.CanvasId)
                .Add(
                    action.CanvasId,
                    currentEditorState with
                    {
                        IsLoading = false,
                        //CachedModelResponse = action.CachedModelResponse,
                        LoadedModelId = action.CachedModelResponse.Id
                    }
                )
        };
    }

    //[ReducerMethod]
    //public static AllEditorComponentState ChangeSelectionCommandReducer(
    //    AllEditorComponentState state,
    //    SelectionChanged action
    //)
    //{
    //    var currentEditorState = state.EditorState[action.CanvasId];
    //    return state with
    //    {
    //        EditorState = state
    //            .EditorState
    //            .Remove(action.CanvasId)
    //            .Add(
    //                action.CanvasId,
    //                currentEditorState with
    //                {
    //                    IsLoading = false,
    //                    SelectedObjects = action.SelectedObjects
    //                }
    //            )
    //    };
    //}

    [ReducerMethod]
    public static AllEditorComponentState ChangeSelectionCommandReducer(
        AllEditorComponentState state,
        ChangeSelectionCommand action
    )
    {
        var currentEditorState = state.EditorState[action.CanvasId];
        return state with
        {
            EditorState = state
                .EditorState
                .Remove(action.CanvasId)
                .Add(
                    action.CanvasId,
                    currentEditorState with
                    {
                        SelectedObjects = action.SelectedObjects
                    }
                )
        };
    }

    //[ReducerMethod]
    //public static AllEditorComponentState ChangeSelectionCommandReducer(
    //    AllEditorComponentState state,
    //    UpdateNodeCacheItem action
    //)
    //{
    //    var currentEditorState = state.EditorState[action.CanvasId];
    //    var newNodes = new Dictionary<int, NodeResponse>(
    //        currentEditorState.CachedModelResponse.Nodes
    //    );
    //    newNodes[action.NodeResponse.Id] = action.NodeResponse;

    //    //currentEditorState.CachedModelResponse.Nodes[action.NodeResponse.Id] = action.NodeResponse;

    //    return state with
    //    {
    //        EditorState = state
    //            .EditorState
    //            .Remove(action.CanvasId)
    //            .Add(
    //                action.CanvasId,
    //                currentEditorState with
    //                {
    //                    CachedModelResponse = currentEditorState.CachedModelResponse with
    //                    {
    //                        Nodes = newNodes
    //                    }
    //                }
    //            )
    //    };
    //}

    //[ReducerMethod]
    //public static CachedModelState ChangeSelectionCommandReducer(
    //    CachedModelState state,
    //    UpdateNodeCacheItem action
    //)
    //{
    //    var model = state.Models[action.NodeResponse.ModelId];
    //    var newNodes = new Dictionary<int, NodeResponse>(model.Nodes)
    //    {
    //        [action.NodeResponse.Id] = action.NodeResponse
    //    };

    //    return state with
    //    {
    //        Models = state
    //            .Models
    //            .Remove(action.NodeResponse.ModelId)
    //            .Add(action.NodeResponse.ModelId, model with { Nodes = newNodes })
    //    };
    //}

    [ReducerMethod]
    public static CachedModelState EditorLoadingBeginReducer(
        CachedModelState state,
        EditorLoadingEnd action
    )
    {
        return state with
        {
            Models = state
                .Models
                .Remove(action.CachedModelResponse.Id)
                .Add(action.CachedModelResponse.Id, action.CachedModelResponse)
        };
    }

    [ReducerMethod]
    public static CachedModelState Reducer(CachedModelState state, ModelEntityUpdated action)
    {
        if (!state.Models.TryGetValue(action.ModelEntity.ModelId, out var model))
        {
            return state;
        }

        ImmutableDictionary<Guid, CachedModelResponse> newState;
        if (action.ModelEntity is NodeResponse nodeResponse)
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        Nodes = model
                            .Nodes
                            .Remove(nodeResponse.Id)
                            .Add(nodeResponse.Id, nodeResponse)
                    }
                );
        }
        else if (action.ModelEntity is Element1dResponse element1d)
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        Element1ds = model
                            .Element1ds
                            .Remove(element1d.Id)
                            .Add(element1d.Id, element1d)
                    }
                );
        }
        else if (action.ModelEntity is PointLoadResponse pointLoad)
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        PointLoads = model
                            .PointLoads
                            .Remove(pointLoad.Id)
                            .Add(pointLoad.Id, pointLoad)
                    }
                );
        }
        else
        {
            throw new Exception($"Type of {action.ModelEntity.GetType()} is not supported");
        }

        return state with
        {
            Models = newState
        };
    }

    [ReducerMethod]
    public static CachedModelState ChangeSelectionCommandReducer(
        CachedModelState state,
        ModelEntityCreated action
    )
    {
        if (!state.Models.TryGetValue(action.ModelEntity.ModelId, out var model))
        {
            return state;
        }

        ImmutableDictionary<Guid, CachedModelResponse> newState;
        if (action.ModelEntity is NodeResponse nodeResponse)
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        Nodes = model.Nodes.Add(nodeResponse.Id, nodeResponse)
                    }
                );
        }
        else if (action.ModelEntity is Element1dResponse element1d)
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        Element1ds = model.Element1ds.Add(element1d.Id, element1d)
                    }
                );
        }
        else if (action.ModelEntity is PointLoadResponse pointLoad)
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        PointLoads = model.PointLoads.Add(pointLoad.Id, pointLoad)
                    }
                );
        }
        else
        {
            throw new Exception($"Type of {action.ModelEntity.GetType()} is not supported");
        }

        return state with
        {
            Models = newState
        };
    }

    [ReducerMethod]
    public static CachedModelState ChangeSelectionCommandReducer(
        CachedModelState state,
        ModelEntityDeleted action
    )
    {
        if (!state.Models.TryGetValue(action.ModelEntity.ModelId, out var model))
        {
            return state;
        }

        ImmutableDictionary<Guid, CachedModelResponse> newState;
        if (action.EntityType == "Node")
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        Nodes = model.Nodes.Remove(action.ModelEntity.Id)
                    }
                );
        }
        else if (action.EntityType == "Element1d")
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        Element1ds = model.Element1ds.Remove(action.ModelEntity.Id)
                    }
                );
        }
        else if (action.EntityType == "PointLoad")
        {
            newState = state
                .Models
                .Remove(action.ModelEntity.ModelId)
                .Add(
                    action.ModelEntity.ModelId,
                    model with
                    {
                        PointLoads = model.PointLoads.Remove(action.ModelEntity.Id)
                    }
                );
        }
        else
        {
            throw new Exception($"Type of {action.ModelEntity.GetType()} is not supported");
        }

        return state with
        {
            Models = newState
        };
    }
}
