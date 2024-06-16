using Xunit.Sdk;

namespace BeamOS.Tests.Common.Traits;

public class AnalysisMethodAttribute : TestBaseAttribute, ITraitAttribute
{
    public AnalysisMethodAttribute(string analysisMethod)
        : base("AnalysisMethod", analysisMethod) { }
}

public class DirectStiffnessMethodAttribute : AnalysisMethodAttribute
{
    public DirectStiffnessMethodAttribute()
        : base("DirectStiffnessMethod") { }
}
