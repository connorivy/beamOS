using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Factories;

internal static class Element1DFactory
{
    public static AnalyticalElement1D Create(
        UnitSettings? unitSettings = null,
        DsmNode? startNode = null,
        DsmNode? endNode = null,
        Material? material = null,
        SectionProfile? sectionProfile = null,
        Angle? rotation = default
    )
    {
        UnitSettings settings = unitSettings ?? UnitSettings.SI;
        return new(
            rotation ?? Angle.Zero,
            startNode ?? new(0, 0, 0, settings.LengthUnit, Restraint.Free),
            endNode ?? new(1, 0, 0, settings.LengthUnit, Restraint.Free),
            material ?? Constants.UnitMaterialSI,
            sectionProfile ?? Constants.UnitSectionProfileSI
        );
    }
}
