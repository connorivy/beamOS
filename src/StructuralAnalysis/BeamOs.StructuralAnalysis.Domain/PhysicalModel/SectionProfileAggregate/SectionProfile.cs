using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DesignCodes.AISC._360_16;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

public class SectionProfile
    : BeamOsModelEntity<SectionProfileId>,
        IHasStrongAxisPlasticSectionModulus
{
    public SectionProfile(
        ModelId modelId,
        Area area,
        AreaMomentOfInertia strongAxisMomentOfInertia,
        AreaMomentOfInertia weakAxisMomentOfInertia,
        AreaMomentOfInertia polarMomentOfInertia,
        Volume strongAxisPlasticSectionModulus,
        Volume weakAxisPlasticSectionModulus,
        Area? strongAxisShearArea,
        Area? weakAxisShearArea,
        SectionProfileId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.ModelId = modelId;
        this.Area = area;
        this.StrongAxisMomentOfInertia = strongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = weakAxisMomentOfInertia;
        this.PolarMomentOfInertia = polarMomentOfInertia;
        this.StrongAxisPlasticSectionModulus = strongAxisPlasticSectionModulus;
        this.WeakAxisPlasticSectionModulus = weakAxisPlasticSectionModulus;
        this.StrongAxisShearArea = strongAxisShearArea;
        this.WeakAxisShearArea = weakAxisShearArea;
    }

    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }
    public Volume StrongAxisPlasticSectionModulus { get; set; }
    public Volume WeakAxisPlasticSectionModulus { get; set; }
    public Area? StrongAxisShearArea { get; set; }
    public Area? WeakAxisShearArea { get; set; }

    public static SectionProfile FromStructuralShapeData(
        ModelId modelId,
        StructuralShapes.Contracts.AiscWShapeData aiscWShapeData
    )
    {
        return new(
            modelId,
            aiscWShapeData.A,
            aiscWShapeData.Ix,
            aiscWShapeData.Iy,
            aiscWShapeData.J,
            aiscWShapeData.Zx,
            aiscWShapeData.Zy,
            null,
            null
        );
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private SectionProfile() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
