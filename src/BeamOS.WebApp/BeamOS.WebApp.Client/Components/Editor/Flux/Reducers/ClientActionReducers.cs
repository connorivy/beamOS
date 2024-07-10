using System.Collections.Immutable;
using BeamOS.WebApp.Client.Components.Editor.Flux.Actions;
using BeamOS.WebApp.Client.Components.Editor.Flux.Events;
using Fluxor;
using static BeamOS.WebApp.Client.Components.Editor.EditorComponent;

namespace BeamOS.WebApp.Client.Components.Editor.Flux.Reducers;

public static class ClientActionReducers
{
    [ReducerMethod]
    public static EditorComponentStates ReduceAddNodeAction(
        EditorComponentStates state,
        AddNodeAction action
    )
    {
        if (
            state
                .CanvasIdToEditorComponentStates
                .TryGetValue(action.CanvasId, out var componentState)
        )
        {
            componentState.NodeIdToResponsesDict[action.Node.Id] = action.Node;
        }

        return state;
    }

    [ReducerMethod]
    public static EditorComponentState2 ReduceAddElement1dAction(
        EditorComponentState2 state,
        AddElement1dAction action
    )
    {
        return state with
        {
            Element1dIdToResponsesDict = new(state.Element1dIdToResponsesDict)
            {
                { action.Element1d.Id, action.Element1d }
            }
        };
    }

    [ReducerMethod]
    public static EditorComponentStates ReduceModelLoadedAction(
        EditorComponentStates state,
        ModelLoadedAction action
    )
    {
        Dictionary<string, EditorComponentState2> dictCopy =
            new(state.CanvasIdToEditorComponentStates)
            {
                { action.ModelId, new(false, "", action.ModelId, [], [], []) }
            };

        return state with
        {
            CanvasIdToEditorComponentStates = dictCopy.ToImmutableDictionary()
        };
    }

    [ReducerMethod]
    public static EditorComponentStates ReduceModelUnloadedAction(
        EditorComponentStates state,
        ModelUnloadedAction action
    )
    {
        Dictionary<string, EditorComponentState2> dictCopy =
            new(state.CanvasIdToEditorComponentStates);
        dictCopy.Remove(action.ModelId);

        return state with
        {
            CanvasIdToEditorComponentStates = dictCopy.ToImmutableDictionary()
        };
    }
}
