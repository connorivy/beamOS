using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.Element1DAggregate;

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
    public Dictionary<Ratio, PointLoad> PointLoads { get; private set; } = [];

    public static Line GetBaseLine(Point startPoint, Point endPoint)
    {
        return new(startPoint, endPoint);
    }

    //public void AddPointLoad(Ratio locationAlongBeam, ImmutablePointLoad pointLoad)
    //{
    //    if (locationAlongBeam.As(UnitsNet.Units.RatioUnit.DecimalFraction) is < 0 or > 1)
    //    {
    //        throw new ArgumentException("Provided location along beam must be between 0 and 1");
    //    }

    //    this.PointLoads.Add(locationAlongBeam, new(new(), pointLoad));
    //}
}
