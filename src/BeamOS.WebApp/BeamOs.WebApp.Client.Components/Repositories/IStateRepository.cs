namespace BeamOs.WebApp.Client.Components.Repositories;

public interface IStateRepository<TState>
{
    public TState? GetComponentStateByCanvasId(string canvasId);

    public TState GetOrSetComponentStateByCanvasId(string canvasId);

    public void SetComponentStateForCanvasId(string canvasId, TState modelId);

    public void RemoveEditorComponentStateForCanvasId(string canvasId);
}
