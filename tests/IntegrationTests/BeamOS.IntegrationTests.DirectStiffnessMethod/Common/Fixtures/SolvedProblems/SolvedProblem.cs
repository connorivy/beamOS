using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.Element1ds;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.Models;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems;

public abstract class SolvedProblem
{
    public ModelFixture ModelFixture { get; set; }
    public List<Element1dFixture> Element1dFixtures { get; set; } = [];
    public List<Node> Nodes { get; set; } = [];
    public List<PointLoad> PointLoads { get; set; } = [];
    public List<MomentLoad> MomentLoads { get; set; } = [];

    public List<CreateNodeRequest> CreateNodeRequests { get; set; } = [];
    public List<CreateElement1DRequest> CreateElement1DRequests { get; set; } = [];
}

public interface ISolvedProblem { }
