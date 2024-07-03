using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Nodes.Mappers;
using BeamOs.Application.PhysicalModel.Nodes.Commands;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Nodes.Endpoints;

public class PatchNode(
    BeamOsFastEndpointOptions options,
    PatchNodeCommandHandler patchNodeCommandHandler,
    NodeResponseMapper responseMapper
) : BeamOsFastEndpoint<PatchNodeRequest, NodeResponse>(options)
{
    public override Http EndpointType => Http.PATCH;
    public override string Route => "nodes/{nodeId}";

    public override PatchNodeRequest? ExampleRequest { get; } =
        new PatchNodeRequest(
            "00000000-0000-0000-0000-000000000000",
            new PatchPointRequest { LengthUnit = "Foot", YCoordinate = 15 },
            new PatchRestraintRequest(CanTranslateAlongX: false, CanRotateAboutY: true)
        );

    public override async Task<NodeResponse> ExecuteAsync(
        PatchNodeRequest req,
        CancellationToken ct
    )
    {
        Node node = await patchNodeCommandHandler.ExecuteAsync(req, ct);

        return responseMapper.Map(node);
    }
}
