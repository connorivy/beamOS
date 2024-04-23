using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Materials.Interfaces;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Application.PhysicalModel.SectionProfiles.Interfaces;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;

public interface IElement1dData : IEntityData
{
    public Guid ModelId { get; }
    public Guid StartNodeId { get; }
    public INodeData? StartNode { get; }
    public Guid EndNodeId { get; }
    public INodeData? EndNode { get; }
    public Guid MaterialId { get; }
    public IMaterialData? Material { get; }
    public Guid SectionProfileId { get; }
    public ISectionProfileData? SectionProfile { get; }

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; }
}
