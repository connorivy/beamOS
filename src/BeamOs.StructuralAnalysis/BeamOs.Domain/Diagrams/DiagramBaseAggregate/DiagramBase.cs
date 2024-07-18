using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.DiagramBaseAggregate;

public abstract class DiagramBase<TId> : AggregateRoot<TId>
    where TId : notnull
{
    public Length ElementLength { get; private set; }
    public Length EqualityTolerance { get; private set; }
    public LengthUnit LengthUnit { get; private set; }
    public List<DiagramConsistantInterval> Intervals { get; private set; }

    protected DiagramBase(
        Length elementLength,
        LengthUnit lengthUnit,
        List<DiagramConsistantInterval> intervals,
        TId id
    )
        : base(id)
    {
        this.ElementLength = elementLength;
        this.Intervals = intervals;
        this.LengthUnit = lengthUnit;
    }

    public DiagramBuilder Integrate()
    {
        return new DiagramBuilder(
            this.ElementLength,
            this.EqualityTolerance,
            this.LengthUnit,
            this.Intervals
        );
    }

    //public DiagramValueAtLocation GetValueAtLocation(Length location)
    //{
    //    return this.Intervals.GetValueAtLocation(location, this.EqualityTolerance);
    //}
}
