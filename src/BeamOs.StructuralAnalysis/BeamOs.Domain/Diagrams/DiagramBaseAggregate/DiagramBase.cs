using System.Numerics;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using MathNet.Numerics;
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

    public (double, double) MinMax()
    {
        double min = double.MaxValue;
        double max = double.MinValue;
        this.MinMax(ref min, ref max);

        return (min, max);
    }

    public void MinMax(ref double min, ref double max)
    {
        foreach (var i in this.Intervals)
        {
            if (i.Polynomial.Degree == -1)
            {
                AssignMinMax(ref min, ref max, 0);
                continue;
            }
            if (i.Polynomial.Degree == 0)
            {
                AssignMinMax(ref min, ref max, i.Polynomial.Coefficients[0]);
                continue;
            }

            var roots = i.Polynomial.Differentiate().Roots();
            foreach (var root in roots)
            {
                if (
                    !root.IsReal()
                    || root.Real < i.StartLocation.As(this.LengthUnit)
                    || root.Real > i.EndLocation.As(this.LengthUnit)
                )
                {
                    continue;
                }

                AssignMinMax(ref min, ref max, i.Polynomial.Evaluate(root.Real));
            }
            AssignMinMax(
                ref min,
                ref max,
                i.Polynomial.Evaluate(i.StartLocation.As(this.LengthUnit))
            );
            AssignMinMax(
                ref min,
                ref max,
                i.Polynomial.Evaluate(i.EndLocation.As(this.LengthUnit))
            );
        }
    }

    private static void AssignMinMax(ref double min, ref double max, double rootValue)
    {
        min = Math.Min(min, rootValue);
        max = Math.Max(max, rootValue);
    }

    //public DiagramValueAtLocation GetValueAtLocation(Length location)
    //{
    //    return this.Intervals.GetValueAtLocation(location, this.EqualityTolerance);
    //}
}
