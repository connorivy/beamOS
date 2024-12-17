using VerifyTUnit;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public class VerifyChecksTest
{
    [Test]
    public Task RunVerifyChecks() => VerifyChecks.Run();
}
