using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.CsSdk;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public partial class BeamOsModelBuilderDomainMapper(Guid modelId)
{
    private readonly ModelId modelIdTyped = new(modelId);

    private ModelId GetModelId() => this.modelIdTyped;

    public partial Model ToDomain(BeamOsModelBuilder builder);

    public DsmAnalysisModel ToDsm(BeamOsModelBuilder builder, out Model model)
    {
        model = this.ToDomain(builder);

        var nodeDict = model.Nodes.ToDictionary(x => x.Id);
        var materialDict = model.Materials.ToDictionary(x => x.Id);
        var sectionProfileDict = model.SectionProfiles.ToDictionary(x => x.Id);

        foreach (var el in model.Element1ds)
        {
            el.StartNode = nodeDict[el.StartNodeId];
            el.EndNode = nodeDict[el.EndNodeId];
            el.Material = materialDict[el.MaterialId];
            el.SectionProfile = sectionProfileDict[el.SectionProfileId];
        }

        foreach (var n in model.Nodes)
        {
            n.PointLoads =  [];
            n.MomentLoads =  [];
        }

        foreach (var pl in model.PointLoads)
        {
            //if (nodeDict[pl.NodeId].PointLoads == null)
            //{
            //    nodeDict[pl.NodeId].PointLoads = new List<PointLoad>();
            //}
            nodeDict[pl.NodeId].PointLoads.Add(pl);
        }

        foreach (var ml in model.MomentLoads)
        {
            //if (nodeDict[ml.NodeId].MomentLoads == null)
            //{
            //    nodeDict[ml.NodeId].MomentLoads = new List<MomentLoad>();
            //}
            nodeDict[ml.NodeId].MomentLoads.Add(ml);
        }

        return new DsmAnalysisModel(model);
    }

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial Node ToDomain(CreateNodeRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial Element1d ToDomain(CreateElement1dRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial PointLoad ToDomain(CreatePointLoadRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial MomentLoad ToDomain(CreateMomentLoadRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial Material ToDomain(CreateMaterialRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial SectionProfile ToDomain(CreateSectionProfileRequest request);
}
