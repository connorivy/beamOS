using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Materials.Mappers;
using BeamOs.Application.PhysicalModel.Materials;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Materials.Endpoints;

public class CreateMaterial(
    BeamOsFastEndpointOptions options,
    CreateMaterialRequestMapper commandMapper,
    CreateMaterialCommandHandler createMaterialCommandHandler,
    MaterialResponseMapper materialResponseMapper
) : BeamOsFastEndpoint<CreateMaterialRequest, MaterialResponse>(options)
{
    public override string Route => "/materials";

    public override Http EndpointType => Http.POST;

    public override async Task<MaterialResponse> ExecuteAsync(
        CreateMaterialRequest request,
        CancellationToken ct
    )
    {
        var command = commandMapper.Map(request);

        Material material = await createMaterialCommandHandler.ExecuteAsync(command, ct);

        var response = materialResponseMapper.Map(material);
        return response;
    }
}
