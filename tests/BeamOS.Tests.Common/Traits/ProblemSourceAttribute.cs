using Xunit.Sdk;

namespace BeamOS.Tests.Common.Traits;

public class ProblemSourceAttribute : TestBaseAttribute, ITraitAttribute
{
    public const string TRAIT_NAME = "ProblemSource";

    public ProblemSourceAttribute(string source)
        : base(TRAIT_NAME, source) { }
}

public class Sap2000ModelAttribute : ProblemSourceAttribute
{
    public const string TRAIT_VALUE = "SAP2000";

    public Sap2000ModelAttribute()
        : base(TRAIT_VALUE) { }
}

public class Kassimali_MatrixAnalysisOfStructures2ndEdAttribute : ProblemSourceAttribute
{
    public const string TRAIT_VALUE = "Kassimali_MatrixAnalysisOfStructures2ndEd";

    public Kassimali_MatrixAnalysisOfStructures2ndEdAttribute()
        : base(TRAIT_VALUE) { }
}
