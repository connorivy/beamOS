using BeamOs.Common.Domain.Models;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

public class NodeData : BeamOSValueObject
{
    public NodeData(Point locationPoint, Restraint restraint)
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
    }

    public Point LocationPoint { get; }
    public Restraint Restraint { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.LocationPoint;
        yield return this.Restraint;
    }
}
