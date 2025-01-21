using System.Text.Json.Serialization;
using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.WebApp.EditorCommands;

public readonly record struct MoveNodeCommand : IEditorCommandUndoable
{
    public MoveNodeCommand() { }

    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();
    public required string CanvasId { get; init; }
    public required Guid NodeId { get; init; }
    public required Coordinate3D PreviousLocation { get; init; }
    public required Coordinate3D NewLocation { get; init; }
    public required ClientActionSource Source { get; init; }

    public IClientCommandUndoable GetUndoCommand(ClientActionSource clientEventSource) =>
        this with
        {
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation,
            Source = clientEventSource
        };

    public IClientCommand WithSource(ClientActionSource clientEventSource) =>
        this with
        {
            Source = clientEventSource
        };
}

public readonly record struct Coordinate3D(double X, double Y, double Z);
