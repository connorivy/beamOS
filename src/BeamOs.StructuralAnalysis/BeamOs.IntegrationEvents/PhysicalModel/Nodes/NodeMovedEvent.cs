using System.Text.Json.Serialization;
using BeamOs.IntegrationEvents.Common;

namespace BeamOs.IntegrationEvents.PhysicalModel.Nodes;

public readonly record struct NodeMovedEvent : IIntegrationEvent, IUndoable, IEditorAction
{
    public NodeMovedEvent() { }

    public required Guid NodeId { get; init; }
    public required Coordinate3D PreviousLocation { get; init; }
    public required Coordinate3D NewLocation { get; init; }

    [JsonIgnore]
    public bool HistoryNeedsUpdating { get; init; } = true;

    [JsonIgnore]
    public bool EditorNeedsUpdating { get; init; } = true;

    [JsonIgnore]
    public bool DbNeedsUpdating { get; init; } = true;

    public IUndoable GetUndoAction() =>
        this with
        {
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation
        };
}

public readonly record struct Coordinate3D(double X, double Y, double Z);
