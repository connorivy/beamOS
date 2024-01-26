using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.Element1DAggregate;

public class Element1D : AggregateRoot<Element1DId>
{
    public Element1D(
        ModelId modelId,
        NodeId startNodeId,
        NodeId endNodeId,
        MaterialId materialId,
        SectionProfileId sectionProfileId,
        Element1DId? id = null
    )
        : base(id ?? new Element1DId())
    {
        this.ModelId = modelId;
        this.StartNodeId = startNodeId;
        this.EndNodeId = endNodeId;
        this.MaterialId = materialId;
        this.SectionProfileId = sectionProfileId;
    }

    public ModelId ModelId { get; private set; }
    public NodeId StartNodeId { get; private set; }
    public NodeId EndNodeId { get; private set; }
    public MaterialId MaterialId { get; private set; }
    public SectionProfileId SectionProfileId { get; private set; }

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; set; }

    //private readonly SortedList<double, PointLoad> loads = new();
    //public IReadOnlyDictionary<double, PointLoad> Loads => this.loads.AsReadOnly();

    //public Line BaseLine { get; }
    //public Length Length => this.BaseLine.Length;

    public static Line GetBaseLine(Point startPoint, Point endPoint)
    {
        return new(startPoint, endPoint);
    }
}
