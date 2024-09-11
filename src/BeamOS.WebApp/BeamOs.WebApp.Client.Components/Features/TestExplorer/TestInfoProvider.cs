using BeamOS.Tests.Common.Interfaces;
using BeamOs.Tests.TestRunner;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public class TestInfoProvider
{
    public Dictionary<string, TestInfo> TestInfos { get; }
    public Dictionary<string, string[]> AllTraits { get; } = [];
    public Dictionary<string, string[]> SourceNameToModelNameDict { get; }

    //private const string defaultTrait = ProblemSourceAttribute.TRAIT_NAME;

    public TestInfoProvider()
    {
        this.TestInfos = AssemblyScanning.GetAllTestInfo().ToDictionary(t => t.Id, t => t);

        IEnumerable<string> traitKeys = this.TestInfos
            .Values
            .SelectMany(i => i.TraitNameToValueDict.Keys)
            .Distinct();

        foreach (string key in traitKeys)
        {
            this.AllTraits.Add(
                key,
                this.TestInfos
                    .Values
                    .Select(i => i.TraitNameToValueDict.GetValueOrDefault(key))
                    .Where(i => i is not null)
                    .Distinct()
                    .ToArray()
            );
        }
        this.SourceNameToModelNameDict = CreateSourceNameToModelNamesDict(
            this.TestInfos.Values.Select(t => t.SourceInfo)
        );
    }

    private static Dictionary<string, string[]> CreateSourceNameToModelNamesDict(
        IEnumerable<SourceInfo?> sourceInfos
    )
    {
        Dictionary<string, HashSet<string>> sourceNameToModelNameDict = [];
        foreach (SourceInfo? sourceInfo in sourceInfos)
        {
            if (sourceInfo is null)
            {
                continue;
            }

            if (
                !sourceNameToModelNameDict.TryGetValue(
                    sourceInfo.SourceName,
                    out HashSet<string>? modelNames
                )
            )
            {
                modelNames =  [];
                sourceNameToModelNameDict.Add(sourceInfo.SourceName, modelNames);
            }

            if (sourceInfo.ModelName is not null)
            {
                _ = modelNames.Add(sourceInfo.ModelName);
            }
        }

        return sourceNameToModelNameDict.ToDictionary(
            dict => dict.Key,
            dict => dict.Value.ToArray()
        );
    }
}
