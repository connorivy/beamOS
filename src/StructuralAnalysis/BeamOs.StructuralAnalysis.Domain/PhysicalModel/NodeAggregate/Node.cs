using System.ComponentModel.DataAnnotations.Schema;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public class Node : NodeDefinition
{
    public Node(
        ModelId modelId,
        Point locationPoint,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : base(modelId, locationPoint, restraint, id)
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint ?? Restraint.Free;
    }

    public Point LocationPoint { get; set; }
    public Restraint Restraint { get; set; }

    // public override Point GetLocationPoint() => this.LocationPoint;

    // public override Node ToNode() => this;

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

public class NodeDefinition : BeamOsModelEntity<NodeId>
{
    public NodeDefinition(
        ModelId modelId,
        Point locationPoint,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.TypeDiscriminator = nameof(Node);
        this.SpatialNodeDefinition = new SpatialNodeDefinition(
            locationPoint,
            restraint ?? Restraint.Free
        );
        this.InternalNodeDefinition = InternalNodeDefinition.Default;
    }

    public NodeDefinition(
        ModelId modelId,
        Element1dId element1dId,
        Ratio ratioAlongElement1d,
        NodeId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.TypeDiscriminator = nameof(InternalNode);
        this.InternalNodeDefinition = new()
        {
            RatioAlongElement1d = ratioAlongElement1d,
            Element1dId = element1dId,
        };
        this.SpatialNodeDefinition = SpatialNodeDefinition.Default;
    }

    public Node? CastToNodeIfApplicable()
    {
        if (this.TypeDiscriminator != nameof(Node))
        {
            return null;
        }

        if (this.SpatialNodeDefinition is null)
        {
            throw new InvalidOperationException("SpacialNodeDefinition must be set for Node.");
        }

        return new Node(
            this.ModelId,
            this.SpatialNodeDefinition.LocationPoint,
            this.SpatialNodeDefinition.Restraint,
            this.Id
        )
        {
            SpatialNodeDefinition = this.SpatialNodeDefinition,
            TypeDiscriminator = nameof(Node),
            StartNodeElements = this.StartNodeElements,
            EndNodeElements = this.EndNodeElements,
            PointLoads = this.PointLoads,
            MomentLoads = this.MomentLoads,
        };
    }

    public InternalNode? CastToInternalNodeIfApplicable()
    {
        if (this.TypeDiscriminator != nameof(InternalNode))
        {
            return null;
        }

        if (this.InternalNodeDefinition is null)
        {
            throw new InvalidOperationException("SpacialNodeDefinition must be set for Node.");
        }

        return new InternalNode(
            this.ModelId,
            this.InternalNodeDefinition.RatioAlongElement1d,
            this.InternalNodeDefinition.Element1dId,
            this.Id
        )
        {
            SpatialNodeDefinition = this.SpatialNodeDefinition,
            TypeDiscriminator = nameof(Node),
            StartNodeElements = this.StartNodeElements,
            EndNodeElements = this.EndNodeElements,
            PointLoads = this.PointLoads,
            MomentLoads = this.MomentLoads,
        };
    }

    public InternalNodeDefinition InternalNodeDefinition { get; private set; }
    public SpatialNodeDefinition SpatialNodeDefinition { get; private set; }
    public string TypeDiscriminator { get; private set; }

    // Navigation properties
    public Element1d? Element1d { get; set; }
    public ICollection<PointLoad> PointLoads { get; set; }
    public ICollection<MomentLoad> MomentLoads { get; set; }
    public ICollection<Element1d> StartNodeElements { get; set; }
    public ICollection<Element1d> EndNodeElements { get; set; }

    public Point GetLocationPoint()
    {
        if (this.TypeDiscriminator == nameof(InternalNode))
        {
            if (this.InternalNodeDefinition is null)
            {
                throw new InvalidOperationException(
                    "InternalNodeDefinition must be set for InternalNode."
                );
            }
            if (this.Element1d is null)
            {
                throw new InvalidOperationException(
                    "Element1d must be set before calculating the location point."
                );
            }
            return this.Element1d.GetPointAtRatio(this.InternalNodeDefinition.RatioAlongElement1d);
        }
        else if (this.TypeDiscriminator == nameof(Node))
        {
            if (this.SpatialNodeDefinition is null)
            {
                throw new InvalidOperationException(
                    "SpacialNodeDefinition must be set for SpacialNode."
                );
            }
            return this.SpatialNodeDefinition.LocationPoint;
        }
        throw new InvalidOperationException($"Unknown Node type: {this.TypeDiscriminator}.");
    }

    public Node ToNode()
    {
        if (this.TypeDiscriminator == nameof(InternalNode))
        {
            if (this.InternalNodeDefinition is null)
            {
                throw new InvalidOperationException(
                    "InternalNodeDefinition must be set for InternalNode."
                );
            }
            return new Node(this.ModelId, this.GetLocationPoint(), Restraint.Free, this.Id)
            {
                InternalNodeDefinition = this.InternalNodeDefinition,
                TypeDiscriminator = nameof(InternalNode),
                StartNodeElements = this.StartNodeElements,
                EndNodeElements = this.EndNodeElements,
                PointLoads = this.PointLoads,
                MomentLoads = this.MomentLoads,
            };
        }
        else if (this.TypeDiscriminator == nameof(Node))
        {
            if (this.SpatialNodeDefinition is null)
            {
                throw new InvalidOperationException(
                    "SpacialNodeDefinition must be set for SpacialNode."
                );
            }
            return new Node(
                this.ModelId,
                this.GetLocationPoint(),
                this.SpatialNodeDefinition.Restraint,
                this.Id
            )
            {
                SpatialNodeDefinition = this.SpatialNodeDefinition,
                TypeDiscriminator = nameof(Node),
                StartNodeElements = this.StartNodeElements,
                EndNodeElements = this.EndNodeElements,
                PointLoads = this.PointLoads,
                MomentLoads = this.MomentLoads,
            };
        }
        throw new InvalidOperationException($"Unknown Node type: {this.TypeDiscriminator}.");
    }

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

public sealed class InternalNodeDefinition : BeamOSValueObject
{
    public required Ratio RatioAlongElement1d { get; set; }
    public required Element1dId Element1dId { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.RatioAlongElement1d;
        yield return this.Element1dId;
    }

    public static InternalNodeDefinition Default { get; } =
        new InternalNodeDefinition { RatioAlongElement1d = Ratio.Zero, Element1dId = 0 };
}

public sealed class SpatialNodeDefinition : BeamOSValueObject
{
    public Point LocationPoint { get; set; }
    public Restraint Restraint { get; set; }

    public SpatialNodeDefinition(Point locationPoint, Restraint restraint)
    {
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
    }

    [Obsolete("EF Core Constructor", true)]
    public SpatialNodeDefinition() { }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.LocationPoint;
        yield return this.Restraint;
    }

    /// <summary>
    /// This is the default that is needed because EF Core does not currently support complex types being null
    /// </summary>
    public static SpatialNodeDefinition Default { get; } =
        new SpatialNodeDefinition(
            new(-852586.0, -454545, -123456, LengthUnit.Meter),
            Restraint.Free
        );
}
