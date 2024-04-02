using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.Extensions;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using MathNet.Numerics;
using MathNet.Numerics.Integration;
using UnitsNet;

namespace BeamOs.Domain.Diagrams;

public class Diagram
{
    private readonly Length elementLength;

    //public Diagram(
    //    Dictionary<double, ImmutablePointLoad> pointLoadsMap,
    //    ShearForceDiagramId? id = null
    //)
    ////: base(id ?? new())
    //{
    //    this.PointLoadsMap = pointLoadsMap;
    //}

    public Dictionary<double, ImmutablePointLoad> PointLoadsMap { get; private set; }
    public List<DiagramConsistantInterval> Intervals { get; private set; } = [new(0, 1, new())];

    public void Build(
        IEnumerable<DiagramPointValue> pointValues,
        IEnumerable<DiagramDistributedValue> distributedValues,
        Length elementLength
    )
    {
        var x = pointValues
            .Select(pv => pv.Location)
            .Concat(distributedValues.Select(dv => dv.StartLocation))
            .Concat(distributedValues.Select(dv => dv.EndLocation))
            .Select(d => Math.Round(elementLength.Value * d, 2))
            .Distinct()
            .Order()
            .ToList();

        List<DiagramConsistantInterval> intervals = [new(0, elementLength.Value, new())];

        foreach (double point in x)
        {
            InsertPointInIntervals(intervals, point);
        }

        foreach (var pointValue in pointValues)
        {
            AddPointValue(
                intervals,
                pointValue.Location * elementLength.Value,
                pointValue.Polynomial
            );
        }

        foreach (var distributedValue in distributedValues)
        {
            AddDistributedValue(
                intervals,
                distributedValue.StartLocation * elementLength.Value,
                distributedValue.EndLocation * elementLength.Value,
                distributedValue.Polynomial
            );
        }

        this.Intervals = intervals;
    }

    private static void InsertPointInIntervals(
        List<DiagramConsistantInterval> intervals,
        double point
    )
    {
        for (int i = 0; i < intervals.Count; i++)
        {
            var interval = intervals[i];
            if (point > interval.EndLocation)
            {
                continue;
            }
            else if (point > interval.StartLocation)
            {
                intervals.Insert(
                    i + 1,
                    new(point, interval.EndLocation, interval.PolynomialDescription)
                );
                interval.EndLocation = point;
            }
            return;
        }
    }

    private static void AddPointValue(
        List<DiagramConsistantInterval> intervals,
        double startLocation,
        Polynomial polynomialValue
    )
    {
        int intervalIndex = intervals.FindIndex(i => i.StartLocation == startLocation);

        if (intervalIndex < 0)
        {
            throw new Exception($"Could not find interval with start location {startLocation}");
        }

        foreach (var interval in intervals.Skip(intervalIndex))
        {
            interval.PolynomialDescription += polynomialValue;
        }
    }

    private static void AddDistributedValue(
        List<DiagramConsistantInterval> intervals,
        double startLocation,
        double endLocation,
        Polynomial polynomialValue
    )
    {
        int startIntervalIndex = intervals.FindIndex(i => i.StartLocation == startLocation);
        int endIntervalIndex = intervals.FindIndex(i => i.StartLocation == endLocation);

        if (startIntervalIndex < 0 || endIntervalIndex < 0)
        {
            throw new Exception(
                $"Could not find interval with start location {startLocation} or {endLocation}"
            );
        }

        Polynomial integrated = polynomialValue.Integrate();
        double boundedIntegral =
            integrated.Evaluate(endLocation) - integrated.Evaluate(startLocation);

        int indexDiff = endIntervalIndex - startIntervalIndex;
        foreach (var interval in intervals.Skip(startIntervalIndex).Take(indexDiff))
        {
            interval.PolynomialDescription += integrated;
        }

        foreach (var interval in intervals.Skip(startIntervalIndex + indexDiff))
        {
            interval.PolynomialDescription += boundedIntegral;
        }
    }

    public void Integrate(double valueAtZero)
    {
        double previousEndValue = valueAtZero;
        foreach (var interval in this.Intervals)
        {
            Polynomial integrated = interval.PolynomialDescription.Integrate();
            double yIntercept = (previousEndValue - integrated).Evaluate(interval.StartLocation);

            double boundedIntegral =
                integrated.Evaluate(interval.EndLocation)
                - integrated.Evaluate(interval.StartLocation);
            previousEndValue += boundedIntegral;
        }
    }
}

public class DiagramPointValue
{
    public DiagramPointValue(double location, Polynomial polynomial)
    {
        this.Location = location;
        this.Polynomial = polynomial;
    }

    public double Location { get; set; }
    public Polynomial Polynomial { get; set; }
}

public class DiagramDistributedValue
{
    public DiagramDistributedValue(double startLocation, double endLocation, Polynomial polynomial)
    {
        this.StartLocation = startLocation;
        this.EndLocation = endLocation;
        this.Polynomial = polynomial;
    }

    public double StartLocation { get; set; }
    public double EndLocation { get; set; }
    public Polynomial Polynomial { get; set; }
}
