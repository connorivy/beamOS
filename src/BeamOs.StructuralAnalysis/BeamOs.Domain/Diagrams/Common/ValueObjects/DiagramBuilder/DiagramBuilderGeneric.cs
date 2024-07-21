using BeamOs.Domain.Common.Models;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;



//public class DiagramBuilderGeneric<TUnit, TUnitType>
//    where TUnitType : Enum
//    where TUnit : IArithmeticQuantity<TUnit, TUnitType, double>
//{
//    private readonly Length elementLength;
//    private readonly Length equalityTolerance;
//    private readonly LengthUnit lengthUnit;
//    private readonly List<DiagramPointValue<TUnit>> pointValues = [];
//    private readonly List<DiagramDistributedValue<TUnit>> distributedValues = [];
//    private readonly List<DiagramPointValue<TUnit>> boundaryConditions = [];
//    private readonly List<DiagramConsistantInterval2> previousDiagramIntervals = [];
//    private int numTimesToIntegrate;

//    private readonly double equalityToleranceAsDouble;

//    public DiagramBuilderGeneric(
//        Length elementLength,
//        Length equalityTolerance,
//        LengthUnit lengthUnit,
//        List<DiagramConsistantInterval2>? previousDiagramIntervals = null
//    )
//    {
//        this.elementLength = elementLength;
//        this.equalityTolerance = equalityTolerance;
//        this.equalityToleranceAsDouble = equalityTolerance.As(lengthUnit);
//        this.lengthUnit = lengthUnit;
//        this.previousDiagramIntervals = previousDiagramIntervals ?? [];
//    }

//    public DiagramBuilderGeneric(
//        Length elementLength,
//        Length equalityTolerance,
//        LengthUnit lengthUnit,
//        List<DiagramPointValue<TUnit>> pointValues,
//        List<DiagramDistributedValue<TUnit>> distributedValues,
//        List<DiagramPointValue<TUnit>> boundaryConditions,
//        List<DiagramConsistantInterval2> previousDiagramIntervals,
//        int numTimesToIntegrate
//    )
//    {
//        this.elementLength = elementLength;
//        this.equalityTolerance = equalityTolerance;
//        this.lengthUnit = lengthUnit;
//        this.pointValues = pointValues;
//        this.distributedValues = distributedValues;
//        this.boundaryConditions = boundaryConditions;
//        this.previousDiagramIntervals = previousDiagramIntervals;
//        this.numTimesToIntegrate = numTimesToIntegrate;
//    }

//    public Diagram2 Build()
//    {
//        var intervals = this.BuildConsistantIntervals();

//        return new Diagram2(this.elementLength, this.lengthUnit, intervals);
//    }

//    private List<DiagramConsistantInterval2> BuildConsistantIntervals()
//    {
//        List<DiagramConsistantInterval2> intervals =
//        [
//            new(new Length(0, this.lengthUnit), this.elementLength, new BeamOsPolynomial<TUnit, TUnitType>() {
//                LengthUnit = this.lengthUnit,
//                UnitType = default,
//                Coefficients = Array.Empty<double>()
//            })
//        ];

//        var orderedLengths = this.pointValues
//            .Select(pv => pv.Location)
//            .Concat(this.distributedValues.Select(dv => dv.StartLocation))
//            .Concat(this.distributedValues.Select(dv => dv.EndLocation))
//            .Concat(this.previousDiagramIntervals.Select(i => i.StartLocation))
//            .Concat(this.previousDiagramIntervals.Select(i => i.EndLocation))
//            .Distinct()
//            .OrderBy(l => l.As(this.lengthUnit));

//        foreach (var point in orderedLengths)
//        {
//            InsertPointInIntervals(intervals, point);
//        }

//        foreach (var pointValue in this.pointValues)
//        {
//            this.AddPointValue(intervals, pointValue.Location, pointValue.Value);
//        }

//        foreach (var distributedValue in this.distributedValues)
//        {
//            this.AddDistributedValue(
//                intervals,
//                distributedValue.StartLocation,
//                distributedValue.EndLocation,
//                distributedValue.Polynomial
//            );
//        }

//        foreach (var distributedValue in this.previousDiagramIntervals)
//        {
//            this.AddDistributedValue(
//                intervals,
//                distributedValue.StartLocation,
//                distributedValue.EndLocation,
//                distributedValue.Polynomial
//            );
//        }

//        this.ApplyBoundaryConditions(intervals);

//        return intervals;
//    }

//    private static void InsertPointInIntervals(
//        List<DiagramConsistantInterval2<TUnit, TUnitType>> intervals,
//        Length locationAlongBeam
//    )
//    {
//        for (var i = 0; i < intervals.Count; i++)
//        {
//            var interval = intervals[i];
//            if (locationAlongBeam > interval.EndLocation)
//            {
//                continue;
//            }
//            else if (locationAlongBeam > interval.StartLocation)
//            {
//                intervals.Insert(
//                    i + 1,
//                    new(locationAlongBeam, interval.EndLocation, interval.Polynomial)
//                );
//                interval.EndLocation = locationAlongBeam;
//            }
//            return;
//        }
//    }

//    private void AddPointValue(
//        List<DiagramConsistantInterval2<TUnit, TUnitType>> intervals,
//        Length startLocation,
//        TUnit value
//    )
//    {
//        var intervalIndex = intervals.FindIndex(
//            i => i.StartLocation.Equals(startLocation, this.equalityTolerance)
//        );

//        if (intervalIndex < 0)
//        {
//            throw new Exception($"Could not find interval with start location {startLocation}");
//        }

//        foreach (var interval in intervals.Skip(intervalIndex))
//        {
//            interval.Polynomial += value;
//        }
//    }

//    private void AddDistributedValue(
//        List<DiagramConsistantInterval2<TUnit, TUnitType>> intervals,
//        Length startLocation,
//        Length endLocation,
//        BeamOsPolynomial<TUnit, TUnitType> polynomialValue
//    )
//    {
//        var startIntervalIndex = intervals.FindIndex(
//            i => i.StartLocation.Equals(startLocation, this.equalityTolerance)
//        );
//        var endIntervalIndex = intervals.FindIndex(
//            i => i.StartLocation.Equals(endLocation, this.equalityTolerance)
//        );

//        if (startIntervalIndex < 0 || endIntervalIndex < 0)
//        {
//            throw new Exception(
//                $"Could not find interval with start location {startLocation} or {endLocation}"
//            );
//        }

//        var integrated = polynomialValue.Integrate();
//        double boundedIntegral = 0;

//        var indexDiff = endIntervalIndex - startIntervalIndex;
//        foreach (var interval in intervals.Skip(startIntervalIndex).Take(indexDiff))
//        {
//            var integralAtStart = integrated.Evaluate(interval.StartLocation.As(this.lengthUnit));
//            var integralAtEnd = integrated.Evaluate(interval.EndLocation.As(this.lengthUnit));

//            interval.Polynomial += integrated + new Polynomial(boundedIntegral - integralAtStart);
//            boundedIntegral += integralAtEnd - integralAtStart;
//        }

//        foreach (var interval in intervals.Skip(startIntervalIndex + indexDiff))
//        {
//            interval.Polynomial += boundedIntegral;
//        }
//    }

//    private void ApplyBoundaryConditions(List<DiagramConsistantInterval> intervals)
//    {
//        // numTimesIntegrated = 1
//        // C = const
//        // | 1 | | C | = | const |

//        // numTimesIntegrated = 2
//        // C1x + C2 = const
//        // | x 1 | | C1 | = | f(x1) |
//        // | x 1 | | C2 |   | f(x2) |

//        // numTimesIntegrated = 3
//        // C1x2 + C2x + C3 = const
//        // | x2 x 1 | | C1 | = | f(x1) |
//        // | x2 x 1 | | C2 |   | f(x2) |
//        // | x2 x 1 | | C3 |   | f(x3) |

//        if (this.boundaryConditions.Count < this.numTimesToIntegrate)
//        {
//            throw new Exception(
//                "Number of provided boundary conditions must match number of times being integrated"
//            );
//        }

//        List<DiagramPointValue> bcsToSolveFor = this.boundaryConditions
//            .Take(this.numTimesToIntegrate)
//            .ToList();
//        double[] integrationConstants = this.SolveForIntegrationConstants(intervals, bcsToSolveFor);
//        Polynomial polynomialToAdd = new(integrationConstants.Reverse());

//        foreach (var interval in intervals)
//        {
//            interval.Polynomial += polynomialToAdd;
//        }
//        this.CheckRedundantBoundaryConditions(
//            intervals,
//            this.boundaryConditions.Skip(this.numTimesToIntegrate)
//        );
//    }

//    private double[] SolveForIntegrationConstants(
//        List<DiagramConsistantInterval> intervals,
//        List<DiagramPointValue> boundaryConditions
//    )
//    {
//        var matrixArray = new double[this.numTimesToIntegrate, this.numTimesToIntegrate];
//        var results = new double[this.numTimesToIntegrate];
//        for (var bcIndex = 0; bcIndex < this.numTimesToIntegrate; bcIndex++)
//        {
//            var bc = boundaryConditions[bcIndex];
//            var currentVal = this.EvaluateIntervalsAtLocationAndThrowIfDifferent(
//                intervals,
//                bc.Location
//            );
//            for (var i = 0; i < this.numTimesToIntegrate; i++)
//            {
//                matrixArray[bcIndex, this.numTimesToIntegrate - (i + 1)] = Math.Pow(
//                    bc.Location.As(this.lengthUnit),
//                    i
//                );
//            }
//            results[bcIndex] = bc.Value - currentVal;
//        }

//        var a = Matrix<double>.Build.DenseOfArray(matrixArray);
//        var b = Vector<double>.Build.Dense(results);
//        return a.Solve(b).ToArray();
//    }

//    private void CheckRedundantBoundaryConditions(
//        List<DiagramConsistantInterval> intervals,
//        IEnumerable<DiagramPointValue> boundaryConditions
//    )
//    {
//        foreach (var bc in boundaryConditions)
//        {
//            var currentVal = this.EvaluateIntervalsAtLocationAndThrowIfDifferent(
//                intervals,
//                bc.Location
//            );

//            if (Math.Abs(bc.Value - currentVal) > this.equalityToleranceAsDouble)
//            {
//                throw new Exception("Unable to apply all boundary conditions");
//            }
//        }
//    }

//    private double EvaluateIntervalsAtLocationAndThrowIfDifferent(
//        List<DiagramConsistantInterval> intervals,
//        Length location
//    )
//    {
//        var containingIntervals = this.GetContainingIntervals(intervals, location).ToList();
//        var valInFirstInterval = containingIntervals[0]
//            .Polynomial
//            .Evaluate(location.As(this.lengthUnit));
//        if (containingIntervals.Count > 1)
//        {
//            if (containingIntervals.Count > 2)
//            {
//                throw new Exception("How did this happen?");
//            }

//            var valInLastInterval = containingIntervals[1]
//                .Polynomial
//                .Evaluate(location.As(this.lengthUnit));
//            if (
//                Math.Abs(valInFirstInterval - valInLastInterval)
//                > this.equalityTolerance.As(this.lengthUnit)
//            )
//            {
//                throw new Exception(
//                    "Cannot apply boundary condition at point with two different values"
//                );
//            }
//        }

//        return valInFirstInterval;
//    }

//    private IEnumerable<DiagramConsistantInterval> GetContainingIntervals(
//        List<DiagramConsistantInterval> intervals,
//        Length location
//    )
//    {
//        for (var i = 0; i < intervals.Count; i++)
//        {
//            var interval = intervals[i];
//            if (location < interval.StartLocation || location > interval.EndLocation)
//            {
//                continue;
//            }

//            yield return interval;
//            if (
//                i < intervals.Count - 1
//                && location.Equals(interval.EndLocation, this.equalityTolerance)
//            )
//            {
//                yield return intervals[i + 1];
//            }
//            yield break;
//        }
//    }

//    #region Builder Methods
//    public DiagramBuilderGeneric<TUnit> AddPointLoads(params DiagramPointValue[] diagramPointValues)
//    {
//        this.pointValues.AddRange(diagramPointValues);
//        return this;
//    }

//    public DiagramBuilderGeneric<TUnit> AddDistributedLoads(
//        params DiagramDistributedValue[] diagramDistributedValues
//    )
//    {
//        this.distributedValues.AddRange(diagramDistributedValues);
//        return this;
//    }

//    public DiagramBuilderGeneric<TUnit> ApplyIntegrationBoundaryConditions(
//        int numTimesIntegrated,
//        params DiagramPointValue[] boundaryConditions
//    )
//    {
//        if (boundaryConditions.Length < numTimesIntegrated)
//        {
//            throw new Exception(
//                $"Number of provided boundary conditions, {boundaryConditions.Length}, is less than the numTimesToIntegrate, {numTimesIntegrated}"
//            );
//        }

//        this.numTimesToIntegrate = numTimesIntegrated;
//        this.boundaryConditions.AddRange(boundaryConditions);

//        return this;
//    }
//    #endregion
//}

//public class DiagramPointValue<TUnit>
//    where TUnit : IQuantity
//{
//    public DiagramPointValue(Length location, TUnit value)
//    {
//        this.Location = location;
//        this.Value = value;
//    }

//    public Length Location { get; set; }
//    public TUnit Value { get; set; }
//}

//public class DiagramDistributedValue<TUnit>
//    where TUnit : IQuantity
//{
//    public DiagramDistributedValue(
//        Length startLocation,
//        Length endLocation,
//        BeamOsPolynomial<TUnit> polynomial
//    )
//    {
//        this.StartLocation = startLocation;
//        this.EndLocation = endLocation;
//        this.Polynomial = polynomial;
//    }

//    public DiagramDistributedValue(
//        double startLocation,
//        double endLocation,
//        LengthUnit lengthUnit,
//        BeamOsPolynomial<TUnit> polynomial
//    )
//        : this(
//            new Length(startLocation, lengthUnit),
//            new Length(endLocation, lengthUnit),
//            polynomial
//        ) { }

//    public Length StartLocation { get; set; }
//    public Length EndLocation { get; set; }
//    public BeamOsPolynomial<TUnit> Polynomial { get; set; }
//}

//public interface IBeamOsPolynomial
//{
//    public IQuantity Evaluate(Length length);
//    //public Enum UnitType { get; }
//    //public double[] Coefficients { get; }
//}

//public record BeamOsPolynomial<TUnit, TUnitType> : IBeamOsPolynomial
//    where TUnitType : Enum
//    where TUnit : IArithmeticQuantity<TUnit, TUnitType, double>
//{
//    private Polynomial polynomial { get; init; }
//    public required LengthUnit LengthUnit { get; init; }
//    public required TUnitType UnitType { get; init; }
//    public required double[] Coefficients
//    {
//        get => this.polynomial.Coefficients;
//        init => this.polynomial = new(value);
//    }

//    public TUnit Evaluate(Length length)
//    {
//        double result = this.polynomial.Evaluate(length.As(this.LengthUnit));
//        return (TUnit)Quantity.From(result, this.UnitType);
//    }

//    IQuantity IBeamOsPolynomial.Evaluate(Length length) => this.Evaluate(length);

//    //
//    // Summary:
//    //     Addition of a polynomial and a scalar.
//    public static BeamOsPolynomial<TUnit, TUnitType> Add(
//        BeamOsPolynomial<TUnit, TUnitType> a,
//        TUnit b
//    )
//    {
//        return a with { polynomial = a.polynomial + b.As(a.UnitType) };
//    }

//    public static BeamOsPolynomial<TUnit, TUnitType> operator +(
//        BeamOsPolynomial<TUnit, TUnitType> a,
//        TUnit b
//    ) => Add(a, b);

//    public static BeamOsPolynomial<TUnit, TUnitType> Add(
//        BeamOsPolynomial<TUnit, TUnitType> a,
//        BeamOsPolynomial<TUnit, TUnitType> b
//    )
//    {
//        return a with { polynomial = a.polynomial + b.polynomial };
//    }

//    public static BeamOsPolynomial<TUnit, TUnitType> operator +(
//        BeamOsPolynomial<TUnit, TUnitType> a,
//        BeamOsPolynomial<TUnit, TUnitType> b
//    ) => Add(a, b);

//    public BeamOsPolynomial<TUnit, TUnitType> Integrate() =>
//        this with
//        {
//            polynomial = this.polynomial.Integrate()
//        };
//}

//public class ForcePerLengthPolynomial : BeamOsPolynomial<Force, ForceUnit> { }

//public sealed class DiagramConsistantInterval2<TUnit, TUnitType>
//    : BeamOSEntity<DiagramConsistantIntervalId>
//    where TUnitType : Enum
//    where TUnit : IArithmeticQuantity<TUnit, TUnitType, double>
//{
//    public DiagramConsistantInterval2(
//        Length startLocation,
//        Length endLocation,
//        BeamOsPolynomial<TUnit, TUnitType> polynomial,
//        DiagramConsistantIntervalId? id = null
//    )
//        : base(id ?? new())
//    {
//        this.StartLocation = startLocation;
//        this.EndLocation = endLocation;
//        this.Polynomial = polynomial;
//        this.LengthUnit = startLocation.Unit;
//    }

//    public Length StartLocation { get; set; }
//    public Length EndLocation { get; set; }
//    public BeamOsPolynomial<TUnit, TUnitType> Polynomial { get; set; }
//    public LengthUnit LengthUnit { get; set; }

//    //public double EvalutateAtLocation(Length location)
//    //{
//    //    if (location < this.StartLocation || location > this.EndLocation)
//    //    {
//    //        throw new Exception("Out of bounds my guy");
//    //    }

//    //    return this.Polynomial.Evaluate(location);
//    //}

//    public Length Length => this.EndLocation - this.StartLocation;

//    //public bool EqualsIntervalAtLocation(DiagramConsistantInterval other, Length location, Length EqualityTolerance)
//    //{
//    //    return this.EvalutateAtLocation()
//    //}

//    [Obsolete("EF Core Constructor", true)]
//#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//    private DiagramConsistantInterval2() { }
//#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//}
