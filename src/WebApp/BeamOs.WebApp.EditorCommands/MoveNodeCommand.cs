using System.Text.Json.Serialization;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.WebApp.EditorCommands;

public readonly record struct MoveNodeCommand : IBeamOsClientCommand
{
    public MoveNodeCommand() { }

    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();
    public required string CanvasId { get; init; }
    public required int NodeId { get; init; }
    public required Coordinate3D PreviousLocation { get; init; }
    public required Coordinate3D NewLocation { get; init; }

    //public required ClientActionSource Source { get; init; }
    public bool HandledByEditor { get; init; }
    public bool HandledByBlazor { get; init; }
    public bool HandledByServer { get; init; }

    //public IClientCommandUndoable GetUndoCommand(ClientActionSource clientEventSource) =>
    //    this with
    //    {
    //        NewLocation = this.PreviousLocation,
    //        PreviousLocation = this.NewLocation,
    //        Source = clientEventSource
    //    };

    public IBeamOsClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation
        };

    public IBeamOsClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
        };
}

public readonly record struct Coordinate3D(double X, double Y, double Z);
