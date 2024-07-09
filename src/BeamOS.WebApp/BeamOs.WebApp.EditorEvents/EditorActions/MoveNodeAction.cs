using System.Text.Json.Serialization;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Actions.EditorActions;

public readonly record struct MoveNodeAction : IEditorActionUndoable
{
    public MoveNodeAction() { }

    [JsonIgnore]
    public Guid Id { get; } = Guid.NewGuid();
    public required string CanvasId { get; init; }
    public required Guid NodeId { get; init; }
    public required Coordinate3D PreviousLocation { get; init; }
    public required Coordinate3D NewLocation { get; init; }
    public required ClientActionSource Source { get; init; }

    public IClientActionUndoable GetUndoAction(ClientActionSource clientEventSource) =>
        this with
        {
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation,
            Source = clientEventSource
        };

    public IClientAction WithSource(ClientActionSource clientEventSource) =>
        this with
        {
            Source = clientEventSource
        };
}

public readonly record struct Coordinate3D(double X, double Y, double Z);
