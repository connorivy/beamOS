using BeamOs.ApiClient.Builders;
using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.ETABS_Models.Simple_3_Story_Rectangular;

public partial class Simple_3_Story_Rectangular
    : CreateModelRequestBuilder,
        IHasExpectedNodeDisplacementResults
{
    private NodeResultFixture[] expectedNodeDisplacementResults;
    public NodeResultFixture[] ExpectedNodeDisplacementResults =>
        this.expectedNodeDisplacementResults ??=
        [
            new()
            {
                NodeId = NodeLocationString(0,0,0)
            }
        ];
}
