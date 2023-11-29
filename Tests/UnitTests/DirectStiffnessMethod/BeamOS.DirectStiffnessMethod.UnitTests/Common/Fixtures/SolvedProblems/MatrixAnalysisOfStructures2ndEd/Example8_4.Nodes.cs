using BeamOS.Common.Domain.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

internal partial class Example8_4
{
    public static AnalyticalNode AnalyticalNode1 { get; }
    public static AnalyticalNode AnalyticalNode2 { get; }
    public static AnalyticalNode AnalyticalNode3 { get; }
    public static AnalyticalNode AnalyticalNode4 { get; }

    private static AnalyticalNode GetAnalyticalNode1()
    {
        return new(0, 0, 0, LengthUnit.Foot, Restraint.Free, id: new(Constants.Guid1));
    }
    private static AnalyticalNode GetAnalyticalNode2()
    {
        return new(-20, 0, 0, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid2));
    }
    private static AnalyticalNode GetAnalyticalNode3()
    {
        return new(0, -20, 0, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid3));
    }
    private static AnalyticalNode GetAnalyticalNode4()
    {
        return new(0, 0, -20, LengthUnit.Foot, Restraint.Fixed, id: new(Constants.Guid4));
    }
}
