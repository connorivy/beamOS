using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Application.PhysicalModel.Materials.Interfaces;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Application.PhysicalModel.SectionProfiles.Interfaces;
using BeamOs.Domain.Common.Models;
using UnitsNet;

namespace BeamOs.Infrastructure.Data.Models;

internal class Element1dReadModel : BeamOSEntity<Guid>, IElement1dData
{
    public Guid ModelId { get; private set; }
    public Guid StartNodeId { get; private set; }
    public NodeReadModel? StartNode { get; set; }
    public Guid EndNodeId { get; private set; }
    public NodeReadModel? EndNode { get; set; }
    public Guid MaterialId { get; private set; }
    public MaterialReadModel? Material { get; private set; }
    public Guid SectionProfileId { get; private set; }
    public SectionProfileReadModel? SectionProfile { get; private set; }

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; set; }

    INodeData? IElement1dData.StartNode => this.StartNode;

    INodeData? IElement1dData.EndNode => this.EndNode;

    IMaterialData? IElement1dData.Material => this.Material;

    ISectionProfileData? IElement1dData.SectionProfile => this.SectionProfile;
}
