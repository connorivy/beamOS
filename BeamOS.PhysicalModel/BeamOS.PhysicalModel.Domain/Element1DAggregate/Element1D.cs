using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.Element1DAggregate;
public class Element1D : AggregateRoot<Element1DId>
{
    public Element1D(
        Element1DId id,
        AnalyticalModelId analyticalModelId,
        AnalyticalNode startNode,
        AnalyticalNode endNode,
        MaterialId material,
        SectionProfileId sectionProfile
    ) : base(id)
    {
        this.AnalyticalModelId = analyticalModelId;
        this.StartNodeId = startNode.Id;
        this.EndNodeId = endNode.Id;
        this.MaterialId = material;
        this.SectionProfileId = sectionProfile;
        this.BaseLine = GetBaseLine(startNode.LocationPoint, endNode.LocationPoint);
    }

    public AnalyticalModelId AnalyticalModelId { get; }
    public AnalyticalNodeId StartNodeId { get; }
    public AnalyticalNodeId EndNodeId { get; }
    public MaterialId MaterialId { get; }
    public SectionProfileId SectionProfileId { get; }
    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; set; }

    private readonly SortedList<double, PointLoad> loads = new();
    public IReadOnlyDictionary<double, PointLoad> Loads => this.loads.AsReadOnly();

    public Line BaseLine { get; }
    public Length Length => this.BaseLine.Length;

    public static Line GetBaseLine(Point startPoint, Point endPoint)
    {
        return new(startPoint, endPoint);
    }
}
