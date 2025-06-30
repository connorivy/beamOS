using System.Diagnostics;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.Common.Extensions;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.DesignCodes.AISC._360_16;

public class AiscSteelSectionSelector<TSectionProfile, TMaterial> : SectionSelectorBase
    where TSectionProfile : SectionProfile, IHasWebArea, IHasStrongAxisPlasticSectionModulus
    where TMaterial : ISteelMaterial
{
    public AiscSteelSelectionResult? SelectOptimalSection(
        Element1d element1d,
        Element1dResult element1dResult,
        TMaterial material,
        TSectionProfile[] sectionProfiles
    )
    {
        // todo: this is not very efficient. we could sort the sections by weight and then binary search
        // for the first section that passes the checks. this would be a lot faster.
        foreach (var section in sectionProfiles)
        {
            var flexuralUnityCheck = this.FlexuralUnityCheck(
                element1d,
                element1dResult,
                section,
                material
            );
            var shearUnityCheck = this.ShearUnityCheck(
                element1d,
                element1dResult,
                section,
                material
            );
            var deflectionCheck = this.DeflectionCheck(
                element1d,
                element1dResult,
                section,
                material
            );
            if (flexuralUnityCheck <= 1 || shearUnityCheck <= 1 || deflectionCheck <= 1)
            {
                return new AiscSteelSelectionResult(
                    section,
                    flexuralUnityCheck,
                    shearUnityCheck,
                    deflectionCheck
                );
            }
        }

        return null;
    }

    public double FlexuralUnityCheck(
        Element1d element1d,
        Element1dResult element1dResult,
        TSectionProfile sectionProfile,
        TMaterial material
    )
    {
        Trace.Assert(
            element1dResult.MaxMoment > element1dResult.MinMoment,
            "Max moment should be greater than min moment."
        );
        var maxMagnitudeMoment =
            element1dResult.MaxMoment.NewtonMeters
            > Math.Abs(element1dResult.MinMoment.NewtonMeters)
                ? element1dResult.MaxMoment
                : element1dResult.MinMoment * -1; // min moment is always less than max,
        // so it must be negative and we need to multiply by -1 to get the max magnitude

        var zxRequired = maxMagnitudeMoment.DivideBy(.9 * material.YieldStrength);
        return zxRequired / sectionProfile.StrongAxisPlasticSectionModulus;
    }

    public double ShearUnityCheck(
        Element1d element1d,
        Element1dResult element1dResult,
        TSectionProfile sectionProfile,
        TMaterial material
    )
    {
        Trace.Assert(
            element1dResult.MaxShear > element1dResult.MinShear,
            "Max shear should be greater than min shear."
        );
        var maxMagnitudeShear =
            element1dResult.MaxShear.Newtons > Math.Abs(element1dResult.MinShear.Newtons)
                ? element1dResult.MaxShear
                : element1dResult.MinShear * -1; // min shear is always less than max,
        // so it must be negative and we need to multiply by -1 to get the max magnitude

        var c1 = 1; // todo: this is almost always 1, but it can be different for some sections
        var vn = .6 * material.YieldStrength * sectionProfile.Aw * c1;
        return maxMagnitudeShear / vn;
    }

    public double DeflectionCheck(
        Element1d element1d,
        Element1dResult element1dResult,
        TSectionProfile sectionProfile,
        TMaterial material
    )
    {
        Trace.Assert(
            element1dResult.MaxDisplacement > element1dResult.MinDisplacement,
            "Max displacement should be greater than min displacement."
        );
        var maxMagnitudeDisplacement =
            element1dResult.MaxDisplacement.Meters
            > Math.Abs(element1dResult.MinDisplacement.Meters)
                ? element1dResult.MaxDisplacement
                : element1dResult.MinDisplacement * -1; // min shear is always less than max,
        // so it must be negative and we need to multiply by -1 to get the max magnitude

        return maxMagnitudeDisplacement / element1d.Length;
    }
}

public interface ISteelMaterial
{
    public Pressure YieldStrength { get; }
    public Pressure UltimateStrength { get; }
    public Pressure ModulusOfElasticity { get; }
    public Pressure ModulusOfRigidity { get; }
    public Pressure PoissonRatio { get; }
}

public interface IHasWebArea
{
    /// <summary>
    /// The area of the web of the section profile.
    /// </summary>
    public Area Aw { get; }
}

public interface IHasStrongAxisPlasticSectionModulus
{
    /// <summary>
    /// The strong axis plastic section modulus of the section profile.
    /// </summary>
    public Volume StrongAxisPlasticSectionModulus { get; }
}

public class AiscSteelSelectionResult
{
    public AiscSteelSelectionResult(
        SectionProfile sectionProfile,
        double flexuralUnityCheck,
        double shearUnityCheck,
        double deflectionCheck
    )
    {
        this.SectionProfile = sectionProfile;
        this.FlexuralUnityCheck = flexuralUnityCheck;
        this.ShearUnityCheck = shearUnityCheck;
        this.DeflectionCheck = deflectionCheck;
    }

    public SectionProfile SectionProfile { get; }
    public double FlexuralUnityCheck { get; }
    public double ShearUnityCheck { get; }
    public double DeflectionCheck { get; }
}
