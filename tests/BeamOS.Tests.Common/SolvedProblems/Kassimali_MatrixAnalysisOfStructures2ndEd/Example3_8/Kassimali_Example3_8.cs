using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;

internal class Kassimali_Example3_8 : ModelFixture2, IHasExpectedNodeResults2
{
    public CreateModelRequest CreateModelRequest { get; } =
        new(
            "Example 3.8",
            "Plane truss direct stiffness method problem",
            new PhysicalModelSettingsDto(UnitSettingsDtoVerbose.K_IN)
        );

    public IEnumerable<CreateElement1dRequest> CreateElement1dRequests = [];

    private static string modelId;
    private static RestraintRequest free2D = new(true, true, false, false, false, true);
    private static RestraintRequest pinned2d = new(false, false, false, false, false, true);

    private static CreateNodeRequest Node1 { get; } =
        new(modelId, 12, 16, 0, "Foot", restraint: free2D);

    public override Guid Id { get; init; }
    public override ModelSettings Settings { get; } = new(UnitSettings.K_IN);
    public override SourceInfo SourceInfo { get; }
    public NodeResultFixture2[] ExpectedNodeResults { get; }

    public NodeResultResponse ToResponse(NodeResultFixture source) =>
        throw new NotImplementedException();
}
