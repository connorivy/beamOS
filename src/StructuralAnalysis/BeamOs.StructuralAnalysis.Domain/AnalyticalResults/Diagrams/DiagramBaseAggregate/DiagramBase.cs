using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using MathNet.Numerics;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.DiagramBaseAggregate;

public abstract class DiagramBase<TId, TInterval> : BeamOsAnalyticalResultEntity<TId>
    where TId : struct, IIntBasedId
    where TInterval : DiagramConsistentInterval
{
    public Length ElementLength { get; private set; }
    public Length EqualityTolerance { get; private set; }
    public LengthUnit LengthUnit { get; private set; }
    public List<TInterval> Intervals { get; private set; }

    protected DiagramBase(
        ModelId modelId,
        ResultSetId resultSetId,
        Length elementLength,
        LengthUnit lengthUnit,
        List<TInterval> intervals,
        TId id
    )
        : base(id, resultSetId, modelId)
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
            this.Intervals.Cast<DiagramConsistentInterval>().ToList()
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

    [Obsolete("EF Ctor")]
    public DiagramBase() { }
}
