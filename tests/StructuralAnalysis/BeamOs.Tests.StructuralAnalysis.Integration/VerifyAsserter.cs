using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.Tests.Common;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public class VerifyAsserter : Asserter
{
    public override async Task VerifyModelProposal(
        Result<ModelProposalResponse> modelProposalResponse
    )
    {
        await Verify(modelProposalResponse)
            .ScrubMembers(l =>
                typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
            );
    }
}
