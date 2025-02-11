using System.Collections.Immutable;
using BeamOs.CodeGen.EditorApi;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.WebApp.Components.Features.ApiResponseHandlers;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.StructuralApi;
using BeamOs.WebApp.EditorCommands;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.Editor;

[FeatureState]
public record EditorComponentState(
    string? LoadingText,
    bool IsLoading,
    Guid? LoadedModelId,
    IEditorApiAlpha? EditorApi,
    SelectedObject[] SelectedObjects
)
{
    public EditorComponentState()
        : this(null, false, null, null, []) { }
}

public static class EditorComponentStateReducers
{
    [ReducerMethod]
    public static EditorComponentState EditorCreatedReducer(
        EditorComponentState state,
        EditorApiCreated action
    )
    {
        return state with { EditorApi = action.EditorApiAlpha };
    }

    [ReducerMethod]
    public static EditorComponentState EditorLoadingBeginReducer(
        EditorComponentState state,
        EditorLoadingBegin action
    )
    {
        return state with { IsLoading = true, LoadingText = action.LoadingText };
    }

    [ReducerMethod]
    public static EditorComponentState EditorLoadingBeginReducer(
        EditorComponentState state,
        EditorLoadingEnd action
    )
    {
        return state with
        {
            IsLoading = false,
            //CachedModelResponse = action.CachedModelResponse,
            LoadedModelId = action.CachedModelResponse.Id
        };
    }

    [ReducerMethod]
    public static EditorComponentState ChangeSelectionCommandReducer(
        EditorComponentState state,
        ChangeSelectionCommand action
    )
    {
        return state with { SelectedObjects = action.SelectedObjects };
    }

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
    public static CachedModelState Reducer(CachedModelState state, ModelEntityCreated action)
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
    public static CachedModelState Reducer(CachedModelState state, ModelEntityDeleted action)
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

    [ReducerMethod]
    public static CachedModelState Reducer(CachedModelState state, DiagramResponseCreated action)
    {
        if (!state.Models.TryGetValue(action.ModelId, out var model))
        {
            return state;
        }

        return state with
        {
            Models = state
                .Models
                .Remove(action.ModelId)
                .Add(
                    action.ModelId,
                    model with
                    {
                        ShearDiagrams = action
                            .DiagramResponse
                            .ShearDiagrams
                            .ToImmutableDictionary(d => d.Element1dId, d => d),
                        MomentDiagrams = action
                            .DiagramResponse
                            .MomentDiagrams
                            .ToImmutableDictionary(d => d.Element1dId, d => d),
                        DeflectionDiagrams = action
                            .DiagramResponse
                            .DeflectionDiagrams
                            .ToImmutableDictionary(d => d.Element1dId, d => d),
                    }
                )
        };
    }
}
