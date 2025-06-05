using System.ComponentModel.DataAnnotations.Schema;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public class Node : NodeBase
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

    public override Point GetLocationPoint() => this.LocationPoint;

    public override Node ToNode() => this;

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

public abstract class NodeBase : BeamOsModelEntity<NodeId>
{
    protected NodeBase(NodeId id, ModelId modelId)
        : base(id, modelId)
    {
        this.StartNodeElements = new HashSet<Element1d>();
        this.EndNodeElements = new HashSet<Element1d>();
        this.PointLoads = new HashSet<PointLoad>();
        this.MomentLoads = new HashSet<MomentLoad>();
    }

    public ICollection<PointLoad> PointLoads { get; set; }
    public ICollection<MomentLoad> MomentLoads { get; set; }
    public ICollection<Element1d> StartNodeElements { get; set; }
    public ICollection<Element1d> EndNodeElements { get; set; }

    public abstract Point GetLocationPoint();
    public abstract Node ToNode();

    [NotMapped]
    public IEnumerable<Element1d>? Elements =>
        this.StartNodeElements?.Union(
            this.EndNodeElements
                ?? throw new InvalidOperationException(
                    "StartNodeElements is not null but EndNodeElements is null."
                )
        );

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected NodeBase() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
