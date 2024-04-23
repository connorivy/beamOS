using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Application.PhysicalModel.Materials.Interfaces;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Application.PhysicalModel.SectionProfiles.Interfaces;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Models.Interfaces;

public interface IModelData : IEntityData
{
    public string? Name { get; }
    public string? Description { get; }
    public ModelSettings Settings { get; }
    public List<INodeData>? Nodes { get; }
    public List<IElement1dData>? Element1ds { get; }
    public List<IMaterialData>? Materials { get; }
    public List<ISectionProfileData>? SectionProfiles { get; }
}
