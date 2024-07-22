using BeamOs.Common.Events;

namespace BeamOs.IntegrationEvents.PhysicalModel.Nodes;

public readonly record struct NodeMovedEvent : IUndoable
{
    public required Guid NodeId { get; init; }
    public required Coordinate3D PreviousLocation { get; init; }
    public required Coordinate3D NewLocation { get; init; }

    public IUndoable GetUndoAction() =>
        this with
        {
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation
        };
}

public readonly record struct Coordinate3D(double X, double Y, double Z);
