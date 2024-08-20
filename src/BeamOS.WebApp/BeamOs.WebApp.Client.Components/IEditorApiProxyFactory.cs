using BeamOs.CodeGen.Apis.EditorApi;

namespace BeamOs.WebApp.Client.Components;

public interface IEditorApiProxyFactory
{
    Task<IEditorApiAlpha> Create(string canvasId, bool isReadOnly);
}
