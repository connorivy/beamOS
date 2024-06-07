using BeamOs.Tests.TestRunner;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

public class TestInfoProvider
{
    public TestInfo[] TestInfos { get; } = [];
    public Dictionary<string, string[]> AllTraits { get; } = [];

    //private const string defaultTrait = ProblemSourceAttribute.TRAIT_NAME;

    public TestInfoProvider()
    {
        this.TestInfos = AssemblyScanning.GetAllTestInfo().ToArray();

        IEnumerable<string> traitKeys = this.TestInfos
            .SelectMany(i => i.TraitNameToValueDict.Keys)
            .Distinct();

        foreach (string key in traitKeys)
        {
            this.AllTraits.Add(
                key,
                this.TestInfos
                    .SelectMany(i => i.TraitNameToValueDict.GetValueOrDefault(key) ?? [])
                    .Distinct()
                    .ToArray()
            );
        }
    }
}
