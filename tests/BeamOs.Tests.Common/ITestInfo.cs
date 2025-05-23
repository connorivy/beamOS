namespace BeamOs.Tests.Common;

public interface ITestInfo
{
    public TestType TestType { get; init; }
}

public enum TestType
{
    Undefined = 0,
    StructuralAnalysis,
    ModelRepair,
}

public abstract class TestInfoBase : ITestInfo
{
    public abstract TestType TestType { get; init; }
    public abstract object[]? TestData { get; init; }
    public abstract MethodInfo MethodInfo { get; init; }
    public abstract Type TestClassType { get; init; }

    public virtual object CreateTestClass(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService(this.TestClassType);
    }

    public virtual ITestFixture? GetTestFixture() =>
        this.TestData?.FirstOrDefault() as ITestFixture;

    public SourceInfo? SourceInfo => this.GetTestFixture()?.SourceInfo;
}
