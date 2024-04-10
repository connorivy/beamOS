using BeamOs.Domain.Common.Models;

namespace BeamOs.Domain.Diagrams.Common.ValueObjects;

public class DiagramValueAtLocation : BeamOSValueObject
{
    public DiagramValueAtLocation(
        double valueOnLeft,
        double valueOnRight
    //bool isBetweenConsistantIntervals
    )
    {
        this.ValueOnLeft = valueOnLeft;
        this.ValueOnRight = valueOnRight;
        //this.IsBetweenConsistantIntervals = isBetweenConsistantIntervals;
    }

    public DiagramValueAtLocation(double value)
        : this(value, value) { }

    public double ValueOnLeft { get; }
    public double ValueOnRight { get; }

    //public bool IsBetweenConsistantIntervals { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ValueOnLeft;
        yield return this.ValueOnRight;
        //yield return this.IsBetweenConsistantIntervals;
    }

    public static implicit operator DiagramValueAtLocation(double value)
    {
        return new(value);
    }

    public static implicit operator DiagramValueAtLocation(ValueTuple<double, double> value)
    {
        return new(value.Item1, value.Item2);
    }
}
