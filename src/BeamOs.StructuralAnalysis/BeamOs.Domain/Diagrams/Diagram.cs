using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
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

    public Diagram(
        Length elementLength,
        LengthUnit lengthUnit,
        List<DiagramConsistantInterval> intervals
    )
    {
        this.ElementLength = elementLength;
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
        Dictionary<Ratio, Length> ratioToLengthDict,
        List<DiagramPointValueAsLength>? boundaryConditions = null
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

        if (boundaryConditions != null)
        {
            ApplyBoundaryConditions(
                intervals,
                boundaryConditions,
                lengthUnit,
                boundaryConditions.Count
            );
        }
        //foreach (var boundaryCondition in boundaryConditions ?? [])
        //{
        //    ApplyBoundaryCondition(intervals, boundaryCondition, lengthUnit);
        //}

        return intervals;
    }

    private static void ApplyBoundaryConditions(
        List<DiagramConsistantInterval> intervals,
        List<DiagramPointValueAsLength> boundaryConditions,
        LengthUnit lengthUnit,
        int numTimesIntegrated
    )
    {
        // numTimesIntegrated = 1
        // C = const
        // | 1 | | C | = | const |

        // numTimesIntegrated = 2
        // C1x + C2 = const
        // | x 1 | | C1 | = | f(x1) |
        // | x 1 | | C2 |   | f(x2) |

        // numTimesIntegrated = 3
        // C1x2 + C2x + C3 = const
        // | x2 x 1 | | C1 | = | f(x1) |
        // | x2 x 1 | | C2 |   | f(x2) |
        // | x2 x 1 | | C3 |   | f(x3) |

        if (boundaryConditions.Count != numTimesIntegrated)
        {
            throw new Exception(
                "Number of provided boundary conditions must match number of times being integrated"
            );
        }

        double[,] matrixArray = new double[numTimesIntegrated, numTimesIntegrated];
        double[] results = new double[numTimesIntegrated];
        for (int bcIndex = 0; bcIndex < numTimesIntegrated; bcIndex++)
        {
            DiagramPointValueAsLength bc = boundaryConditions[bcIndex];
            double currentVal = EvaluateIntervalsAtLocationAndThrowIfDifferent(
                intervals,
                lengthUnit,
                bc.Location
            );
            for (int i = 0; i < numTimesIntegrated; i++)
            {
                matrixArray[bcIndex, numTimesIntegrated - (i + 1)] = Math.Pow(
                    bc.Location.As(lengthUnit),
                    i
                );
            }
            results[bcIndex] = bc.Value - currentVal;
        }

        var a = Matrix<double>.Build.DenseOfArray(matrixArray);
        var b = Vector<double>.Build.Dense(results);
        var x = a.Solve(b).ToArray();
    }

    private static double EvaluateIntervalsAtLocationAndThrowIfDifferent(
        List<DiagramConsistantInterval> intervals,
        LengthUnit lengthUnit,
        Length location
    )
    {
        List<DiagramConsistantInterval> containingIntervals = GetContainingIntervals(
                intervals,
                location
            )
            .ToList();
        double valInFirstInterval = containingIntervals[0]
            .PolynomialDescription
            .Evaluate(location.As(lengthUnit));
        if (containingIntervals.Count > 1)
        {
            if (containingIntervals.Count > 2)
            {
                throw new Exception("How did this happen?");
            }

            double valInLastInterval = containingIntervals[1]
                .PolynomialDescription
                .Evaluate(location.As(lengthUnit));
            if (Math.Abs(valInFirstInterval - valInLastInterval) > EqualityTolerance.As(lengthUnit))
            {
                throw new Exception(
                    "Cannot apply boundary condition at point with two different values"
                );
            }
        }

        return valInFirstInterval;
    }

    private static IEnumerable<DiagramConsistantInterval> GetContainingIntervals(
        List<DiagramConsistantInterval> intervals,
        Length location
    )
    {
        for (int i = 0; i < intervals.Count; i++)
        {
            var interval = intervals[i];
            if (location < interval.StartLocation || location > interval.EndLocation)
            {
                continue;
            }

            yield return interval;
            if (i < intervals.Count - 1 && location.Equals(interval.EndLocation, EqualityTolerance))
            {
                yield return intervals[i + 1];
            }
            yield break;
        }
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
        IEnumerable<DiagramPointValue>? pointValues = null,
        IEnumerable<DiagramDistributedValue>? additionalDistributedValues = null,
        List<DiagramPointValueAsLength>? boundaryConditions = null
    )
    {
        pointValues ??=  [];
        additionalDistributedValues ??=  [];

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
            this.ElementLength,
            this.LengthUnit,
            ratioToLengthDict,
            boundaryConditions
        );

        return new Diagram(this.ElementLength, ratioToLengthDict, intervals, this.LengthUnit);
    }

    public DiagramBuilder Integrate2()
    {
        return new DiagramBuilder(
            this.ElementLength,
            EqualityTolerance,
            this.LengthUnit,
            this.Intervals
        );
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
