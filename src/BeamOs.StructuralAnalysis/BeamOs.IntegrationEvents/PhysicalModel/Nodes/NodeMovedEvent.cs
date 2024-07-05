using BeamOs.Common.Domain.Models;
using BeamOs.IntegrationEvents.Common;

namespace BeamOs.IntegrationEvents.PhysicalModel.Nodes;

public class NodeMovedEvent : BeamOSValueObject, IUndoable, IEditorAction
{
    public required Guid NodeId { get; init; }
    public required Coordinate3D PreviousLocation { get; init; }
    public required Coordinate3D NewLocation { get; init; }
    public string FullType => typeof(NodeMovedEvent).FullName;

    public IUndoable GetUndoAction() =>
        new NodeMovedEvent
        {
            NodeId = this.NodeId,
            NewLocation = this.PreviousLocation,
            PreviousLocation = this.NewLocation
        };

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.NodeId;
        yield return this.PreviousLocation;
        yield return this.NewLocation;
    }
}

public class Coordinate3D(double x, double y, double z) : BeamOSValueObject
{
    public double X { get; init; } = x;
    public double Y { get; init; } = y;
    public double Z { get; init; } = z;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Math.Round(this.X, 14);
        yield return Math.Round(this.Y, 14);
        yield return Math.Round(this.Z, 14);
    }
}
