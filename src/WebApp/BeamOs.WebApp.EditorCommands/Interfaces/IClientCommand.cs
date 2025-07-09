using System.Text.Json.Serialization;

namespace BeamOs.WebApp.EditorCommands.Interfaces;

public interface IClientCommand { }

public interface IEditorCommand : IClientCommand { }

public interface ICSharpCommand : IClientCommand { }

public interface IBeamOsClientCommand : IEditorCommand
{
    [JsonIgnore]
    public Guid Id { get; }

    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }
}

public interface IBeamOsUndoableClientCommand : IBeamOsClientCommand
{
    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null);
    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null);
}

public readonly record struct BeamOsClientCommandArgs
{
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    public static BeamOsClientCommandArgs Unhandled { get; } =
        new()
        {
            HandledByBlazor = false,
            HandledByEditor = false,
            HandledByServer = false,
        };
}

public enum ClientActionSource
{
    Undefined = 0,
    CSharp = 1,
    Editor = 2,
}
