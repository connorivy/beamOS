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
    public OctreeNodeId OctreeNodeId { get; set; }

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

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Node() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

internal abstract class NodeDefinition : BeamOsModelEntity<NodeId>, INode3D
{
    public const string TypeDiscriminator = "NodeType";

    public NodeDefinition(NodeId id, ModelId modelId)
        : base(id, modelId) { }

    // public NodeDefinition(
    //     ModelId modelId,
    //     Point locationPoint,
    //     Restraint? restraint = null,
    //     NodeId? id = null
    // )
    //     : base(id ?? new(), modelId)
    // {
    //     this.TypeDiscriminator = nameof(Node);
    //     this.SpatialNodeDefinition = new SpatialNodeDefinition(
    //         locationPoint,
    //         restraint ?? Restraint.Free
    //     );
    //     this.InternalNodeDefinition = InternalNodeDefinition.Default;
    // }

    // public NodeDefinition(
    //     ModelId modelId,
    //     Element1dId element1dId,
    //     Ratio ratioAlongElement1d,
    //     Restraint? restraint = null,
    //     NodeId? id = null
    // )
    //     : base(id ?? new(), modelId)
    // {
    //     this.TypeDiscriminator = nameof(InternalNode);
    //     this.InternalNodeDefinition = new()
    //     {
    //         RatioAlongElement1d = ratioAlongElement1d,
    //         Element1dId = element1dId,
    //         Restraint = restraint ?? Restraint.Free,
    //     };
    //     this.SpatialNodeDefinition = SpatialNodeDefinition.Default;
    // }

    public Node? CastToNodeIfApplicable()
    {
        return this as Node;
    }

    public InternalNode? CastToInternalNodeIfApplicable()
    {
        return this as InternalNode;
    }

    // public InternalNodeDefinition InternalNodeDefinition { get; private set; }
    // public SpatialNodeDefinition SpatialNodeDefinition { get; private set; }
    // public string TypeDiscriminator { get; private set; }

    // Navigation properties
    // public Element1d? Element1d { get; set; }
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

    // {
    //     if (this.TypeDiscriminator == nameof(InternalNode))
    //     {
    //         if (this.InternalNodeDefinition is null)
    //         {
    //             throw new InvalidOperationException(
    //                 "InternalNodeDefinition must be set for InternalNode."
    //             );
    //         }
    //         if (this.Element1d is null)
    //         {
    //             throw new InvalidOperationException(
    //                 "Element1d must be set before calculating the location point."
    //             );
    //         }
    //         return this.Element1d.GetPointAtRatio(this.InternalNodeDefinition.RatioAlongElement1d);
    //     }
    //     else if (this.TypeDiscriminator == nameof(Node))
    //     {
    //         if (this.SpatialNodeDefinition is null)
    //         {
    //             throw new InvalidOperationException(
    //                 "SpacialNodeDefinition must be set for SpacialNode."
    //             );
    //         }
    //         return this.SpatialNodeDefinition.LocationPoint;
    //     }
    //     throw new InvalidOperationException($"Unknown Node type: {this.TypeDiscriminator}.");
    // }

    public abstract Node ToNode();

    // {
    //     if (this.TypeDiscriminator == nameof(InternalNode))
    //     {
    //         if (this.InternalNodeDefinition is null)
    //         {
    //             throw new InvalidOperationException(
    //                 "InternalNodeDefinition must be set for InternalNode."
    //             );
    //         }
    //         return new Node(this.ModelId, this.GetLocationPoint(), Restraint.Free, this.Id)
    //         {
    //             InternalNodeDefinition = this.InternalNodeDefinition,
    //             TypeDiscriminator = nameof(InternalNode),
    //             StartNodeElements = this.StartNodeElements,
    //             EndNodeElements = this.EndNodeElements,
    //             PointLoads = this.PointLoads,
    //             MomentLoads = this.MomentLoads,
    //         };
    //     }
    //     else if (this.TypeDiscriminator == nameof(Node))
    //     {
    //         if (this.SpatialNodeDefinition is null)
    //         {
    //             throw new InvalidOperationException(
    //                 "SpacialNodeDefinition must be set for SpacialNode."
    //             );
    //         }
    //         return new Node(
    //             this.ModelId,
    //             this.GetLocationPoint(),
    //             this.SpatialNodeDefinition.Restraint,
    //             this.Id
    //         )
    //         {
    //             SpatialNodeDefinition = this.SpatialNodeDefinition,
    //             TypeDiscriminator = nameof(Node),
    //             StartNodeElements = this.StartNodeElements,
    //             EndNodeElements = this.EndNodeElements,
    //             PointLoads = this.PointLoads,
    //             MomentLoads = this.MomentLoads,
    //         };
    //     }
    //     throw new InvalidOperationException($"Unknown Node type: {this.TypeDiscriminator}.");
    // }

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
