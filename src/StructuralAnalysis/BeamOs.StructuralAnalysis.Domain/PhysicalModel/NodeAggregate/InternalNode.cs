using System.ComponentModel.DataAnnotations.Schema;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public class InternalNode : BeamOsModelEntity<NodeId>
{
    public InternalNode(
        ModelId modelId,
        Ratio ratioAlongElement1d,
        Element1dId element1DId,
        NodeId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.RatioAlongElement1d = ratioAlongElement1d;
        this.Element1dId = element1DId;
    }

    public Ratio RatioAlongElement1d { get; set; }
    public Element1dId Element1dId { get; set; }
    public Element1d? Element1d { get; set; }

    [NotMapped]
    public IEnumerable<Element1d>? Elements =>
        this.StartNodeElements?.Union(
            this.EndNodeElements
                ?? throw new InvalidOperationException(
                    "StartNodeElements is not null but EndNodeElements is null."
                )
        );
    public ICollection<Element1d>? StartNodeElements { get; set; }
    public ICollection<Element1d>? EndNodeElements { get; set; }
    public ICollection<PointLoad>? PointLoads { get; set; }
    public ICollection<MomentLoad>? MomentLoads { get; set; }

    public Point GetLocationPoint(Element1d element1d)
    {
        if (this.Element1dId != element1d.Id)
        {
            throw new InvalidOperationException(
                "The Element1dId of the InternalNode does not match the provided Element1d."
            );
        }

        if (element1d.StartNode is null || element1d.EndNode is null)
        {
            throw new InvalidOperationException(
                "Element1d must have both StartNode and EndNode defined to calculate the location point."
            );
        }

        return element1d.GetPointAtRatio(this.RatioAlongElement1d);
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
