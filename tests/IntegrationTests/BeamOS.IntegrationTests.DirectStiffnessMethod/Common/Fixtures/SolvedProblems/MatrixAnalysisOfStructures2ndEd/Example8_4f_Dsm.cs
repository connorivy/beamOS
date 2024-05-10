using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

internal class Example8_4f_Dsm : DsmModelFixture, IDsmModelFixture, IHasStructuralStiffnessMatrix
{
    public Example8_4f_Dsm(Example8_4f modelFixture)
        : base(modelFixture)
    {
        this.DsmNodes =
        [
            this.ToDsm(Nodes.Node1),
            this.ToDsm(Nodes.Node2),
            this.ToDsm(Nodes.Node3),
            this.ToDsm(Nodes.Node4),
        ];

        this.DsmElement1ds =
        [
            this.ToDsm(Element1ds.Element1),
            this.ToDsm(Element1ds.Element2),
            this.ToDsm(Element1ds.Element3),
        ];

        this.DsmElement1ds2 =
        [
            DsmElement1dFixtures.Element1,
            DsmElement1dFixtures.Element2,
            DsmElement1dFixtures.Element3,
        ];
    }

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

    public DsmNodeVo[] DsmNodes { get; }
    public DsmElement1d[] DsmElement1ds { get; }
    public DsmElement1dFixture[] DsmElement1ds2 { get; }
    public double[,] ExpectedStructuralStiffnessMatrix { get; } =
        new double[,]
        {
            { 3990.3, -5.2322, 0, -627.87, -1075.4, 712.92 },
            { -5.2322, 4008.4, 0, 1800.4, 627.87, -2162.9 },
            { 0, 0, 3999.4, -2162.9, 712.92, 0 },
            { -627.87, 1800.4, -2162.9, 634857, 100459, 0 },
            { -1075.4, 627.87, 712.92, 100459, 286857, 0 },
            { 712.92, -2162.9, 0, 0, 0, 460857 }
        };
}
