namespace BeamOs.CodeGen.Apis.EditorApi;

public partial interface IEditorApiAlpha : IAsyncDisposable { }

public partial class EditorApiAlpha
{
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
