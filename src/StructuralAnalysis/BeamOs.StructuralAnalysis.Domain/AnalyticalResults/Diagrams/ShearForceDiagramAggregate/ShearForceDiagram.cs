using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.DiagramBaseAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;

public class ShearForceDiagram : DiagramBase<ShearForceDiagramId, ShearDiagramConsistentInterval>
{
    public Element1d? Element1d { get; private set; }
    public Element1dId Element1dId { get; private set; }
    public LinearCoordinateDirection3D ShearDirection { get; private set; }
    public Vector3D GlobalShearDirection { get; private init; }
    public ForceUnit ForceUnit { get; }

    protected ShearForceDiagram(
        ModelId modelId,
        ResultSetId resultSetId,
        Element1dId element1DId,
        LinearCoordinateDirection3D shearDirection,
        Length elementLength,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        List<ShearDiagramConsistentInterval> intervals,
        ShearForceDiagramId? id = null
    )
        : base(modelId, resultSetId, elementLength, lengthUnit, intervals, id ?? new())
    {
        this.Element1dId = element1DId;
        this.ShearDirection = shearDirection;
        this.ForceUnit = forceUnit;
    }

    public static ShearForceDiagram Create(
        ModelId modelId,
        ResultSetId modelResultId,
        Element1dId element1d,
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
            startPoint.X.As(lengthUnit),
            startPoint.Y.As(lengthUnit),
            startPoint.Z.As(lengthUnit)
        );
        var newCoordSys = Element1d.GetRotationMatrix(endPoint, startPoint, rotation);
        var coordinateSys = new CoordinateSystem(
            origin,
            UnitVector3D.Create(newCoordSys[0, 0], newCoordSys[0, 1], newCoordSys[0, 2]),
            UnitVector3D.Create(newCoordSys[1, 0], newCoordSys[1, 1], newCoordSys[1, 2]),
            UnitVector3D.Create(newCoordSys[2, 0], newCoordSys[2, 1], newCoordSys[2, 2])
        );
        var globalShearDirection = localAxisDirection.TransformBy(coordinateSys);

        ShearForceDiagramId diagramId = new();
        return new(
            modelId,
            modelResultId,
            element1d,
            localShearDirection,
            elementLength,
            lengthUnit,
            forceUnit,
            db.Intervals.Select(i => new ShearDiagramConsistentInterval(diagramId, i)).ToList(),
            diagramId
        )
        {
            GlobalShearDirection = globalShearDirection
        };
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ShearForceDiagram()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
