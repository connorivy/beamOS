using BeamOs.Application.PhysicalModel.Element1dAggregate.Interfaces;
using BeamOs.Application.PhysicalModel.Materials.Interfaces;
using BeamOs.Application.PhysicalModel.Models.Interfaces;
using BeamOs.Application.PhysicalModel.Nodes.Interfaces;
using BeamOs.Application.PhysicalModel.SectionProfiles.Interfaces;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class ModelReadModel : BeamOSEntity<Guid>, IModelData
{
    public string? Name { get; internal set; }
    public string? Description { get; internal set; }
    public ModelSettings Settings { get; internal set; }
    public List<NodeReadModel>? Nodes { get; internal set; }
    public List<Element1dReadModel>? Element1ds { get; internal set; }
    public List<MaterialReadModel>? Materials { get; internal set; }
    public List<SectionProfileReadModel>? SectionProfiles { get; internal set; }
    public List<PointLoadReadModel>? PointLoads { get; internal set; }

    //public List<MomentLoadReadModel>? MomentLoads { get; internal set; }

    List<INodeData>? IModelData.Nodes => this.Nodes?.ToList<INodeData>();

    List<IElement1dData>? IModelData.Element1ds => this.Element1ds?.ToList<IElement1dData>();

    List<IMaterialData>? IModelData.Materials => this.Materials?.ToList<IMaterialData>();

    List<ISectionProfileData>? IModelData.SectionProfiles =>
        this.SectionProfiles?.ToList<ISectionProfileData>();
}
