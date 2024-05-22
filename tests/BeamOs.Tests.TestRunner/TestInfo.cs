using System.Reflection;

namespace BeamOs.Tests.TestRunner;

public class TestInfo
{
    public TestInfo(
        Type testClassType,
        object[] testData,
        MethodInfo methodInfo,
        Dictionary<string, string[]> traitNameToValueDict,
        Guid? modelId,
        Guid? elementId
    )
    {
        this.ModelId = modelId;
        this.ElementId = elementId;
        this.TestData = testData;
        this.MethodInfo = methodInfo;
        this.TestClassType = testClassType;
        this.TraitNameToValueDict = traitNameToValueDict;
    }

    public Guid? ModelId { get; }
    public Guid? ElementId { get; }
    public object[] TestData { get; }
    public MethodInfo MethodInfo { get; }
    public Type TestClassType { get; }
    public Dictionary<string, string[]> TraitNameToValueDict { get; }
}
