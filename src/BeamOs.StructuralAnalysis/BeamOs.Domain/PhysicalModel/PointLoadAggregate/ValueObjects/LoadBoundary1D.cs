using BeamOs.Domain.Common.Models;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;

internal class LoadBoundary1D : BeamOSValueObject
{
    public LoadBoundary1D(Length distanceFromStart)
    {
        this.DistanceFromStart = distanceFromStart;
    }

    public Length DistanceFromStart { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.DistanceFromStart;
    }
}
