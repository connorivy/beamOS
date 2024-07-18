using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.Domain.Diagrams.DiagramBaseAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;

public class ShearForceDiagram : DiagramBase<ShearForceDiagramId>
{
    public Element1DId Element1DId { get; private set; }
    public LinearCoordinateDirection3D ShearDirection { get; private set; }
    public ForceUnit ForceUnit { get; }

    protected ShearForceDiagram(
        Element1DId element1DId,
        LinearCoordinateDirection3D shearDirection,
        Length elementLength,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        List<DiagramConsistantInterval> intervals,
        ShearForceDiagramId? id = null
    )
        : base(elementLength, lengthUnit, intervals, id ?? new())
    {
        this.Element1DId = element1DId;
        this.ShearDirection = shearDirection;
        this.ForceUnit = forceUnit;
    }

    //public static ShearForceDiagram Create(
    //    Element1D element1d,
    //    LengthUnit lengthUnit,
    //    ForceUnit forceUnit,
    //    TorqueUnit torqueUnit,
    //    LinearCoordinateDirection3D localShearDirection
    //)
    //{
    //    var initialDirection = localShearDirection switch
    //    {
    //        LinearCoordinateDirection3D.AlongX => new Vector3D(1, 0, 0),
    //        LinearCoordinateDirection3D.AlongY => new Vector3D(0, 1, 0),
    //        LinearCoordinateDirection3D.AlongZ => new Vector3D(0, 0, 1),
    //        LinearCoordinateDirection3D.Undefined => throw new NotImplementedException(),
    //    };

    //    var origin = new Point3D(
    //        element1d.StartNode.LocationPoint.XCoordinate.As(lengthUnit),
    //        element1d.StartNode.LocationPoint.YCoordinate.As(lengthUnit),
    //        element1d.StartNode.LocationPoint.ZCoordinate.As(lengthUnit)
    //    );
    //    var newCoordSys = element1d.GetRotationMatrix();
    //    var coordinateSys = new CoordinateSystem(
    //        origin,
    //        UnitVector3D.Create(newCoordSys[0, 0], newCoordSys[0, 1], newCoordSys[0, 2]),
    //        UnitVector3D.Create(newCoordSys[1, 0], newCoordSys[1, 1], newCoordSys[1, 2]),
    //        UnitVector3D.Create(newCoordSys[2, 0], newCoordSys[2, 1], newCoordSys[2, 2])
    //    );
    //    var shearDirection = initialDirection.TransformBy(coordinateSys);
    //    // shearDirection
    //    // elementDirection
    //    Vector3D elementDirection = element1d.BaseLine.GetDirection(lengthUnit);
    //    double[,] rotationMatrix = element1d.GetRotationMatrix();

    //    List<DiagramPointValue> pointValues = [];
    //    List<DiagramPointValue> boundaryConditions = [];

    //    //foreach (var pointLoad in element1d.StartNode.PointLoads)
    //    //{
    //    //    Force forceDueToLoad = pointLoad.GetForceInDirection(shearDirection);
    //    //    pointValues.Add(new DiagramPointValue(Length.Zero, forceDueToLoad.As(forceUnit)));
    //    //}
    //    var startReaction = element1d
    //        .StartNode
    //        .NodeResult
    //        .Forces
    //        .GetForceInDirection(shearDirection);
    //    pointValues.Add(new DiagramPointValue(Length.Zero, startReaction.As(forceUnit)));

    //    if (element1d.StartNode.Restraint.IsFullyRestrainedInDirection(shearDirection))
    //    {
    //        boundaryConditions.Add(new DiagramPointValue(Length.Zero, 0));
    //    }

    //    //foreach (var pointLoad in element1d.EndNode.PointLoads)
    //    //{
    //    //    Force forceDueToLoad = pointLoad.GetForceInDirection(shearDirection);
    //    //    pointValues.Add(new DiagramPointValue(element1d.Length, forceDueToLoad.As(forceUnit)));
    //    //}
    //    var endReaction = element1d.EndNode.NodeResult.Forces.GetForceInDirection(shearDirection);
    //    pointValues.Add(new DiagramPointValue(element1d.Length, endReaction.As(forceUnit)));

    //    if (element1d.EndNode.Restraint.IsFullyRestrainedInDirection(shearDirection))
    //    {
    //        boundaryConditions.Add(new DiagramPointValue(element1d.Length, 0));
    //    }

    //    //List<DiagramDistributedValue> distributedValues = [];
    //    //foreach (var momLoad in element1d.StartNode.MomentLoads)
    //    //{
    //    //    var forceDueToLoad = momLoad.GetForceAboutAxis(shearDirection);
    //    //    pointValues.Add(new DiagramPointValue(Length.Zero, forceDueToLoad.As(torqueUnit)));
    //    //}
    //    //foreach (var momLoad in element1d.EndNode.MomentLoads)
    //    //{
    //    //    var forceDueToLoad = momLoad.GetForceAboutAxis(shearDirection);
    //    //    pointValues.Add(new DiagramPointValue(element1d.Length, forceDueToLoad.As(torqueUnit)));
    //    //}

    //    var db = new DiagramBuilder(
    //        element1d.Length,
    //        new Length(1, LengthUnit.Inch),
    //        lengthUnit,
    //        pointValues,
    //        [],
    //        [],
    //        [],
    //        0
    //    ).Build();

    //    return new(
    //        element1d.Id,
    //        shearDirection,
    //        element1d.Length,
    //        lengthUnit,
    //        forceUnit,
    //        db.Intervals
    //    );
    //}

    public static ShearForceDiagram Create(
        Element1DId element1d,
        Length elementLength,
        Vector<double> localMemberForces,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        TorqueUnit torqueUnit,
        LinearCoordinateDirection3D localShearDirection
    )
    {
        Forces startForces =
            new(
                new(localMemberForces[0], forceUnit),
                new(localMemberForces[1], forceUnit),
                new(localMemberForces[2], forceUnit),
                Torque.Zero,
                Torque.Zero,
                Torque.Zero
            );
        Forces endForces =
            new(
                new(localMemberForces[6], forceUnit),
                new(localMemberForces[7], forceUnit),
                new(localMemberForces[8], forceUnit),
                Torque.Zero,
                Torque.Zero,
                Torque.Zero
            );

        var shearDirection = localShearDirection switch
        {
            LinearCoordinateDirection3D.AlongX => new Vector3D(1, 0, 0),
            LinearCoordinateDirection3D.AlongY => new Vector3D(0, 1, 0),
            LinearCoordinateDirection3D.AlongZ => new Vector3D(0, 0, 1),
            LinearCoordinateDirection3D.Undefined => throw new NotImplementedException(),
        };

        //var origin = new Point3D(
        //    element1d.StartPoint.XCoordinate.As(lengthUnit),
        //    element1d.StartPoint.YCoordinate.As(lengthUnit),
        //    element1d.StartPoint.ZCoordinate.As(lengthUnit)
        //);
        //var newCoordSys = element1d.GetRotationMatrix();
        //var coordinateSys = new CoordinateSystem(
        //    origin,
        //    UnitVector3D.Create(newCoordSys[0, 0], newCoordSys[0, 1], newCoordSys[0, 2]),
        //    UnitVector3D.Create(newCoordSys[1, 0], newCoordSys[1, 1], newCoordSys[1, 2]),
        //    UnitVector3D.Create(newCoordSys[2, 0], newCoordSys[2, 1], newCoordSys[2, 2])
        //);
        //var shearDirection = initialDirection.TransformBy(coordinateSys);
        //// shearDirection
        //// elementDirection
        //Vector3D elementDirection = element1d.BaseLine.GetDirection(lengthUnit);
        ////double[,] rotationMatrix = element1d.GetRotationMatrix();

        List<DiagramPointValue> pointValues = [];
        List<DiagramPointValue> boundaryConditions = [];

        ////foreach (var pointLoad in element1d.StartNode.PointLoads)
        ////{
        ////    Force forceDueToLoad = pointLoad.GetForceInDirection(shearDirection);
        ////    pointValues.Add(new DiagramPointValue(Length.Zero, forceDueToLoad.As(forceUnit)));
        ////}
        //for (var i = 0; i < 3; i++)
        //{
        //var x = element1d
        //    .GetLocalMemberEndForcesVector()

        //}


        var startReaction = startForces.GetForceInDirection(shearDirection);
        pointValues.Add(new DiagramPointValue(Length.Zero, startReaction.As(forceUnit)));

        //if (element1d.StartNode.Restraint.IsFullyRestrainedInDirection(shearDirection))
        //{
        //    boundaryConditions.Add(new DiagramPointValue(Length.Zero, 0));
        //}

        //foreach (var pointLoad in element1d.EndNode.PointLoads)
        //{
        //    Force forceDueToLoad = pointLoad.GetForceInDirection(shearDirection);
        //    pointValues.Add(new DiagramPointValue(element1d.Length, forceDueToLoad.As(forceUnit)));
        //}
        var endReaction = endForces.GetForceInDirection(shearDirection);
        pointValues.Add(new DiagramPointValue(elementLength, endReaction.As(forceUnit)));

        //if (element1d.EndNode.Restraint.IsFullyRestrainedInDirection(shearDirection))
        //{
        //    boundaryConditions.Add(new DiagramPointValue(element1d.Length, 0));
        //}

        //List<DiagramDistributedValue> distributedValues = [];
        //foreach (var momLoad in element1d.StartNode.MomentLoads)
        //{
        //    var forceDueToLoad = momLoad.GetForceAboutAxis(shearDirection);
        //    pointValues.Add(new DiagramPointValue(Length.Zero, forceDueToLoad.As(torqueUnit)));
        //}
        //foreach (var momLoad in element1d.EndNode.MomentLoads)
        //{
        //    var forceDueToLoad = momLoad.GetForceAboutAxis(shearDirection);
        //    pointValues.Add(new DiagramPointValue(element1d.Length, forceDueToLoad.As(torqueUnit)));
        //}

        var db = new DiagramBuilder(
            elementLength,
            new Length(1, LengthUnit.Inch),
            lengthUnit,
            pointValues,
            [],
            [],
            [],
            0
        ).Build();

        return new(
            element1d,
            localShearDirection,
            elementLength,
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

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ShearForceDiagram()
        : base(Length.Zero, LengthUnit.Meter, null, null) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
