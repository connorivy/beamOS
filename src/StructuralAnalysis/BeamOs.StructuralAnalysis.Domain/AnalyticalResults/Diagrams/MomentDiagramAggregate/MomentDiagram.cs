using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects.DiagramBuilder;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.DiagramBaseAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate;

public sealed class MomentDiagram : DiagramBase<MomentDiagramId, MomentDiagramConsistentInterval>
{
    public Element1d? Element1d { get; private set; }
    public Element1dId Element1dId { get; private set; }
    public LinearCoordinateDirection3D ShearDirection { get; private set; }
    public Vector3D GlobalShearDirection { get; private init; }
    public ForceUnit ForceUnit { get; }

    private MomentDiagram(
        ModelId modelId,
        ResultSetId resultSetId,
        Element1dId element1DId,
        LinearCoordinateDirection3D shearDirection,
        Length elementLength,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        List<MomentDiagramConsistentInterval> intervals,
        MomentDiagramId? id = null
    )
        : base(modelId, resultSetId, elementLength, lengthUnit, intervals, id ?? new())
    {
        this.Element1dId = element1DId;
        this.ShearDirection = shearDirection;
        this.ForceUnit = forceUnit;
    }

    public static MomentDiagram Create(
        ModelId modelId,
        ResultSetId modelResultId,
        Element1dId element1d,
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
        // point moments get applied with a flipped sign in bending moment diagram
        // https://youtu.be/paHXLFuqap0?si=qdxoWmB5OkQGmAwJ&t=790
        var startPointValue = new DiagramPointValue(Length.Zero, -1 * startReaction.As(torqueUnit));
        pointValues.Add(startPointValue);
        //boundaryConditions.Add(startPointValue);

        var endReaction = endForces.GetTorqueAboutAxis(localAxisDirection);
        var endPointValue = new DiagramPointValue(elementLength, -1 * endReaction.As(torqueUnit));
        pointValues.Add(endPointValue);
        //boundaryConditions.Add(endPointValue);

        var db = shearForceDiagram
            .Integrate()
            .AddPointLoads([.. pointValues])
            //.ApplyIntegrationBoundaryConditions(1, startPointValue)
            .Build();

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

        //List<DiagramPointValue> boundaryConditions = [

        MomentDiagramId diagramId = new();
        return new(
            modelId,
            modelResultId,
            element1d,
            localShearDirection,
            elementLength,
            lengthUnit,
            forceUnit,
            db.Intervals.Select(i => new MomentDiagramConsistentInterval(diagramId, i)).ToList(),
            diagramId
        )
        {
            GlobalShearDirection = globalShearDirection
        };
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private MomentDiagram()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
