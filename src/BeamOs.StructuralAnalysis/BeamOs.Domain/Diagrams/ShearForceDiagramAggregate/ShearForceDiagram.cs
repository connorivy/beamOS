using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.Domain.Diagrams.DiagramBaseAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;

public class ShearForceDiagram : DiagramBase<ShearForceDiagramId>
{
    private readonly Vector3D elementDirection;
    private readonly Vector3D shearDirection;
    private readonly UnitSettings unitSettings;

    protected ShearForceDiagram(
        UnitSettings unitSettings,
        Vector3D elementDirection,
        Vector3D shearDirection,
        Length elementLength,
        List<DiagramConsistantInterval> intervals,
        ShearForceDiagramId? id = null
    )
        : base(elementLength, unitSettings.LengthUnit, intervals, id ?? new())
    {
        this.elementDirection = elementDirection;
        this.shearDirection = shearDirection;
        this.unitSettings = unitSettings;
    }

    public static ShearForceDiagram Create(
        UnitSettings unitSettings,
        Vector3D shearDirection,
        Element1dData element1DData,
        List<(Length length, PointLoadData pointLoad)> pointLoads
    )
    {
        // shearDirection
        // elementDirection
        var elementDirection = element1DData.BaseLine.GetDirection(unitSettings.LengthUnit);

        List<DiagramPointValue> pointValues = [];
        foreach (var (length, pointLoad) in pointLoads)
        {
            Force forceDueToLoad = pointLoad.GetForceInDirection(shearDirection);
            pointValues.Add(
                new DiagramPointValue(length, forceDueToLoad.As(unitSettings.ForceUnit))
            );
        }

        var db = new DiagramBuilder(
            element1DData.BaseLine.Length,
            new Length(1, LengthUnit.Inch),
            unitSettings.LengthUnit,
            pointValues,
            [],
            [],
            [],
            0
        ).Build();

        return new(
            unitSettings,
            elementDirection,
            shearDirection,
            element1DData.BaseLine.Length,
            db.Intervals
        );
    }

    public MomentDiagram CreateMomentDiagram(
        List<(Length length, MomentLoadData momentLoadData)> momentLoads
    )
    {
        var aboutAxis = this.elementDirection.CrossProduct(this.shearDirection);
        List<DiagramPointValue> pointValues = [];
        foreach (var (length, loadData) in momentLoads)
        {
            Torque torqueDueToLoad = loadData.GetTorqueAboutAxis(aboutAxis);
            pointValues.Add(
                new DiagramPointValue(length, torqueDueToLoad.As(this.unitSettings.TorqueUnit))
            );
        }

        var diagram = new DiagramBuilder(
            this.ElementLength,
            this.EqualityTolerance,
            this.unitSettings.LengthUnit,
            pointValues,
            [],
            [],
            this.Intervals,
            0
        ).Build();

        return new(this.ElementLength, this.LengthUnit, diagram.Intervals);
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
