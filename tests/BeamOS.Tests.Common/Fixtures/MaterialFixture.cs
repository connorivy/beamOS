using UnitsNet;

namespace BeamOS.Tests.Common.Fixtures;

public class MaterialFixture(Pressure modulusOfElasticity, Pressure modulusOfRigidity) : FixtureBase
{
    public Pressure ModulusOfElasticity { get; } = modulusOfElasticity;
    public Pressure ModulusOfRigidity { get; } = modulusOfRigidity;
}

public class MaterialFixture2 : FixtureBase2
{
    public required Guid ModelId { get; init; }
    public required Pressure ModulusOfElasticity { get; init; }
    public required Pressure ModulusOfRigidity { get; init; }
}
