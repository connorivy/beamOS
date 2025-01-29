using System.Collections.Immutable;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.StructuralApi;
using BeamOs.WebApp.EditorCommands;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.Editor;

[FeatureState]
public record AllEditorComponentState(ImmutableDictionary<string, EditorComponentState> EditorState)
{
    public AllEditorComponentState()
        : this(ImmutableDictionary<string, EditorComponentState>.Empty) { }
}


//public static class AllEditorComponentStateReducers
//{
//    [ReducerMethod]
//    public static AllEditorComponentState EditorCreatedReducer(
//        AllEditorComponentState state,
//        EditorCreated action
//    )
//    {
//        return state with { EditorState = state.EditorState.Add(action.CanvasId, new()) };
//    }

//    [ReducerMethod]
//    public static AllEditorComponentState EditorDisposedReducer(
//        AllEditorComponentState state,
//        EditorDisposed action
//    )
//    {
//        return state with { EditorState = state.EditorState.Remove(action.CanvasId) };
//    }

//    [ReducerMethod]
//    public static AllEditorComponentState EditorCreatedReducer(
//        AllEditorComponentState state,
//        EditorApiCreated action
//    )
//    {
//        var currentEditorState = state.EditorState[action.CanvasId];
//        return state with
//        {
//            EditorState = state
//                .EditorState
//                .Remove(action.CanvasId)
//                .Add(action.CanvasId, currentEditorState with { EditorApi = action.EditorApiAlpha })
//        };
//    }

//    [ReducerMethod]
//    public static AllEditorComponentState EditorLoadingBeginReducer(
//        AllEditorComponentState state,
//        EditorLoadingBegin action
//    )
//    {
//        var currentEditorState = state.EditorState[action.CanvasId];
//        return state with
//        {
//            EditorState = state
//                .EditorState
//                .Remove(action.CanvasId)
//                .Add(
//                    action.CanvasId,
//                    currentEditorState with
//                    {
//                        IsLoading = true,
//                        LoadingText = action.LoadingText
//                    }
//                )
//        };
//    }

//    [ReducerMethod]
//    public static AllEditorComponentState EditorLoadingBeginReducer(
//        AllEditorComponentState state,
//        EditorLoadingEnd action
//    )
//    {
//        var currentEditorState = state.EditorState[action.CanvasId];
//        return state with
//        {
//            EditorState = state
//                .EditorState
//                .Remove(action.CanvasId)
//                .Add(
//                    action.CanvasId,
//                    currentEditorState with
//                    {
//                        IsLoading = false,
//                        //CachedModelResponse = action.CachedModelResponse,
//                        LoadedModelId = action.CachedModelResponse.Id
//                    }
//                )
//        };
//    }

//    [ReducerMethod]
//    public static AllEditorComponentState ChangeSelectionCommandReducer(
//        AllEditorComponentState state,
//        ChangeSelectionCommand action
//    )
//    {
//        var currentEditorState = state.EditorState[action.CanvasId];
//        return state with
//        {
//            EditorState = state
//                .EditorState
//                .Remove(action.CanvasId)
//                .Add(
//                    action.CanvasId,
//                    currentEditorState with
//                    {
//                        SelectedObjects = action.SelectedObjects
//                    }
//                )
//        };
//    }

//    [ReducerMethod]
//    public static CachedModelState EditorLoadingBeginReducer(
//        CachedModelState state,
//        EditorLoadingEnd action
//    )
//    {
//        return state with
//        {
//            Models = state
//                .Models
//                .Remove(action.CachedModelResponse.Id)
//                .Add(action.CachedModelResponse.Id, action.CachedModelResponse)
//        };
//    }

//    [ReducerMethod]
//    public static CachedModelState Reducer(CachedModelState state, ModelEntityUpdated action)
//    {
//        if (!state.Models.TryGetValue(action.ModelEntity.ModelId, out var model))
//        {
//            return state;
//        }

//        ImmutableDictionary<Guid, CachedModelResponse> newState;
//        if (action.ModelEntity is NodeResponse nodeResponse)
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        Nodes = model
//                            .Nodes
//                            .Remove(nodeResponse.Id)
//                            .Add(nodeResponse.Id, nodeResponse)
//                    }
//                );
//        }
//        else if (action.ModelEntity is Element1dResponse element1d)
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        Element1ds = model
//                            .Element1ds
//                            .Remove(element1d.Id)
//                            .Add(element1d.Id, element1d)
//                    }
//                );
//        }
//        else if (action.ModelEntity is PointLoadResponse pointLoad)
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        PointLoads = model
//                            .PointLoads
//                            .Remove(pointLoad.Id)
//                            .Add(pointLoad.Id, pointLoad)
//                    }
//                );
//        }
//        else
//        {
//            throw new Exception($"Type of {action.ModelEntity.GetType()} is not supported");
//        }

//        return state with
//        {
//            Models = newState
//        };
//    }

//    [ReducerMethod]
//    public static CachedModelState ChangeSelectionCommandReducer(
//        CachedModelState state,
//        ModelEntityCreated action
//    )
//    {
//        if (!state.Models.TryGetValue(action.ModelEntity.ModelId, out var model))
//        {
//            return state;
//        }

//        ImmutableDictionary<Guid, CachedModelResponse> newState;
//        if (action.ModelEntity is NodeResponse nodeResponse)
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        Nodes = model.Nodes.Add(nodeResponse.Id, nodeResponse)
//                    }
//                );
//        }
//        else if (action.ModelEntity is Element1dResponse element1d)
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        Element1ds = model.Element1ds.Add(element1d.Id, element1d)
//                    }
//                );
//        }
//        else if (action.ModelEntity is PointLoadResponse pointLoad)
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        PointLoads = model.PointLoads.Add(pointLoad.Id, pointLoad)
//                    }
//                );
//        }
//        else
//        {
//            throw new Exception($"Type of {action.ModelEntity.GetType()} is not supported");
//        }

//        return state with
//        {
//            Models = newState
//        };
//    }

//    [ReducerMethod]
//    public static CachedModelState ChangeSelectionCommandReducer(
//        CachedModelState state,
//        ModelEntityDeleted action
//    )
//    {
//        if (!state.Models.TryGetValue(action.ModelEntity.ModelId, out var model))
//        {
//            return state;
//        }

//        ImmutableDictionary<Guid, CachedModelResponse> newState;
//        if (action.EntityType == "Node")
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        Nodes = model.Nodes.Remove(action.ModelEntity.Id)
//                    }
//                );
//        }
//        else if (action.EntityType == "Element1d")
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        Element1ds = model.Element1ds.Remove(action.ModelEntity.Id)
//                    }
//                );
//        }
//        else if (action.EntityType == "PointLoad")
//        {
//            newState = state
//                .Models
//                .Remove(action.ModelEntity.ModelId)
//                .Add(
//                    action.ModelEntity.ModelId,
//                    model with
//                    {
//                        PointLoads = model.PointLoads.Remove(action.ModelEntity.Id)
//                    }
//                );
//        }
//        else
//        {
//            throw new Exception($"Type of {action.ModelEntity.GetType()} is not supported");
//        }

//        return state with
//        {
//            Models = newState
//        };
//    }
//}
