using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.Domain.Diagrams.DiagramBaseAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate.ValueObjects;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams.MomentDiagramAggregate;

public sealed class MomentDiagram : DiagramBase<MomentDiagramId>
{
    public Element1DId Element1DId { get; private set; }
    public LinearCoordinateDirection3D ShearDirection { get; private set; }
    public Vector3D GlobalShearDirection { get; private init; }
    public ForceUnit ForceUnit { get; }

    protected MomentDiagram(
        Element1DId element1DId,
        LinearCoordinateDirection3D shearDirection,
        Length elementLength,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        List<DiagramConsistantInterval> intervals,
        MomentDiagramId? id = null
    )
        : base(elementLength, lengthUnit, intervals, id ?? new())
    {
        this.Element1DId = element1DId;
        this.ShearDirection = shearDirection;
        this.ForceUnit = forceUnit;
    }

    public static MomentDiagram Create(
        Element1DId element1d,
        Point startPoint,
        Point endPoint,
        Angle rotation,
        Length elementLength,
        Vector<double> localMemberTorques,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        TorqueUnit torqueUnit,
        LinearCoordinateDirection3D localShearDirection,
        ShearForceDiagram shearForceDiagram
    )
    {
        Forces startForces =
            new(
                Force.Zero,
                Force.Zero,
                Force.Zero,
                new(localMemberTorques[3], torqueUnit),
                new(localMemberTorques[4], torqueUnit),
                new(localMemberTorques[5], torqueUnit)
            );

        Forces endForces =
            new(
                Force.Zero,
                Force.Zero,
                Force.Zero,
                new(localMemberTorques[9], torqueUnit),
                new(localMemberTorques[10], torqueUnit),
                new(localMemberTorques[11], torqueUnit)
            );

        var localAxisDirection = localShearDirection switch
        {
            LinearCoordinateDirection3D.AlongX => new Vector3D(1, 0, 0),
            LinearCoordinateDirection3D.AlongY => new Vector3D(0, 0, 1),
            LinearCoordinateDirection3D.AlongZ => new Vector3D(0, 0, 1),
            LinearCoordinateDirection3D.Undefined => throw new NotImplementedException(),
        };

        List<DiagramPointValue> pointValues = [];
        List<DiagramPointValue> boundaryConditions = [];

        var startReaction = startForces.GetTorqueAboutAxis(localAxisDirection);
        var startPointValue = new DiagramPointValue(Length.Zero, startReaction.As(torqueUnit));
        pointValues.Add(startPointValue);
        //boundaryConditions.Add(startPointValue);

        var endReaction = endForces.GetTorqueAboutAxis(localAxisDirection);
        var endPointValue = new DiagramPointValue(elementLength, endReaction.As(torqueUnit));
        pointValues.Add(endPointValue);
        //boundaryConditions.Add(endPointValue);

        var db = shearForceDiagram
            .Integrate()
            .AddPointLoads([.. pointValues])
            //.ApplyIntegrationBoundaryConditions(1, startPointValue, endPointValue)
            .Build();

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

        //List<DiagramPointValue> boundaryConditions = [

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
    private MomentDiagram()
        : base(Length.Zero, LengthUnit.Meter, null, null) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
