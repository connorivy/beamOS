using Xunit.Sdk;

namespace BeamOS.Tests.Common.Traits;

public class ProblemSourceAttribute : TestBaseAttribute, ITraitAttribute
{
    public ProblemSourceAttribute(string source)
        : base("ProblemSource", source) { }
}

public class Kassimali_MatrixAnalysisOfStructures2ndEdAttribute : ProblemSourceAttribute
{
    public Kassimali_MatrixAnalysisOfStructures2ndEdAttribute()
        : base("Kassimali_MatrixAnalysisOfStructures2ndEd") { }
}
