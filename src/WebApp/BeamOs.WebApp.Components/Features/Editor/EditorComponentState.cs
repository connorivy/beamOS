using System.Collections.Immutable;
using BeamOs.CodeGen.EditorApi;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.ModelObjectEditor;
using BeamOs.WebApp.Components.Features.ModelObjectEditor.Members;
using BeamOs.WebApp.Components.Features.ModelObjectEditor.Nodes;
using BeamOs.WebApp.Components.Features.StructuralApi;
using BeamOs.WebApp.EditorCommands;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.Editor;

[FeatureState]
public record EditorComponentState(
    string? CanvasId,
    string? LoadingText,
    bool IsLoading,
    Guid? LoadedModelId,
    CachedModelResponse? CachedModelResponse,
    IEditorApiAlpha? EditorApi,
    SelectedObject[] SelectedObjects,
    bool HasResults
)
{
    public EditorComponentState()
        : this(null, null, false, null, null, null, [], false) { }
}

public static class EditorComponentStateReducers
{
    [ReducerMethod]
    public static EditorComponentState EditorCreatedReducer(
        EditorComponentState state,
        EditorCreated action
    )
    {
        return state with { CanvasId = action.CanvasId };
    }

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

    [ReducerMethod(typeof(EditorLoadingEnd))]
    public static EditorComponentState EditorLoadingEndReducer(EditorComponentState state)
    {
        return state with { IsLoading = false };
    }

    [ReducerMethod]
    public static EditorComponentState EditorLoadingBeginReducer(
        EditorComponentState state,
        ModelLoaded action
    )
    {
        return state with
        {
            CachedModelResponse = action.CachedModelResponse,
            LoadedModelId = action.CachedModelResponse.Id,
            HasResults = (action.CachedModelResponse.NodeResults?.Count ?? 0) > 0
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

    [ReducerMethod(typeof(AnalyticalResultsCreated))]
    public static EditorComponentState ResultsCreatedReducer(EditorComponentState state)
    {
        return state with { HasResults = true };
    }

    [ReducerMethod(typeof(AnalyticalResultsCleared))]
    public static EditorComponentState ResultsClearedReducer(EditorComponentState state)
    {
        return state with { HasResults = false };
    }

    [ReducerMethod]
    public static EditorComponentState Reducer(
        EditorComponentState state,
        PutNodeClientCommand action
    )
    {
        Guid modelId = action.New.ModelId;

        if (!(state.CachedModelResponse?.Id == modelId))
        {
            return state;
        }

        return state with
        {
            CachedModelResponse = state.CachedModelResponse with
            {
                Nodes = state
                    .CachedModelResponse
                    .Nodes
                    .Remove(action.New.Id)
                    .Add(action.New.Id, action.New)
            }
        };
    }

    [ReducerMethod]
    public static EditorComponentState Reducer(
        EditorComponentState state,
        DeleteNodeClientCommand action
    )
    {
        Guid modelId = action.ModelId;

        if (!(state.CachedModelResponse?.Id == modelId))
        {
            return state;
        }

        return state with
        {
            CachedModelResponse = state.CachedModelResponse with
            {
                Nodes = state.CachedModelResponse.Nodes.Remove(action.NodeId)
            }
        };
    }

    [ReducerMethod]
    public static EditorComponentState Reducer(
        EditorComponentState state,
        CreateNodeClientCommand action
    )
    {
        Guid modelId = action.ModelId;

        if (!(state.CachedModelResponse?.Id == modelId))
        {
            return state;
        }

        if (action.NodeId is null)
        {
            throw new InvalidOperationException(
                "CreateNodeClientCommand does not have a valid NodeId"
            );
        }

        return state with
        {
            CachedModelResponse = state.CachedModelResponse with
            {
                Nodes = state
                    .CachedModelResponse
                    .Nodes
                    .Add(action.NodeId.Value, new(action.NodeId.Value, action.ModelId, action.Data))
            }
        };
    }

    [ReducerMethod]
    public static EditorComponentState Reducer(
        EditorComponentState state,
        PutElement1dClientCommand action
    )
    {
        Guid modelId = action.New.ModelId;

        if (!(state.CachedModelResponse?.Id == modelId))
        {
            return state;
        }

        return state with
        {
            CachedModelResponse = state.CachedModelResponse with
            {
                Element1ds = state
                    .CachedModelResponse
                    .Element1ds
                    .Remove(action.New.Id)
                    .Add(action.New.Id, action.New)
            }
        };
    }

    [ReducerMethod]
    public static EditorComponentState Reducer(
        EditorComponentState state,
        DeleteElement1dClientCommand action
    )
    {
        Guid modelId = action.ModelId;

        if (!(state.CachedModelResponse?.Id == modelId))
        {
            return state;
        }

        return state with
        {
            CachedModelResponse = state.CachedModelResponse with
            {
                Element1ds = state.CachedModelResponse.Element1ds.Remove(action.Element1dId)
            }
        };
    }

    [ReducerMethod]
    public static EditorComponentState Reducer(
        EditorComponentState state,
        CreateElement1dClientCommand action
    )
    {
        Guid modelId = action.ModelId;

        if (!(state.CachedModelResponse?.Id == modelId))
        {
            return state;
        }

        if (action.Element1dId is null)
        {
            throw new InvalidOperationException(
                "CreateElement1dClientCommand does not have a valid Element1dId"
            );
        }

        return state with
        {
            CachedModelResponse = state.CachedModelResponse with
            {
                Element1ds = state
                    .CachedModelResponse
                    .Element1ds
                    .Add(
                        action.Element1dId ?? action.TempElement1dId,
                        new(
                            action.Element1dId.Value,
                            action.ModelId,
                            action.Data.StartNodeId,
                            action.Data.EndNodeId,
                            action.Data.MaterialId,
                            action.Data.SectionProfileId,
                            action.Data.SectionProfileRotation.Value,
                            null
                        )
                    )
            }
        };
    }

    [ReducerMethod]
    public static CachedModelState EditorLoadingBeginReducer(
        CachedModelState state,
        ModelLoaded action
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
    public static CachedModelState Reducer(CachedModelState state, AnalyticalResultsCreated action)
    {
        if (!state.Models.TryGetValue(action.AnalyticalResults.ModelId, out var model))
        {
            return state;
        }

        return state with
        {
            Models = state
                .Models
                .Remove(action.AnalyticalResults.ModelId)
                .Add(
                    action.AnalyticalResults.ModelId,
                    model with
                    {
                        ShearDiagrams = action
                            .AnalyticalResults
                            .ShearDiagrams
                            .ToImmutableDictionary(d => d.Element1dId, d => d),
                        MomentDiagrams = action
                            .AnalyticalResults
                            .MomentDiagrams
                            .ToImmutableDictionary(d => d.Element1dId, d => d),
                        DeflectionDiagrams = action
                            .AnalyticalResults
                            .DeflectionDiagrams
                            .ToImmutableDictionary(d => d.Element1dId, d => d),
                    }
                )
        };
    }

    [ReducerMethod]
    public static CachedModelState Reducer(CachedModelState state, AnalyticalResultsCleared action)
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
                        NodeResults = null,
                        ShearDiagrams = null,
                        MomentDiagrams = null,
                        DeflectionDiagrams = null,
                    }
                )
        };
    }
}

public readonly record struct AnalysisBegan
{
    public required Guid ModelId { get; init; }
}

public readonly record struct AnalysisEnded
{
    public required Guid ModelId { get; init; }
}

public readonly record struct AnalyticalResultsCreated
{
    public required AnalyticalResultsResponse AnalyticalResults { get; init; }
}

public readonly record struct AnalyticalResultsCleared
{
    public required Guid ModelId { get; init; }
}
