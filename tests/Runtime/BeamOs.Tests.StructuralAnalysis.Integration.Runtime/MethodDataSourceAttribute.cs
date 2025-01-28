namespace BeamOs.Tests.StructuralAnalysis.Integration;

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
