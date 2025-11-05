using System.ComponentModel.DataAnnotations.Schema;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

internal class Node : NodeDefinition
{
    public Node(
        ModelId modelId,
        Point locationPoint,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : base(id ?? new(), modelId)
    // : base(modelId, locationPoint, restraint, id)
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint ?? Restraint.Free;
    }

    public override BeamOsObjectType NodeType
    {
        get => BeamOsObjectType.Node;
        protected set { } // EF Core requires a setter for the discriminator property
    }

    public Point LocationPoint { get; set; }
    public Restraint Restraint { get; set; }

    public override Point GetLocationPoint(
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    ) => this.LocationPoint;

    public override bool DependsOnElement1d(
        Element1dId element1dId,
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    ) => false;

    public override bool DependsOnNode(
        NodeId nodeId,
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    ) => false;

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

    public override bool MemberwiseEquals(NodeDefinition other)
    {
        if (other is not Node otherNode)
        {
            return false;
        }

        return this.LocationPoint.Equals(otherNode.LocationPoint)
            && this.Restraint.Equals(otherNode.Restraint);
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Node() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

internal abstract class NodeDefinition
    : BeamOsModelEntity<NodeId>,
        INode3D,
        IBeamOsModelEntity<NodeId, NodeDefinition>
{
    public const string TypeDiscriminator = "NodeType";

    public NodeDefinition(NodeId id, ModelId modelId)
        : base(id, modelId) { }

    public Node? CastToNodeIfApplicable()
    {
        return this as Node;
    }

    public InternalNode? CastToInternalNodeIfApplicable()
    {
        return this as InternalNode;
    }

    public abstract BeamOsObjectType NodeType { get; protected set; }
    public ICollection<PointLoad>? PointLoads { get; set; }
    public ICollection<MomentLoad>? MomentLoads { get; set; }
    public ICollection<Element1d>? StartNodeElements { get; set; }
    public ICollection<Element1d>? EndNodeElements { get; set; }

    public abstract Point GetLocationPoint(
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    );

    public abstract bool DependsOnElement1d(
        Element1dId element1dId,
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    );

    public abstract bool DependsOnNode(
        NodeId nodeId,
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    );

    public abstract Node ToNode();

    public abstract bool MemberwiseEquals(NodeDefinition other);

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
    protected NodeDefinition() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

internal readonly record struct NodeIdAndLocation(NodeId NodeId, Point LocationPoint);

internal interface INode3D
{
    public Point GetLocationPoint(
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    );

    public bool DependsOnElement1d(
        Element1dId element1dId,
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    );

    public bool DependsOnNode(
        NodeId nodeId,
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    );
}
