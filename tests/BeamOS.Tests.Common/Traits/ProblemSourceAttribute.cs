using Xunit.Sdk;

namespace BeamOS.Tests.Common.Traits;

public class ProblemSourceAttribute : TestBaseAttribute, ITraitAttribute
{
    public const string TRAIT_NAME = "ProblemSource";

    public ProblemSourceAttribute(string source)
        : base(TRAIT_NAME, source) { }
}

public class Kassimali_MatrixAnalysisOfStructures2ndEdAttribute : ProblemSourceAttribute
{
    public Kassimali_MatrixAnalysisOfStructures2ndEdAttribute()
        : base("Kassimali_MatrixAnalysisOfStructures2ndEd") { }
}
