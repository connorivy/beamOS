using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public class InternalNode : NodeDefinition
{
    public InternalNode(
        ModelId modelId,
        Ratio ratioAlongElement1d,
        Element1dId element1DId,
        Restraint? restraint = null,
        NodeId? id = null
    )
        : base(id ?? new(), modelId)
    // : base(modelId, element1DId, ratioAlongElement1d, restraint, id)
    {
        this.RatioAlongElement1d = ratioAlongElement1d;
        this.Element1dId = element1DId;
        this.Restraint = restraint ?? Restraint.Free;
    }

    public override BeamOsObjectType NodeType
    {
        get => BeamOsObjectType.InternalNode;
        protected set { } // EF Core requires a setter for the discriminator property
    }

    public Restraint Restraint { get; set; }
    public Ratio RatioAlongElement1d { get; set; }
    public Element1dId Element1dId { get; set; }
    public Element1d? Element1d { get; set; }

    public override Point GetLocationPoint(
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    )
    {
        var element1d =
            (this.Element1d ?? elementStore?.GetValueOrDefault(this.Element1dId))
            ?? throw new InvalidOperationException(
                "Element1d must be set before calculating the location point."
            );

        return element1d.GetPointAtRatio(this.RatioAlongElement1d, elementStore, nodeStore);
    }

    public override bool DependsOnElement1d(
        Element1dId element1dId,
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    )
    {
        if (this.Element1dId == element1dId)
        {
            return true;
        }

        var element1d =
            (this.Element1d ?? elementStore?.GetValueOrDefault(this.Element1dId))
            ?? throw new InvalidOperationException(
                "Element1d must be set before checking dependencies."
            );

        var startNode =
            element1d.StartNode
            ?? nodeStore?.GetValueOrDefault(element1d.StartNodeId)
            ?? throw new InvalidOperationException(
                "StartNode must be set before checking dependencies."
            );
        var endNode =
            element1d.EndNode
            ?? nodeStore?.GetValueOrDefault(element1d.EndNodeId)
            ?? throw new InvalidOperationException(
                "EndNode must be set before checking dependencies."
            );

        return startNode.DependsOnElement1d(element1dId, elementStore, nodeStore)
            || endNode.DependsOnElement1d(element1dId, elementStore, nodeStore);
    }

    public override bool DependsOnNode(
        NodeId nodeId,
        IReadOnlyDictionary<Element1dId, Element1d>? elementStore = null,
        IReadOnlyDictionary<NodeId, NodeDefinition>? nodeStore = null
    )
    {
        if (this.Id == nodeId)
        {
            return true;
        }
        var element1d =
            (this.Element1d ?? elementStore?.GetValueOrDefault(this.Element1dId))
            ?? throw new InvalidOperationException(
                "Element1d must be set before checking dependencies."
            );
        var startNode =
            element1d.StartNode
            ?? nodeStore?.GetValueOrDefault(element1d.StartNodeId)
            ?? throw new InvalidOperationException(
                "StartNode must be set before checking dependencies."
            );
        var endNode =
            element1d.EndNode
            ?? nodeStore?.GetValueOrDefault(element1d.EndNodeId)
            ?? throw new InvalidOperationException(
                "EndNode must be set before checking dependencies."
            );

        return startNode.Id == nodeId
            || endNode.Id == nodeId
            || startNode.DependsOnNode(nodeId, elementStore, nodeStore)
            || endNode.DependsOnNode(nodeId, elementStore, nodeStore);
    }

    public override Node ToNode()
    {
        return new(this.ModelId, this.GetLocationPoint(), this.Restraint, this.Id);
    }

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
    protected InternalNode() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
