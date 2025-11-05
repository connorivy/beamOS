using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DesignCodes.AISC._360_16;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

internal class SectionProfile
    : SectionProfileInfoBase,
        IHasStrongAxisPlasticSectionModulus,
        IBeamOsModelEntity<SectionProfileId, SectionProfile>
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

    public override BeamOsObjectType SectionProfileType
    {
        get => BeamOsObjectType.SectionProfile;
        protected set { } // EF Core requires a setter for the discriminator property
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
            // todo: these are the values that sap2000 uses for shear area, but they don't seem to be consistent
            // with ETABS and those two are not consistent with FEM versions of the effective shear area
            // https://github.com/robbievanleeuwen/section-properties/issues/526
            // https://www.eng-tips.com/threads/shear-area-sap2000-vs-etabs.493951/
            5.0 / 6 * 2 * aiscWShapeData.tf * aiscWShapeData.bf,
            aiscWShapeData.tw * aiscWShapeData.d,
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

    public bool MemberwiseEquals(SectionProfile other)
    {
        var momentOfInertiaTolerance = new AreaMomentOfInertia(
            .0001,
            AreaMomentOfInertiaUnit.MeterToTheFourth
        );
        return this.Area.Equals(other.Area, new Area(.0001, AreaUnit.SquareMeter))
            && this.StrongAxisMomentOfInertia.Equals(
                other.StrongAxisMomentOfInertia,
                momentOfInertiaTolerance
            )
            && this.WeakAxisMomentOfInertia.Equals(
                other.WeakAxisMomentOfInertia,
                momentOfInertiaTolerance
            )
            && this.PolarMomentOfInertia.Equals(
                other.PolarMomentOfInertia,
                momentOfInertiaTolerance
            )
            && this.StrongAxisPlasticSectionModulus.Equals(
                other.StrongAxisPlasticSectionModulus,
                momentOfInertiaTolerance
            )
            && this.WeakAxisPlasticSectionModulus.Equals(
                other.WeakAxisPlasticSectionModulus,
                momentOfInertiaTolerance
            )
            && (
                this.StrongAxisShearArea?.Equals(
                    other.StrongAxisShearArea,
                    new Area(.0001, AreaUnit.SquareMeter)
                ) ?? (other.StrongAxisShearArea is null)
            )
            && (
                this.WeakAxisShearArea?.Equals(
                    other.WeakAxisShearArea,
                    new Area(.0001, AreaUnit.SquareMeter)
                ) ?? (other.WeakAxisShearArea is null)
            );
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected SectionProfile() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

internal abstract class SectionProfileProposalInfoBase
    : BeamOsModelProposalEntity<SectionProfileProposalId, SectionProfileId>
{
    public SectionProfileProposalInfoBase(
        ModelId modelId,
        ModelProposalId modelProposalId,
        string name,
        SectionProfileProposalId? id = null
    )
        : base(id ?? new(), modelProposalId, modelId)
    {
        this.Name = name;
    }

    public string Name { get; set; }

    public abstract SectionProfileInfoBase ToDomain();

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected SectionProfileProposalInfoBase() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

internal sealed class SectionProfileProposal : SectionProfileProposalInfoBase
{
    public SectionProfileProposal(
        ModelId modelId,
        ModelProposalId modelProposalId,
        string name,
        Area area,
        AreaMomentOfInertia strongAxisMomentOfInertia,
        AreaMomentOfInertia weakAxisMomentOfInertia,
        AreaMomentOfInertia polarMomentOfInertia,
        Volume strongAxisPlasticSectionModulus,
        Volume weakAxisPlasticSectionModulus,
        Area? strongAxisShearArea,
        Area? weakAxisShearArea,
        SectionProfileProposalId? id = null
    )
        : base(modelId, modelProposalId, name, id)
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

    public SectionProfileProposal(
        SectionProfile sectionProfile,
        ModelProposalId modelProposalId,
        string? name = null,
        Area? area = null,
        AreaMomentOfInertia? strongAxisMomentOfInertia = null,
        AreaMomentOfInertia? weakAxisMomentOfInertia = null,
        AreaMomentOfInertia? polarMomentOfInertia = null,
        Volume? strongAxisPlasticSectionModulus = null,
        Volume? weakAxisPlasticSectionModulus = null,
        Area? strongAxisShearArea = null,
        Area? weakAxisShearArea = null,
        SectionProfileProposalId? id = null
    )
        : this(
            sectionProfile.ModelId,
            modelProposalId,
            name ?? sectionProfile.Name,
            area ?? sectionProfile.Area,
            strongAxisMomentOfInertia ?? sectionProfile.StrongAxisMomentOfInertia,
            weakAxisMomentOfInertia ?? sectionProfile.WeakAxisMomentOfInertia,
            polarMomentOfInertia ?? sectionProfile.PolarMomentOfInertia,
            strongAxisPlasticSectionModulus ?? sectionProfile.StrongAxisPlasticSectionModulus,
            weakAxisPlasticSectionModulus ?? sectionProfile.WeakAxisPlasticSectionModulus,
            strongAxisShearArea ?? sectionProfile.StrongAxisShearArea,
            weakAxisShearArea ?? sectionProfile.WeakAxisShearArea,
            id
        ) { }

    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }
    public Volume StrongAxisPlasticSectionModulus { get; set; }
    public Volume WeakAxisPlasticSectionModulus { get; set; }
    public Area? StrongAxisShearArea { get; set; }
    public Area? WeakAxisShearArea { get; set; }

    public override SectionProfile ToDomain()
    {
        return new SectionProfile(
            this.ModelId,
            this.Name,
            this.Area,
            this.StrongAxisMomentOfInertia,
            this.WeakAxisMomentOfInertia,
            this.PolarMomentOfInertia,
            this.StrongAxisPlasticSectionModulus,
            this.WeakAxisPlasticSectionModulus,
            this.StrongAxisShearArea,
            this.WeakAxisShearArea
        );
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private SectionProfileProposal() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

internal sealed class SectionProfileProposalFromLibrary : SectionProfileProposalInfoBase
{
    public SectionProfileProposalFromLibrary(
        ModelId modelId,
        ModelProposalId modelProposalId,
        string name,
        StructuralCode library,
        SectionProfileProposalId? id = null
    )
        : base(modelId, modelProposalId, name, id)
    {
        this.Library = library;
    }

    public StructuralCode Library { get; set; }

    public override SectionProfileFromLibrary ToDomain()
    {
        return new SectionProfileFromLibrary(this.ModelId, this.Name, this.Library);
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private SectionProfileProposalFromLibrary() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
