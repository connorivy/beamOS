using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example7_6;

public class Udoeyo_StructuralAnalysis_Example7_6 : ModelFixture2
{
    public override PhysicalModelSettings Settings { get; } =
        new(UnitSettingsDtoVerbose.kN_M, new(Element1dAnalysisType.Euler));

    public override SourceInfo SourceInfo { get; } =
        new(
            "Structural Analysis by Felix Udoeyo",
            FixtureSourceType.Textbook,
            "Example7_6",
            null,
            "https://temple.manifoldapp.org/read/structural-analysis/section/072c774b-392a-4b99-a65d-f2e9d1fddfbd#:~:text=The%20moment%2Darea%20method%20uses,method%2C%20which%20are%20derived%20below."
        );
    public override Guid ModelGuid { get; }

    //override
}
