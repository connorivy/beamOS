using System.Text.Json.Serialization;

namespace BeamOs.WebApp.Client.Events.Interfaces;

public interface IClientAction { }

public interface IEditorAction : IClientAction { }

public interface ICSharpAction : IClientAction { }

public interface IClientActionWithSource : IClientAction
{
    public ClientActionSource Source { get; init; }
    public IClientAction WithSource(ClientActionSource clientEventSource);
}

public interface IClientActionUndoable : IClientActionWithSource
{
    [JsonIgnore]
    public Guid Id { get; }
    public IClientActionUndoable GetUndoAction(ClientActionSource clientEventSource);
}

public interface IEditorActionUndoable
    : IEditorAction,
        IClientActionUndoable,
        IClientActionWithSource { }

public enum ClientActionSource
{
    Undefined = 0,
    CSharp = 1,
    Editor = 2
}
