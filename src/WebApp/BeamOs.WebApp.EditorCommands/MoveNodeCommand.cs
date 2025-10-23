using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.WebApp.EditorCommands;

public readonly record struct MoveNodeCommand : IBeamOsUndoableClientCommand
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

    public IBeamOsUndoableClientCommand GetUndoCommand(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation,
        };

    public IBeamOsUndoableClientCommand WithArgs(BeamOsClientCommandArgs? args = null) =>
        this with
        {
            HandledByBlazor = args?.HandledByBlazor ?? this.HandledByBlazor,
            HandledByEditor = args?.HandledByEditor ?? this.HandledByEditor,
        };
}

public record Coordinate3D
{
    public Coordinate3D() { }

    [SetsRequiredMembers]
    public Coordinate3D(double x, double y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public required double X { get; init; }
    public required double Y { get; init; }
    public required double Z { get; init; }
}
