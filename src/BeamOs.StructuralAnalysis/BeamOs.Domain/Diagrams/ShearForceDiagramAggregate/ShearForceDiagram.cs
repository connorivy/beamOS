using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.Domain.Diagrams.DiagramBaseAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;

public class ShearForceDiagram : DiagramBase<ShearForceDiagramId>
{
    protected ShearForceDiagram(
        Length elementLength,
        LengthUnit lengthUnit,
        List<DiagramConsistantInterval> intervals,
        ShearForceDiagramId? id = null
    )
        : base(elementLength, lengthUnit, intervals, id ?? new()) { }

    public static ShearForceDiagram Create(
        Length elementLength,
        Vector<double> direction,
        UnitSettings unitSettings,
        List<(Length length, PointLoadData pointLoad)> pointLoads
    )
    {
        List<DiagramPointValue> pointValues = [];
        foreach (var (length, pointLoad) in pointLoads)
        {
            Force forceDueToLoad = pointLoad.GetForceInDirection(direction);
            pointValues.Add(
                new DiagramPointValue(length, forceDueToLoad.As(unitSettings.ForceUnit))
            );
        }

        var db = new DiagramBuilder(
            elementLength,
            new Length(1, LengthUnit.Inch),
            unitSettings.LengthUnit,
            pointValues,
            [],
            [],
            [],
            0
        ).Build();

        return new(elementLength, unitSettings.LengthUnit, db.Intervals);
    }

    //public static ShearForceDiagram Create(
    //    CoordinateSystemDirection3D direction,
    //    UnitSettings unitSettings,
    //    Element1dData element1dData,
    //    List<(Length length, PointLoadData pointLoad)> pointLoads
    //)
    //{
    //    List<DiagramPointValue> pointValues = [];
    //    foreach (var (length, pointLoad) in pointLoads)
    //    {
    //        Force forceDueToLoad = pointLoad.GetForceInDirection(direction);
    //        pointValues.Add(
    //            new DiagramPointValue(length, forceDueToLoad.As(unitSettings.ForceUnit))
    //        );
    //    }

    //    var db = new DiagramBuilder(
    //        element,
    //        new Length(1, LengthUnit.Inch),
    //        unitSettings.LengthUnit,
    //        pointValues,
    //        [],
    //        [],
    //        [],
    //        0
    //    ).Build();

    //    return new(elementLength, unitSettings.LengthUnit, db.Intervals);
    //}

    public MomentDiagram CreateMomentDiagram(List<(Length length, MomentLoad load)> moments)
    {
        List<DiagramPointValue> pointValues = [];
        foreach (var (length, load) in moments)
        {
            //Torque torqueDueToLoad = load.GetTorqueInDirection(direction);
            //pointValues.Add(
            //    new DiagramPointValue(length, forceDueToLoad.As(unitSettings.ForceUnit))
            //);
        }

        var builder = this.Integrate().Build();
        return new(this.ElementLength, this.LengthUnit, builder.Intervals);
    }

    //    private readonly Length elementLength;

    //    public ShearForceDiagramAggregate(
    //        Dictionary<double, ImmutablePointLoad> pointLoadsMap,
    //        ShearForceDiagramId? id = null
    //    )
    //        : base(id ?? new())
    //    {
    //        this.PointLoadsMap = pointLoadsMap;
    //    }

    //    public Dictionary<double, ImmutablePointLoad> PointLoadsMap { get; private set; }
    //    public List<DiagramConsistantInterval> Intervals { get; private set; } = [new(0, 1, new())];

    //public void Build(CoordinateSystemDirection3D direction, UnitSettings unitSettings)
    //{
    //    List<double> sortedLocationKeys = this.PointLoadsMap.Keys.OrderBy(x => x).ToList();

    //    foreach (var locationKey in this.PointLoadsMap.Keys.OrderBy(key => key))
    //    {
    //        var plAtZero = this.PointLoadsMap[locationKey];
    //        Force forceDueToLoad = plAtZero.GetForceInDirection(direction);
    //        Polynomial shearValue = new(forceDueToLoad.As(unitSettings.ForceUnit));

    //        this.AddPointLoad(locationKey, shearValue);
    //    }
    //}

    //    private void AddPointLoad(double startLocation, Polynomial polynomialValue)
    //    {
    //        int intervalIndex = this.GetIntervalIndexThatContainsLocation(startLocation);
    //        var currentInterval = this.Intervals[intervalIndex];

    //        if (currentInterval.StartLocation == startLocation)
    //        {
    //            // current interval is already split at the correct location, just add the equation
    //            AddEquationToIntervals(polynomialValue, this.Intervals.Skip((intervalIndex + 1) - 1));
    //        }
    //        else
    //        {
    //            DiagramConsistantInterval newInterval =
    //                new(
    //                    startLocation,
    //                    currentInterval.EndLocation,
    //                    currentInterval.PolynomialDescription + polynomialValue
    //                );
    //            currentInterval.EndLocation = startLocation;
    //            this.Intervals.Insert(intervalIndex + 1, newInterval);
    //            AddEquationToIntervals(polynomialValue, this.Intervals.Skip((intervalIndex + 1) + 1));
    //        }
    //    }

    //    private int GetIntervalIndexThatContainsLocation(double location)
    //    {
    //        for (var i = 0; i < this.Intervals.Count; i++)
    //        {
    //            var currentInterval = this.Intervals[i];

    //            if (currentInterval.StartLocation <= location && currentInterval.EndLocation > location)
    //            {
    //                return i;
    //            }
    //        }

    //        if (location == 1.0)
    //        {
    //            var currentLastInterval = this.Intervals.Last();
    //            this.Intervals.Add(new(1.0, 1.0, currentLastInterval.PolynomialDescription));
    //            return this.Intervals.Count - 1;
    //        }

    //        throw new Exception($"Could not find interval that contains location {location}");
    //    }

    //    private static void AddEquationToIntervals(
    //        Polynomial polynomialValue,
    //        IEnumerable<DiagramConsistantInterval> intervals
    //    )
    //    {
    //        foreach (var interval in intervals)
    //        {
    //            interval.PolynomialDescription += polynomialValue;
    //        }
    //    }

    //    public void Integrate(double valueAtZero)
    //    {
    //        double previousEndValue = valueAtZero;
    //        foreach (var interval in this.Intervals)
    //        {
    //            Polynomial integrated = interval.PolynomialDescription.Integrate();
    //            double yIntercept = (previousEndValue - integrated).Evaluate(interval.StartLocation);

    //            double boundedIntegral =
    //                integrated.Evaluate(interval.EndLocation)
    //                - integrated.Evaluate(interval.StartLocation);
    //            previousEndValue += boundedIntegral;
    //        }
    //    }
}
