using System.Diagnostics;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.Tests.Common;
using DiffEngine;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public class VerifyAsserter : Asserter
{
    public override async Task VerifyModelProposal(
        Result<ModelProposalResponse> modelProposalResponse
    )
    {
        try
        {
            await Verify(modelProposalResponse)
                .ScrubMembers(l =>
                    typeof(IHasIntId).IsAssignableFrom(l.DeclaringType) && l.Name == "Id"
                );
        }
        catch
        {
            DiffRunner.Disabled = false;
            var tempFile =
                @"/workspaces/beamOS/tests/StructuralAnalysis/BeamOs.Tests.StructuralAnalysis.Integration/ExtendCoplanarElement1dsToJoinNodesTests.BeamsThatAlmostMeetAtAPoint_ButBothNeedToBeExtended_ShouldBeExtended_apiClient=null.received.txt";
            var verFile =
                @"/workspaces/beamOS/tests/StructuralAnalysis/BeamOs.Tests.StructuralAnalysis.Integration/ExtendCoplanarElement1dsToJoinNodesTests.BeamsThatAlmostMeetAtAPoint_ButBothNeedToBeExtended_ShouldBeExtended_apiClient=null.verified.txt";
            // // Process p = new()
            // // {
            // //     StartInfo = new ProcessStartInfo
            // //     {
            // //         FileName =
            // //             "/vscode/vscode-server/bin/linux-x64/2901c5ac6db8a986a5666c3af51ff804d05af0d4/bin/remote-cli/code",
            // //         Arguments = "--diff " + $"\"{tempFile}\" " + $"\"{verFile}\" -n",
            // //         // RedirectStandardOutput = true,
            // //         // RedirectStandardError = true,
            // //         // UseShellExecute = false,
            // //         // CreateNoWindow = true,
            // //     },
            // // };
            // // p.Start();
            // // // string output = p.StandardOutput.ReadToEnd();
            // // // string error = p.StandardError.ReadToEnd();
            // // p.WaitForExit();
            // await DiffRunner.LaunchAsync(tempFile, verFile);
            throw;
        }
    }
}
