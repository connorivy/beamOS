using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Factories;

internal static class MaterialFactory
{
    public static Material Create(
        Pressure? modulusOfElasticity = null,
        Pressure? modulusOfRigidity = null
    ) =>
        new(
            modulusOfElasticity ?? new Pressure(1, UnitSystem.SI),
            modulusOfRigidity ?? new Pressure(1, UnitSystem.SI)
        );
}
