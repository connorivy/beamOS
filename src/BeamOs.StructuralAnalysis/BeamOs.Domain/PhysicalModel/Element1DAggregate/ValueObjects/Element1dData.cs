using BeamOs.Common.Domain.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;

public class Element1dData : BeamOSValueObject
{
    public Element1dData(
        NodeData startNodeData,
        NodeData endNodeData,
        MaterialData materialData,
        SectionProfileData sectionProfileData,
        Angle sectionProfileRotation
    )
    {
        this.StartNodeData = startNodeData;
        this.EndNodeData = endNodeData;
        this.MaterialData = materialData;
        this.SectionProfileData = sectionProfileData;
        this.SectionProfileRotation = sectionProfileRotation;
        this.BaseLine = new(startNodeData.LocationPoint, endNodeData.LocationPoint);
    }

    public Line BaseLine { get; }
    public NodeData StartNodeData { get; }
    public NodeData EndNodeData { get; }
    public MaterialData MaterialData { get; }
    public SectionProfileData SectionProfileData { get; }

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.StartNodeData;
        yield return this.EndNodeData;
        yield return this.MaterialData;
        yield return this.SectionProfileData;
        yield return this.SectionProfileRotation;
    }
}
