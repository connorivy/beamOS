using System.Text.Json.Serialization;

namespace BeamOs.WebApp.Client.Events.Interfaces;

public interface IClientCommand { }

public interface IEditorCommand : IClientCommand { }

public interface ICSharpCommand : IClientCommand { }

public interface IClientCommandWithSource : IClientCommand
{
    public ClientActionSource Source { get; init; }
    public IClientCommand WithSource(ClientActionSource clientEventSource);
}

public interface IClientCommandUndoable : IClientCommandWithSource
{
    [JsonIgnore]
    public Guid Id { get; }
    public IClientCommandUndoable GetUndoCommand(ClientActionSource clientEventSource);
}

public interface IEditorCommandUndoable
    : IEditorCommand,
        IClientCommandUndoable,
        IClientCommandWithSource { }

public enum ClientActionSource
{
    Undefined = 0,
    CSharp = 1,
    Editor = 2
}
