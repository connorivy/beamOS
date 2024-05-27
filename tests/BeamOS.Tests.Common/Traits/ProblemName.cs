using Xunit.Sdk;

namespace BeamOS.Tests.Common.Traits;

public class ProblemNameAttribute : TestBaseAttribute, ITraitAttribute
{
    public const string TRAIT_NAME = "ProblemName";

    public ProblemNameAttribute(string name)
        : base(TRAIT_NAME, name) { }
}
