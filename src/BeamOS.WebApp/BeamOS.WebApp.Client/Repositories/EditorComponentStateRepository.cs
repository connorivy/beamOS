namespace BeamOS.WebApp.Client.Repositories;

public class GenericComponentStateRepository<TState> : IStateRepository<TState>
    where TState : new()
{
    private readonly Dictionary<string, TState> canvasIdToModelIdDict = [];

    public TState? GetComponentStateByCanvasId(string canvasId) =>
        this.canvasIdToModelIdDict.GetValueOrDefault(canvasId);

    public TState? GetOrSetComponentStateByCanvasId(string canvasId)
    {
        if (this.canvasIdToModelIdDict.TryGetValue(canvasId, out var componentState))
        {
            return componentState;
        }

        componentState = new TState();
        this.canvasIdToModelIdDict.Add(canvasId, componentState);
        return componentState;
    }

    public void SetComponentStateForCanvasId(string canvasId, TState modelId) =>
        this.canvasIdToModelIdDict[canvasId] = modelId;

    public void RemoveEditorComponentStateForCanvasId(string canvasId) =>
        this.canvasIdToModelIdDict.Remove(canvasId);
}
