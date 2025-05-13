using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using Microsoft.EntityFrameworkCore;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public class Node : BeamOsModelEntity<NodeId>
{
    public Node(
        ModelId modelId,
        Point locationPoint,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint ?? Restraint.Free;
    }

    public Point LocationPoint { get; set; }
    public Restraint Restraint { get; set; }

    public ICollection<PointLoad>? PointLoads { get; set; }

    public ICollection<MomentLoad>? MomentLoads { get; set; }

    //public NodeResult? NodeResult { get; private set; }

    public Forces GetForcesInGlobalCoordinates(LoadCombination loadCombination)
    {
        var forceAlongX = Force.Zero;
        var forceAlongY = Force.Zero;
        var forceAlongZ = Force.Zero;
        var momentAboutX = Torque.Zero;
        var momentAboutY = Torque.Zero;
        var momentAboutZ = Torque.Zero;

        foreach (var linearLoad in this.PointLoads)
        {
            forceAlongX += linearLoad.GetScaledForce(
                CoordinateSystemDirection3D.AlongX,
                loadCombination
            );
            forceAlongY += linearLoad.GetScaledForce(
                CoordinateSystemDirection3D.AlongY,
                loadCombination
            );
            forceAlongZ += linearLoad.GetScaledForce(
                CoordinateSystemDirection3D.AlongZ,
                loadCombination
            );
        }
        foreach (var momentLoad in this.MomentLoads)
        {
            momentAboutX += momentLoad.GetScaledTorque(
                CoordinateSystemDirection3D.AboutX,
                loadCombination
            );
            momentAboutY += momentLoad.GetScaledTorque(
                CoordinateSystemDirection3D.AboutY,
                loadCombination
            );
            momentAboutZ += momentLoad.GetScaledTorque(
                CoordinateSystemDirection3D.AboutZ,
                loadCombination
            );
        }
        return new(forceAlongX, forceAlongY, forceAlongZ, momentAboutX, momentAboutY, momentAboutZ);
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Node() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

[PrimaryKey(nameof(Id), [nameof(ModelId), nameof(ModelChangeRequestId)])]
public sealed class NodeChangeRequest : Node
{
    public NodeChangeRequest(
        NodeId id,
        ModelId modelId,
        ModelChangeRequestId modelChangeRequestId,
        Point locationPoint,
        Restraint restraint
    )
        : base(modelId, locationPoint, restraint, id)
    {
        this.ModelChangeRequestId = modelChangeRequestId;
    }

    public ModelChangeRequestId ModelChangeRequestId { get; }

    public static NodeChangeRequest FromNode(ModelChangeRequestId modelChangeRequestId, Node node)
    {
        return new(node.Id, node.ModelId, modelChangeRequestId, node.LocationPoint, node.Restraint);
    }

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private NodeChangeRequest()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
