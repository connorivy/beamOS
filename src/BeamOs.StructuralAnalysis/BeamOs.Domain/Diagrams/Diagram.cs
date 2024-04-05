using BeamOs.Domain.Diagrams.Common;
using MathNet.Numerics;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams;

public class Diagram
{
    private static readonly Length EqualityTolerance = new(1, LengthUnit.Inch);
    public Length ElementLength { get; private set; }
    public LengthUnit LengthUnit { get; private set; }
    public Dictionary<Ratio, Length> RatioToLengthDict { get; private set; }
    public List<DiagramConsistantInterval> Intervals { get; private set; }

    public Diagram(
        Length elementLength,
        Dictionary<Ratio, Length> ratioToLengthDict,
        List<DiagramConsistantInterval> intervals,
        LengthUnit lengthUnit
    )
    {
        this.ElementLength = elementLength;
        this.RatioToLengthDict = ratioToLengthDict;
        this.Intervals = intervals;
        this.LengthUnit = lengthUnit;
    }

    public static Diagram Build(
        IEnumerable<DiagramPointValue> pointValues,
        IEnumerable<DiagramDistributedValue> distributedValues,
        Length elementLength,
        LengthUnit lengthUnit
    )
    {
        var ratioToLengthDict = pointValues
            .Select(pv => pv.Location)
            .Concat(distributedValues.Select(dv => dv.StartLocation))
            .Concat(distributedValues.Select(dv => dv.EndLocation))
            .Distinct()
            .ToDictionary(d => d, d => elementLength * d.As(RatioUnit.DecimalFraction));

        var intervals = BuildConsistantIntervals(
            pointValues,
            distributedValues,
            elementLength,
            lengthUnit,
            ratioToLengthDict
        );

        return new Diagram(elementLength, ratioToLengthDict, intervals, lengthUnit);
    }

    private static List<DiagramConsistantInterval> BuildConsistantIntervals(
        IEnumerable<DiagramPointValue> pointValues,
        IEnumerable<DiagramDistributedValue> distributedValues,
        Length elementLength,
        LengthUnit lengthUnit,
        Dictionary<Ratio, Length> ratioToLengthDict
    )
    {
        List<DiagramConsistantInterval> intervals =
        [
            new(new Length(0, elementLength.Unit), elementLength, new())
        ];

        foreach (Length point in ratioToLengthDict.Values.OrderBy(l => l.As(lengthUnit)))
        {
            InsertPointInIntervals(intervals, point);
        }

        foreach (var pointValue in pointValues)
        {
            AddPointValue(intervals, ratioToLengthDict[pointValue.Location], pointValue.Polynomial);
        }

        foreach (var distributedValue in distributedValues)
        {
            AddDistributedValue(
                intervals,
                ratioToLengthDict[distributedValue.StartLocation],
                ratioToLengthDict[distributedValue.EndLocation],
                distributedValue.Polynomial,
                lengthUnit
            );
        }

        return intervals;
    }

    private static void InsertPointInIntervals(
        List<DiagramConsistantInterval> intervals,
        Length locationAlongBeam
    )
    {
        for (int i = 0; i < intervals.Count; i++)
        {
            var interval = intervals[i];
            if (locationAlongBeam > interval.EndLocation)
            {
                continue;
            }
            else if (locationAlongBeam > interval.StartLocation)
            {
                intervals.Insert(
                    i + 1,
                    new(locationAlongBeam, interval.EndLocation, interval.PolynomialDescription)
                );
                interval.EndLocation = locationAlongBeam;
            }
            return;
        }
    }

    private static void AddPointValue(
        List<DiagramConsistantInterval> intervals,
        Length startLocation,
        Polynomial polynomialValue
    )
    {
        int intervalIndex = intervals.FindIndex(
            i => i.StartLocation.Equals(startLocation, EqualityTolerance)
        );

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
        Length startLocation,
        Length endLocation,
        Polynomial polynomialValue,
        LengthUnit lengthUnit
    )
    {
        int startIntervalIndex = intervals.FindIndex(
            i => i.StartLocation.Equals(startLocation, EqualityTolerance)
        );
        int endIntervalIndex = intervals.FindIndex(
            i => i.StartLocation.Equals(endLocation, EqualityTolerance)
        );

        if (startIntervalIndex < 0 || endIntervalIndex < 0)
        {
            throw new Exception(
                $"Could not find interval with start location {startLocation} or {endLocation}"
            );
        }

        Polynomial integrated = polynomialValue.Integrate();
        double boundedIntegral = 0;

        int indexDiff = endIntervalIndex - startIntervalIndex;
        foreach (var interval in intervals.Skip(startIntervalIndex).Take(indexDiff))
        {
            double integralAtStart = integrated.Evaluate(interval.StartLocation.As(lengthUnit));
            double integralAtEnd = integrated.Evaluate(interval.EndLocation.As(lengthUnit));

            interval.PolynomialDescription +=
                integrated + new Polynomial(boundedIntegral - integralAtStart);
            boundedIntegral += integralAtEnd - integralAtStart;
        }

        foreach (var interval in intervals.Skip(startIntervalIndex + indexDiff))
        {
            interval.PolynomialDescription += boundedIntegral;
        }
    }

    public Diagram Integrate(
        IEnumerable<DiagramPointValue> pointValues,
        IEnumerable<DiagramDistributedValue> additionalDistributedValues
    )
    {
        var ratioToLengthDict = this.RatioToLengthDict
            .Select(i => i.Key)
            .Concat(pointValues.Select(pv => pv.Location))
            .Concat(additionalDistributedValues.Select(dv => dv.StartLocation))
            .Concat(additionalDistributedValues.Select(dv => dv.EndLocation))
            .Distinct()
            .ToDictionary(d => d, d => this.ElementLength * d.As(RatioUnit.DecimalFraction));

        var intervals = BuildConsistantIntervals(
            pointValues,
            this.GetDistributedValuesFromDiagram().Concat(additionalDistributedValues),
            ElementLength,
            LengthUnit,
            ratioToLengthDict
        );

        return new Diagram(ElementLength, ratioToLengthDict, intervals, LengthUnit);
    }

    private IEnumerable<DiagramDistributedValue> GetDistributedValuesFromDiagram()
    {
        foreach (var interval in this.Intervals)
        {
            Ratio startRatio =
                new(interval.StartLocation / this.ElementLength, RatioUnit.DecimalFraction);
            Ratio endRatio =
                new(interval.EndLocation / this.ElementLength, RatioUnit.DecimalFraction);
            yield return new(startRatio, endRatio, interval.PolynomialDescription);
        }
    }
}

public class DiagramPointValue
{
    public DiagramPointValue(Ratio location, Polynomial polynomial)
    {
        this.Location = location;
        this.Polynomial = polynomial;
    }

    public Ratio Location { get; set; }
    public Polynomial Polynomial { get; set; }
}

public class DiagramDistributedValue
{
    public DiagramDistributedValue(Ratio startLocation, Ratio endLocation, Polynomial polynomial)
    {
        this.StartLocation = startLocation;
        this.EndLocation = endLocation;
        this.Polynomial = polynomial;
    }

    public Ratio StartLocation { get; set; }
    public Ratio EndLocation { get; set; }
    public Polynomial Polynomial { get; set; }
}
