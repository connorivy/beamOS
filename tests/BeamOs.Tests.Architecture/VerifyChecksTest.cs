using TUnit.Core;
using VerifyTUnit;

namespace BeamOs.Tests.Architecture;

public class VerifyChecksTest
{
    [Test]
    public Task RunVerifyChecks() => VerifyChecks.Run();
}
