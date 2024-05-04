using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.Domain.Diagrams.DiagramBaseAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;

public class ShearForceDiagram : DiagramBase<ShearForceDiagramId>
{
    public Element1DId Element1DId { get; private set; }
    public Vector3D ElementDirection { get; private set; }
    public Vector3D ShearDirection { get; private set; }
    public ForceUnit ForceUnit { get; }

    protected ShearForceDiagram(
        Element1DId element1DId,
        Vector3D elementDirection,
        Vector3D shearDirection,
        Length elementLength,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        List<DiagramConsistantInterval> intervals,
        ShearForceDiagramId? id = null
    )
        : base(elementLength, lengthUnit, intervals, id ?? new())
    {
        this.Element1DId = element1DId;
        this.ElementDirection = elementDirection;
        this.ShearDirection = shearDirection;
        this.ForceUnit = forceUnit;
    }

    public static ShearForceDiagram Create(
        Element1DId element1DId,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        LinearCoordinateDirection3D localShearDirection,
        Vector3D shearDirection,
        Element1dData element1DData,
        List<(Length length, PointLoadData pointLoad)> pointLoads
    )
    {
        // shearDirection
        // elementDirection
        Vector3D elementDirection = element1DData.BaseLine.GetDirection(lengthUnit);
        //UnitVector3D shearDirection = shearDirection;

        List<DiagramPointValue> pointValues = [];
        foreach (var (length, pointLoad) in pointLoads)
        {
            Force forceDueToLoad = pointLoad.GetForceInLocalAxisDirection(localShearDirection);
            pointValues.Add(new DiagramPointValue(length, forceDueToLoad.As(forceUnit)));
        }

        var db = new DiagramBuilder(
            element1DData.BaseLine.Length,
            new Length(1, LengthUnit.Inch),
            lengthUnit,
            pointValues,
            [],
            [],
            [],
            0
        ).Build();

        return new(
            element1DId,
            elementDirection,
            shearDirection,
            element1DData.BaseLine.Length,
            lengthUnit,
            forceUnit,
            db.Intervals
        );
    }

    public static ShearForceDiagram Create(
        Element1D element1D,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        LinearCoordinateDirection3D localShearDirection,
        Vector3D shearDirection,
        Element1dData element1DData,
        List<(Length length, PointLoadData pointLoad)> pointLoads
    )
    {
        // shearDirection
        // elementDirection
        Vector3D elementDirection = element1DData.BaseLine.GetDirection(lengthUnit);

        List<DiagramPointValue> pointValues = [];
        foreach (var (length, pointLoad) in pointLoads)
        {
            Force forceDueToLoad = pointLoad.GetForceInLocalAxisDirection(localShearDirection);
            pointValues.Add(new DiagramPointValue(length, forceDueToLoad.As(forceUnit)));
        }

        var db = new DiagramBuilder(
            element1DData.BaseLine.Length,
            new Length(1, LengthUnit.Inch),
            lengthUnit,
            pointValues,
            [],
            [],
            [],
            0
        ).Build();

        return new(
            element1D.Id,
            elementDirection,
            shearDirection,
            element1DData.BaseLine.Length,
            lengthUnit,
            forceUnit,
            db.Intervals
        );
    }

    //public MomentDiagram CreateMomentDiagram(
    //    List<(Length length, MomentLoadData momentLoadData)> momentLoads
    //)
    //{
    //    var aboutAxis = this.ElementDirection.CrossProduct(this.ShearDirection);
    //    List<DiagramPointValue> pointValues = [];
    //    foreach (var (length, loadData) in momentLoads)
    //    {
    //        Torque torqueDueToLoad = loadData.GetTorqueAboutAxis(aboutAxis);
    //        pointValues.Add(
    //            new DiagramPointValue(length, torqueDueToLoad.As(this.UnitSettings.TorqueUnit))
    //        );
    //    }

    //    var diagram = new DiagramBuilder(
    //        this.ElementLength,
    //        this.EqualityTolerance,
    //        this.UnitSettings.LengthUnit,
    //        pointValues,
    //        [],
    //        [],
    //        this.Intervals,
    //        0
    //    ).Build();

    //    return new(this.ElementLength, this.LengthUnit, diagram.Intervals);
    //}

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
