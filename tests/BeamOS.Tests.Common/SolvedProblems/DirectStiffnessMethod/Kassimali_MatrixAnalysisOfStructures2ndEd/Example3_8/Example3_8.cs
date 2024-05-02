using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;

internal class Example3_8
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
        new(modelId, 12 * 12, 16 * 12, 0, Restraint: free2D);

    //private static CreateElement1dRequest Element1(string modelId)
    //{
    //    //return new(modelId);
    //}
}
