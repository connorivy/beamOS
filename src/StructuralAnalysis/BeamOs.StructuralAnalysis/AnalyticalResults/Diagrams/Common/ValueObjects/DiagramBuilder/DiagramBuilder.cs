using System.Security.Cryptography.X509Certificates;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.Extensions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects.DiagramBuilder;

internal class DiagramBuilder
{
    private readonly Length elementLength;
    private readonly Length equalityTolerance;
    private readonly LengthUnit lengthUnit;
    private readonly List<DiagramPointValue> pointValues = [];
    private readonly List<DiagramDistributedValue> distributedValues = [];
    private readonly List<DiagramPointValue> boundaryConditions = [];
    private readonly List<DiagramConsistentInterval> previousDiagramIntervals = [];
    private int numTimesToIntegrate;

    private readonly double equalityToleranceAsDouble;

    public DiagramBuilder(
        Length elementLength,
        Length equalityTolerance,
        LengthUnit lengthUnit,
        List<DiagramConsistentInterval>? previousDiagramIntervals = null
    )
    {
        this.elementLength = elementLength;
        this.equalityTolerance = equalityTolerance;
        this.equalityToleranceAsDouble = equalityTolerance.As(lengthUnit);
        this.lengthUnit = lengthUnit;
        this.previousDiagramIntervals = previousDiagramIntervals ?? [];
    }

    public DiagramBuilder(
        Length elementLength,
        Length equalityTolerance,
        LengthUnit lengthUnit,
        List<DiagramPointValue> pointValues,
        List<DiagramDistributedValue> distributedValues,
        List<DiagramPointValue> boundaryConditions,
        List<DiagramConsistentInterval> previousDiagramIntervals,
        int numTimesToIntegrate
    )
    {
        this.elementLength = elementLength;
        this.equalityTolerance = equalityTolerance;
        this.lengthUnit = lengthUnit;
        this.pointValues = pointValues;
        this.distributedValues = distributedValues;
        this.boundaryConditions = boundaryConditions;
        this.previousDiagramIntervals = previousDiagramIntervals;
        this.numTimesToIntegrate = numTimesToIntegrate;
    }

    public Diagram Build()
    {
        var intervals = this.BuildConsistentIntervals();

        return new Diagram(this.elementLength, this.lengthUnit, intervals);
    }

    private List<DiagramConsistentInterval> BuildConsistentIntervals()
    {
        //List<DiagramConsistentInterval> intervals =
        //[
        //    new(new Length(0, this.lengthUnit), this.elementLength, new Polynomial(0.0))
        //];

        var orderedLengths = this.pointValues
            .Select(pv => pv.Location)
            .Concat(this.distributedValues.Select(dv => dv.StartLocation))
            .Concat(this.distributedValues.Select(dv => dv.EndLocation))
            .Concat(this.previousDiagramIntervals.Select(i => i.StartLocation))
            .Concat(this.previousDiagramIntervals.Select(i => i.EndLocation))
            .Distinct()
            .OrderBy(l => l.As(this.lengthUnit))
            .ToList();

        orderedLengths.Insert(0, new Length(0, this.lengthUnit));
        orderedLengths.Add(this.elementLength);

        Length previousLength = orderedLengths[0];
        List<DiagramConsistentInterval> intervals = [];
        for (int i = 1; i < orderedLengths.Count; i++)
        {
            var currentPoint = orderedLengths[i];
            intervals.Add(
                new DiagramConsistentInterval(previousLength, currentPoint, Polynomial.Zero)
            );
            previousLength = currentPoint;
        }

        //foreach (var point in orderedLengths)
        //{
        //    InsertPointInIntervals(intervals, point);
        //}

        foreach (var pointValue in this.pointValues)
        {
            this.AddPointValue(intervals, pointValue.Location, pointValue.Value);
        }

        foreach (var distributedValue in this.distributedValues)
        {
            this.AddDistributedValue(
                intervals,
                distributedValue.StartLocation,
                distributedValue.EndLocation,
                distributedValue.Polynomial
            );
        }

        foreach (var distributedValue in this.previousDiagramIntervals)
        {
            if (distributedValue.Polynomial == Polynomial.Zero)
            {
                continue;
            }

            this.AddDistributedValue(
                intervals,
                distributedValue.StartLocation,
                distributedValue.EndLocation,
                distributedValue.Polynomial
            );
        }

        this.ApplyBoundaryConditions(intervals);

        return intervals;
    }

    private static void InsertPointInIntervals(
        List<DiagramConsistentInterval> intervals,
        Length locationAlongBeam
    )
    {
        for (var i = 0; i < intervals.Count; i++)
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
                    new(locationAlongBeam, interval.EndLocation, interval.Polynomial)
                );
                interval.EndLocation = locationAlongBeam;
            }
            return;
        }
    }

    private void AddPointValue(
        List<DiagramConsistentInterval> intervals,
        Length startLocation,
        double value
    )
    {
        var intervalIndex = GetIndexOfIntervalWithStartLocation(intervals, startLocation);

        foreach (var interval in intervals.Skip(intervalIndex))
        {
            interval.Polynomial += value;
        }
    }

    private int GetIndexOfIntervalWithStartLocation(
        List<DiagramConsistentInterval> intervals,
        Length location
    )
    {
        var intervalIndex = intervals.FindIndex(
            i => i.StartLocation.Equals(location, this.equalityTolerance)
        );

        if (intervalIndex < 0)
        {
            throw new Exception($"Could not find interval with start location {location}");
        }

        if (
            location < this.equalityTolerance
            && intervals[intervalIndex].Length < this.equalityTolerance
        )
        {
            intervalIndex++;
        }

        return intervalIndex;
    }

    private void AddDistributedValue(
        List<DiagramConsistentInterval> intervals,
        Length startLocation,
        Length endLocation,
        Polynomial polynomialValue
    )
    {
        var startIntervalIndex = GetIndexOfIntervalWithStartLocation(intervals, startLocation);
        var endIntervalIndex = GetIndexOfIntervalWithStartLocation(intervals, endLocation);

        var integrated = polynomialValue.Integrate();
        double boundedIntegral = 0;

        var indexDiff = endIntervalIndex - startIntervalIndex;
        foreach (var interval in intervals.Skip(startIntervalIndex).Take(indexDiff))
        {
            var integralAtStart = integrated.Evaluate(interval.StartLocation.As(this.lengthUnit));
            var integralAtEnd = integrated.Evaluate(interval.EndLocation.As(this.lengthUnit));

            interval.Polynomial += integrated + new Polynomial(boundedIntegral - integralAtStart);
            boundedIntegral += integralAtEnd - integralAtStart;
        }

        foreach (var interval in intervals.Skip(startIntervalIndex + indexDiff))
        {
            interval.Polynomial += boundedIntegral;
        }
    }

    private void ApplyBoundaryConditions(List<DiagramConsistentInterval> intervals)
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

        if (this.boundaryConditions.Count < this.numTimesToIntegrate)
        {
            throw new Exception(
                "Number of provided boundary conditions must match number of times being integrated"
            );
        }

        List<DiagramPointValue> bcsToSolveFor = this.boundaryConditions
            .Take(this.numTimesToIntegrate)
            .ToList();
        IEnumerable<double> integrationConstants = this.SolveForIntegrationConstants(
            intervals,
            bcsToSolveFor
        );
        Polynomial polynomialToAdd = new(integrationConstants.Reverse());

        foreach (var interval in intervals)
        {
            interval.Polynomial += polynomialToAdd;
        }
        this.CheckRedundantBoundaryConditions(
            intervals,
            this.boundaryConditions.Skip(this.numTimesToIntegrate)
        );
    }

    private double[] SolveForIntegrationConstants(
        List<DiagramConsistentInterval> intervals,
        List<DiagramPointValue> boundaryConditions
    )
    {
        if (this.numTimesToIntegrate == 0)
        {
            return [0];
        }

        var matrixArray = new double[this.numTimesToIntegrate, this.numTimesToIntegrate];
        var results = new double[this.numTimesToIntegrate];
        for (var bcIndex = 0; bcIndex < this.numTimesToIntegrate; bcIndex++)
        {
            var bc = boundaryConditions[bcIndex];
            var currentVal = this.EvaluateIntervalsAtLocationAndThrowIfDifferent(
                intervals,
                bc.Location
            );
            for (var i = 0; i < this.numTimesToIntegrate; i++)
            {
                matrixArray[bcIndex, this.numTimesToIntegrate - (i + 1)] = Math.Pow(
                    bc.Location.As(this.lengthUnit),
                    i
                );
            }
            results[bcIndex] = bc.Value - currentVal;
        }

        var a = Matrix<double>.Build.DenseOfArray(matrixArray);
        var b = Vector<double>.Build.Dense(results);
        return a.Solve(b).ToArray();
    }

    private void CheckRedundantBoundaryConditions(
        List<DiagramConsistentInterval> intervals,
        IEnumerable<DiagramPointValue> boundaryConditions
    )
    {
        foreach (var bc in boundaryConditions)
        {
            var currentVal = this.EvaluateIntervalsAtLocationAndThrowIfDifferent(
                intervals,
                bc.Location
            );

            if (Math.Abs(bc.Value - currentVal) > this.equalityToleranceAsDouble)
            {
                throw new Exception("Unable to apply all boundary conditions");
            }
        }
    }

    private double EvaluateIntervalsAtLocationAndThrowIfDifferent(
        List<DiagramConsistentInterval> intervals,
        Length location
    )
    {
        var containingIntervals = this.GetContainingIntervals(intervals, location).ToList();
        var valInFirstInterval = containingIntervals[0]
            .Polynomial
            .SafeEvaluate(location.As(this.lengthUnit));

        if (containingIntervals.Count > 1)
        {
            if (containingIntervals.Count > 2)
            {
                throw new Exception("How did this happen?");
            }

            var valInLastInterval = containingIntervals[1]
                .Polynomial
                .SafeEvaluate(location.As(this.lengthUnit));
            if (
                Math.Abs(valInFirstInterval - valInLastInterval)
                > this.equalityTolerance.As(this.lengthUnit)
            )
            {
                throw new Exception(
                    "Cannot apply boundary condition at point with two different values"
                );
            }
        }

        return valInFirstInterval;
    }

    private IEnumerable<DiagramConsistentInterval> GetContainingIntervals(
        List<DiagramConsistentInterval> intervals,
        Length location
    )
    {
        for (var i = 0; i < intervals.Count; i++)
        {
            var interval = intervals[i];
            if (location < interval.StartLocation || location > interval.EndLocation)
            {
                continue;
            }

            yield return interval;
            if (
                i < intervals.Count - 1
                && location.Equals(interval.EndLocation, this.equalityTolerance)
            )
            {
                yield return intervals[i + 1];
            }
            yield break;
        }
    }

    #region Builder Methods
    public DiagramBuilder AddPointLoads(params DiagramPointValue[] diagramPointValues)
    {
        this.pointValues.AddRange(diagramPointValues);
        return this;
    }

    public DiagramBuilder AddDistributedLoads(
        params DiagramDistributedValue[] diagramDistributedValues
    )
    {
        this.distributedValues.AddRange(diagramDistributedValues);
        return this;
    }

    public DiagramBuilder ApplyIntegrationBoundaryConditions(
        int numTimesIntegrated,
        params DiagramPointValue[] boundaryConditions
    )
    {
        if (boundaryConditions.Length < numTimesIntegrated)
        {
            throw new Exception(
                $"Number of provided boundary conditions, {boundaryConditions.Length}, is less than the numTimesToIntegrate, {numTimesIntegrated}"
            );
        }

        this.numTimesToIntegrate = numTimesIntegrated;
        this.boundaryConditions.AddRange(boundaryConditions);

        return this;
    }
    #endregion
}

internal class DiagramPointValue
{
    public DiagramPointValue(Length location, double value)
    {
        this.Location = location;
        this.Value = value;
    }

    public Length Location { get; set; }
    public double Value { get; set; }
}

internal class DiagramDistributedValue
{
    public DiagramDistributedValue(Length startLocation, Length endLocation, Polynomial polynomial)
    {
        this.StartLocation = startLocation;
        this.EndLocation = endLocation;
        this.Polynomial = polynomial;
    }

    public DiagramDistributedValue(
        double startLocation,
        double endLocation,
        LengthUnit lengthUnit,
        Polynomial polynomial
    )
        : this(
            new Length(startLocation, lengthUnit),
            new Length(endLocation, lengthUnit),
            polynomial
        ) { }

    public Length StartLocation { get; set; }
    public Length EndLocation { get; set; }
    public Polynomial Polynomial { get; set; }
}
