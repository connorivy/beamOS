using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.ModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.NodeAggregate;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Factories;
internal static class Element1DFactory
{
    public static Element1D Create(
      UnitSettings? unitSettings = null,
      Node? startNode = null,
      Node? endNode = null,
      Material? material = null,
      SectionProfile? sectionProfile = null,
      Angle? rotation = default
      )
    {
        UnitSettings settings = unitSettings ?? UnitSettings.SI;
        return Element1D.Create(
            rotation ?? Angle.Zero,
            settings,
            startNode ?? Node.Create(0, 0, 0, settings.LengthUnit),
            endNode ?? Node.Create(1, 0, 0, settings.LengthUnit),
            material ?? Constants.UnitMaterialSI,
            sectionProfile ?? Constants.UnitSectionProfileSI
        );
    }
}

