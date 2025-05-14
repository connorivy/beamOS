using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DesignCodes.AISC._360_16;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

public class SectionProfile : SectionProfileInfoBase, IHasStrongAxisPlasticSectionModulus
{
    public SectionProfile(
        ModelId modelId,
        string name,
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
        : base(modelId, name, id)
    {
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

    public override SectionProfile GetSectionProfile() => this;

    public static SectionProfile FromStructuralShapeData(
        ModelId modelId,
        StructuralShapes.Contracts.AiscWShapeData aiscWShapeData,
        SectionProfileId? id = null
    )
    {
        return new(
            modelId,
            aiscWShapeData.Name,
            aiscWShapeData.A,
            aiscWShapeData.Ix,
            aiscWShapeData.Iy,
            aiscWShapeData.J,
            aiscWShapeData.Zx,
            aiscWShapeData.Zy,
            null,
            null,
            id
        );
    }

    public static SectionProfile? FromLibraryValue(
        ModelId modelId,
        StructuralCode structuralCode,
        string sectionProfileName,
        SectionProfileId? id = null
    )
    {
        string lowerSectionProfileName = sectionProfileName.ToLowerInvariant();

        switch (structuralCode)
        {
            case StructuralCode.AISC_360_16:
                if (lowerSectionProfileName.StartsWith("wt", StringComparison.OrdinalIgnoreCase))
                {
                    // make sure that wt shapes do not fall into the w category
                }
                else if (
                    lowerSectionProfileName.StartsWith("w", StringComparison.OrdinalIgnoreCase)
                )
                {
                    return FromStructuralShapeData(
                        modelId,
                        StructuralShapes.Lib.AISC.v16_0.WShapes.GetShapeByName(sectionProfileName),
                        id
                    );
                }
                // else if (lowerSectionProfileName.StartsWith("s", StringComparison.OrdinalIgnoreCase))
                // {
                //     return FromStructuralShapeData(
                //         modelId,
                //         StructuralShapes.Lib.AISC.v16_0.SShapes.GetShapeByName(sectionProfileName)
                //     );
                // }
                // else if (lowerSectionProfileName.StartsWith("c", StringComparison.OrdinalIgnoreCase))
                // {
                //     return FromStructuralShapeData(
                //         modelId,
                //         StructuralShapes.Lib.AISC.v16_0.CShapes.GetShapeByName(sectionProfileName)
                //     );
                // }
                // else if (lowerSectionProfileName.StartsWith("t", StringComparison.OrdinalIgnoreCase))
                // {
                //     return FromStructuralShapeData(
                //         modelId,
                //         StructuralShapes.Lib.AISC.v16_0.WShapes.GetShapeByName(sectionProfileName)
                //     );
                // }
                return null;
            case StructuralCode.Undefined:
            default:
                throw new NotImplementedException(
                    $"Structural code {structuralCode} not implemented."
                );
        }
    }

    public SectionProfile Copy(SectionProfileId id)
    {
        return new(
            this.ModelId,
            this.Name,
            this.Area,
            this.StrongAxisMomentOfInertia,
            this.WeakAxisMomentOfInertia,
            this.PolarMomentOfInertia,
            this.StrongAxisPlasticSectionModulus,
            this.WeakAxisPlasticSectionModulus,
            this.StrongAxisShearArea,
            this.WeakAxisShearArea,
            id
        );
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private SectionProfile() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
