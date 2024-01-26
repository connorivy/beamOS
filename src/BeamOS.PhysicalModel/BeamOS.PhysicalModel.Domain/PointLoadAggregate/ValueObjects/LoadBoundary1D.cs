using BeamOS.Common.Domain.Models;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;

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
