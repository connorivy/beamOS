using System.Text.Json.Serialization;
using BeamOs.IntegrationEvents.Common;

namespace BeamOs.IntegrationEvents.PhysicalModel.Nodes;

public readonly record struct NodeMovedEvent : IIntegrationEvent, IUndoable, IEditorAction
{
    public NodeMovedEvent() { }

    public required Guid NodeId { get; init; }
    public required Coordinate3D PreviousLocation { get; init; }
    public required Coordinate3D NewLocation { get; init; }
    public string FullType => typeof(NodeMovedEvent).FullName;

    [JsonIgnore]
    public bool HistoryUpdated { get; init; }

    [JsonIgnore]
    public bool EditorUpdated { get; init; }

    public bool DbUpdated { get; init; }

    public IUndoable GetUndoAction() =>
        this with
        {
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation
        };
}

public readonly record struct Coordinate3D(double X, double Y, double Z);
