using BeamOs.CodeGen.Apis.EditorApi;

namespace BeamOs.WebApp.Client.Components.Repositories;

public class EditorApiRepository
{
    private readonly Dictionary<string, IEditorApiAlpha> canvasIdToEditorApis = [];

    public IEditorApiAlpha GetEditorApiByCanvasId(string canvasId) =>
        this.canvasIdToEditorApis[canvasId];

    public void AddEditorApiByCanvasId(string canvasId, IEditorApiAlpha editorApiAlpha) =>
        this.canvasIdToEditorApis.Add(canvasId, editorApiAlpha);

    public void RemoveEditorApiByCanvasId(string canvasId) =>
        this.canvasIdToEditorApis.Remove(canvasId);
}

public class ModelIdRepository
{
    private readonly Dictionary<string, string> canvasIdToModelIdDict = [];

    public string? GetModelIdByCanvasId(string canvasId) =>
        this.canvasIdToModelIdDict.GetValueOrDefault(canvasId);

    public void SetModelIdForCanvasId(string canvasId, string modelId) =>
        this.canvasIdToModelIdDict[canvasId] = modelId;
}
