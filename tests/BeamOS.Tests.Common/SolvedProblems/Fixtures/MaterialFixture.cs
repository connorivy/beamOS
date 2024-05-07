using UnitsNet;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public class MaterialFixture(Pressure modulusOfElasticity, Pressure modulusOfRigidity) : FixtureBase
{
    public Pressure ModulusOfElasticity { get; } = modulusOfElasticity;
    public Pressure ModulusOfRigidity { get; } = modulusOfRigidity;
}
