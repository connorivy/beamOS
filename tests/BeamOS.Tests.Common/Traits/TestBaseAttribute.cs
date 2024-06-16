using Xunit.Abstractions;
using Xunit.Sdk;

namespace BeamOS.Tests.Common.Traits;

[TraitDiscoverer("BeamOS.Tests.Common.Traits.TestBaseDiscoverer", "BeamOS.Tests.Common")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class TestBaseAttribute : Attribute, ITraitAttribute
{
    public TestBaseAttribute(string traitName, string traitValue)
    {
        this.TraitName = traitName;
        this.TraitValue = traitValue;
    }

    public string TraitName { get; }
    public string TraitValue { get; }
}

/// <summary>
/// This class discovers all of the tests and test classes that have
/// applied the AnalysisMethod attribute
/// </summary>
public class TestBaseDiscoverer : ITraitDiscoverer
{
    /// <summary>
    /// Gets the trait values from the AnalysisMethod attribute.
    /// </summary>
    /// <param name="traitAttribute">The trait attribute containing the trait values.</param>
    /// <returns>The trait values.</returns>
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var traitName = traitAttribute.GetNamedArgument<string>(
            nameof(TestBaseAttribute.TraitName)
        );
        var traitValue = traitAttribute.GetNamedArgument<string>(
            nameof(TestBaseAttribute.TraitValue)
        );

        if (traitName is not null && traitValue is not null)
        {
            yield return new KeyValuePair<string, string>(traitName, traitValue);
        }
    }
}
