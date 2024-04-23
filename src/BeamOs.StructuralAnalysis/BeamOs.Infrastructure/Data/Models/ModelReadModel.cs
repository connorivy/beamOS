using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Application.PhysicalModel.Materials.Interfaces;
using BeamOs.Application.PhysicalModel.Models.Interfaces;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Application.PhysicalModel.SectionProfiles.Interfaces;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class ModelReadModel : ReadModelBase, IModelData
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public ModelSettings Settings { get; private set; }
    public List<NodeReadModel>? Nodes { get; private set; }
    public List<Element1dReadModel>? Element1ds { get; private set; }
    public List<MaterialReadModel>? Materials { get; private set; }
    public List<SectionProfileReadModel>? SectionProfiles { get; private set; }

    List<INodeData>? IModelData.Nodes => this.Nodes?.ToList<INodeData>();

    List<IElement1dData>? IModelData.Element1ds => this.Element1ds?.ToList<IElement1dData>();

    List<IMaterialData>? IModelData.Materials => this.Materials?.ToList<IMaterialData>();

    List<ISectionProfileData>? IModelData.SectionProfiles =>
        this.SectionProfiles?.ToList<ISectionProfileData>();
    //public List<PointLoadReadModel>? PointLoads { get; private set; }
    //public List<MomentLoadReadModel>? MomentLoads { get; private set; }
}
