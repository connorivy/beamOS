namespace BeamOs.Tests.Common;

public class TestAttribute : Attribute { }

[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property,
    AllowMultiple = true
)]
public class MethodDataSourceAttribute(
    Type classProvidingDataSource,
    string methodNameProvidingDataSource
) : Attribute
{
    public Type ClassProvidingDataSource { get; } = classProvidingDataSource;
    public string MethodNameProvidingDataSource { get; } = methodNameProvidingDataSource;
}

public class ParallelGroupAttribute(string group) : Attribute
{
    public string Group { get; } = group;
}
