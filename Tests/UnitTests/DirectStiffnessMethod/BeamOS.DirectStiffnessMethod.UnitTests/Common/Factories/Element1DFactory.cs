using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Factories;
internal static class Element1DFactory
{
    public static Element1D Create(
      UnitSettings? unitSettings = null,
      AnalyticalNode? startNode = null,
      AnalyticalNode? endNode = null,
      Material? material = null,
      SectionProfile? sectionProfile = null,
      Angle? rotation = default
      )
    {
        UnitSettings settings = unitSettings ?? UnitSettings.SI;
        return Element1D.Create(
            rotation ?? Angle.Zero,
            settings,
            startNode ?? AnalyticalNode.Create(0, 0, 0, settings.LengthUnit),
            endNode ?? AnalyticalNode.Create(1, 0, 0, settings.LengthUnit),
            material ?? Constants.UnitMaterialSI,
            sectionProfile ?? Constants.UnitSectionProfileSI
        );
    }
}

