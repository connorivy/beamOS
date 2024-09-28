using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.PhysicalModel.Nodes.Commands;
using BeamOs.Common.Identity.Policies;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Node;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Nodes.Endpoints;

public class PatchNode(
    BeamOsFastEndpointOptions options,
    PatchNodeCommandHandler patchNodeCommandHandler
) : BeamOsFastEndpoint<PatchNodeRequest, NodeResponse>(options)
{
    public override Http EndpointType => Http.PATCH;
    public override string Route => "nodes/{nodeId}";

    public override PatchNodeRequest? ExampleRequest { get; } =
        new PatchNodeRequest()
        {
            NodeId = "00000000-0000-0000-0000-000000000000",
            LocationPoint = new PatchPointRequest
            {
                LengthUnit = LengthUnitContract.Foot,
                YCoordinate = 15
            },
            Restraint = new PatchRestraintRequest(CanTranslateAlongX: false, CanRotateAboutY: true)
        };

    public override void ConfigureAuthentication()
    {
        this.Policy(p => p.Requirements.Add(new RequireNodeWriteAccess()));
    }

    public override async Task<NodeResponse> ExecuteRequestAsync(
        PatchNodeRequest req,
        CancellationToken ct
    )
    {
        return await patchNodeCommandHandler.ExecuteAsync(req, ct);
    }
}
