using System;
using BeamOs.Tests.Common.Integration;

namespace BeamOs.Tests.WebApp.Integration;

public class AssemblySetup
{
    [Before(TUnitHookType.Assembly)]
    public static async Task Setup()
    {
        await DbTestContainer.InitializeAsync();
    }
}
