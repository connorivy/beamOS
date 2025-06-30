using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.Tests.Common;

// public class TestTypeAttribute(TestType testType) : Attribute
// {
//     public TestType TestType { get; } = testType;
// }

public class ComparedObjectEventArgs<T>(T expected, T calculated, string comparedObjectName)
    : EventArgs
{
    public T Expected { get; } = expected;
    public T Calculated { get; } = calculated;
    public string ComparedObjectName { get; } = comparedObjectName;
}

public record ComparedObjectEventArgs
{
    public required BeamOsObjectType BeamOsObjectType { get; init; }
    public required string BeamOsObjectId { get; init; }
    public required string ComparedObjectPropertyName { get; init; }
    public required object ExpectedValue { get; init; }
    public required object CalculatedValue { get; init; }
    public ICollection<string>? ComparedValueNameCollection { get; init; }
}

public static class TestUtils
{
    public static Asserter Asserter { get; set; } = new Asserter();
}
