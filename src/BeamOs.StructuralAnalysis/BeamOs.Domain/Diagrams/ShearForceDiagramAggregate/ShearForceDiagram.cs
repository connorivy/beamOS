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
    public Vector3D GlobalShearDirection { get; private init; }
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

    public static ShearForceDiagram Create(
        Element1DId element1d,
        Point startPoint,
        Point endPoint,
        Angle rotation,
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

        var localAxisDirection = localShearDirection switch
        {
            LinearCoordinateDirection3D.AlongX => new Vector3D(1, 0, 0),
            LinearCoordinateDirection3D.AlongY => new Vector3D(0, 1, 0),
            LinearCoordinateDirection3D.AlongZ => new Vector3D(0, 0, 1),
            LinearCoordinateDirection3D.Undefined => throw new NotImplementedException(),
        };

        List<DiagramPointValue> pointValues = [];
        List<DiagramPointValue> boundaryConditions = [];

        var startReaction = startForces.GetForceInDirection(localAxisDirection);
        pointValues.Add(new DiagramPointValue(Length.Zero, startReaction.As(forceUnit)));

        var endReaction = endForces.GetForceInDirection(localAxisDirection);
        pointValues.Add(new DiagramPointValue(elementLength, endReaction.As(forceUnit)));

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

        var origin = new Point3D(
            startPoint.XCoordinate.As(lengthUnit),
            startPoint.YCoordinate.As(lengthUnit),
            startPoint.ZCoordinate.As(lengthUnit)
        );
        var newCoordSys = Element1D.GetRotationMatrix(endPoint, startPoint, rotation);
        var coordinateSys = new CoordinateSystem(
            origin,
            UnitVector3D.Create(newCoordSys[0, 0], newCoordSys[0, 1], newCoordSys[0, 2]),
            UnitVector3D.Create(newCoordSys[1, 0], newCoordSys[1, 1], newCoordSys[1, 2]),
            UnitVector3D.Create(newCoordSys[2, 0], newCoordSys[2, 1], newCoordSys[2, 2])
        );
        var globalShearDirection = localAxisDirection.TransformBy(coordinateSys);

        return new(
            element1d,
            localShearDirection,
            elementLength,
            lengthUnit,
            forceUnit,
            db.Intervals
        )
        {
            GlobalShearDirection = globalShearDirection
        };
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ShearForceDiagram()
        : base(Length.Zero, LengthUnit.Meter, null, null) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
