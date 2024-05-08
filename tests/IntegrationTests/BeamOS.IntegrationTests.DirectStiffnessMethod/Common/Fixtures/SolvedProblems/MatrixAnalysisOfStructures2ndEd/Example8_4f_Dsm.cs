using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.DirectStiffnessMethod.Services;
using BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

internal class Example8_4f_Dsm : Example8_4f
{
    //public Example8_4f_Dsm()
    //    : base(new Example8_4f()) { }

    public IEnumerable<(
        NodeFixture node,
        CoordinateSystemDirection3D direction
    )> DegreeOfFreedomIds { get; } =

        [
            (Nodes.Node1, CoordinateSystemDirection3D.AlongX),
            (Nodes.Node1, CoordinateSystemDirection3D.AlongY),
            (Nodes.Node1, CoordinateSystemDirection3D.AlongZ),
            (Nodes.Node1, CoordinateSystemDirection3D.AboutX),
            (Nodes.Node1, CoordinateSystemDirection3D.AboutY),
            (Nodes.Node1, CoordinateSystemDirection3D.AboutZ),
        ];

    public IEnumerable<(
        NodeFixture node,
        CoordinateSystemDirection3D direction
    )> BoundaryConditionIds { get; } =

        [
            (Nodes.Node2, CoordinateSystemDirection3D.AlongX),
            (Nodes.Node2, CoordinateSystemDirection3D.AlongY),
            (Nodes.Node2, CoordinateSystemDirection3D.AlongZ),
            (Nodes.Node2, CoordinateSystemDirection3D.AboutX),
            (Nodes.Node2, CoordinateSystemDirection3D.AboutY),
            (Nodes.Node2, CoordinateSystemDirection3D.AboutZ),
            (Nodes.Node3, CoordinateSystemDirection3D.AlongX),
            (Nodes.Node3, CoordinateSystemDirection3D.AlongY),
            (Nodes.Node3, CoordinateSystemDirection3D.AlongZ),
            (Nodes.Node3, CoordinateSystemDirection3D.AboutX),
            (Nodes.Node3, CoordinateSystemDirection3D.AboutY),
            (Nodes.Node3, CoordinateSystemDirection3D.AboutZ),
            (Nodes.Node4, CoordinateSystemDirection3D.AlongX),
            (Nodes.Node4, CoordinateSystemDirection3D.AlongY),
            (Nodes.Node4, CoordinateSystemDirection3D.AlongZ),
            (Nodes.Node4, CoordinateSystemDirection3D.AboutX),
            (Nodes.Node4, CoordinateSystemDirection3D.AboutY),
            (Nodes.Node4, CoordinateSystemDirection3D.AboutZ),
        ];

    public static void he()
    {
        DirectStiffnessMethodSolver.BuildStructureStiffnessMatrix
    }
}
