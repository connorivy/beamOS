using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.StructuralAnalysis.Sdk;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal partial class BeamOsModelBuilderDomainMapper(Guid modelId)
{
    // private readonly ModelId modelIdTyped = new(modelId);
    private ModelId modelIdTyped => modelId;

    private ModelId GetModelId() => this.modelIdTyped;

    public partial Model ToDomain(IBeamOsModel builder);

    public DsmAnalysisModel ToDsm(IBeamOsModel builder) => this.ToDsm(builder, out _);

    public DsmAnalysisModel ToDsm(IBeamOsModel builder, out Model model)
    {
        model = this.ToDomain(builder);

        var nodeDict = model
            .Nodes.Concat<NodeDefinition>(model.InternalNodes)
            .ToDictionary(x => x.Id);

        var internalNodeDict = model
            .InternalNodes.GroupBy(el => el.Element1dId)
            .ToDictionary(x => x.Key, x => x.ToList());

        var materialDict = model.Materials.ToDictionary(x => x.Id);
        var sectionProfileDict = (model.SectionProfiles ?? [])
            .Concat<SectionProfileInfoBase>(model.SectionProfilesFromLibrary ?? [])
            .ToDictionary(x => x.Id);

        foreach (var el in model.Element1ds)
        {
            el.StartNode = nodeDict[el.StartNodeId];
            el.EndNode = nodeDict[el.EndNodeId];
            el.InternalNodes = internalNodeDict.GetValueOrDefault(el.Id, []);
            el.Material = materialDict[el.MaterialId];
            el.SectionProfile = sectionProfileDict[el.SectionProfileId].GetSectionProfile();
        }

        foreach (var n in model.Nodes)
        {
            n.PointLoads = [];
            n.MomentLoads = [];
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
    public partial Node ToDomain(PutNodeRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial InternalNode ToDomain(InternalNodeContract request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial Element1d ToDomain(PutElement1dRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial LoadCase ToDomain(LoadCaseContract request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial LoadCombination ToDomain(LoadCombinationContract request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial PointLoad ToDomain(PutPointLoadRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial MomentLoad ToDomain(PutMomentLoadRequest request);

    [MapValue("ModelId", Use = nameof(GetModelId))]
    public partial Domain.PhysicalModel.SectionProfileAggregate.SectionProfileFromLibrary ToDomain(
        SectionProfileFromLibraryContract request
    );

    public Material ToDomain(PutMaterialRequest request) =>
        new Material(
            modelId,
            new(request.ModulusOfElasticity, request.PressureUnit.MapToPressureUnit()),
            new(request.ModulusOfRigidity, request.PressureUnit.MapToPressureUnit()),
            new(request.Id)
        );

    public SectionProfile ToDomain(PutSectionProfileRequest request) =>
        new(
            modelId,
            request.Name,
            new(request.Area, request.AreaUnit.MapToAreaUnit()),
            new(
                request.StrongAxisMomentOfInertia,
                request.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
            ),
            new(
                request.WeakAxisMomentOfInertia,
                request.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
            ),
            new(
                request.PolarMomentOfInertia,
                request.AreaMomentOfInertiaUnit.MapToAreaMomentOfInertiaUnit()
            ),
            new(request.StrongAxisPlasticSectionModulus, request.VolumeUnit.MapToVolumeUnit()),
            new(request.WeakAxisPlasticSectionModulus, request.VolumeUnit.MapToVolumeUnit()),
            request.StrongAxisShearArea.HasValue
                ? new(request.StrongAxisShearArea.Value, request.AreaUnit.MapToAreaUnit())
                : null,
            request.WeakAxisShearArea.HasValue
                ? new(request.WeakAxisShearArea.Value, request.AreaUnit.MapToAreaUnit())
                : null,
            new(request.Id)
        );
}
