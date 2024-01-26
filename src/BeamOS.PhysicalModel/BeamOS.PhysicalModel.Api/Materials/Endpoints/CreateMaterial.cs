using BeamOS.Common.Api;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Api.Materials.Mappers;
using BeamOS.PhysicalModel.Application.Materials;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;

namespace BeamOS.PhysicalModel.Api.Materials.Endpoints;

public class CreateMaterial(
    CreateMaterialRequestMapper commandMapper,
    CreateMaterialCommandHandler createMaterialCommandHandler,
    MaterialResponseMapper materialResponseMapper
) : BeamOsEndpoint<CreateMaterialRequest, MaterialResponse>
{
    public override string Route => "materials";

    public override EndpointType EndpointType => EndpointType.Post;

    public override async Task<MaterialResponse> ExecuteAsync(
        CreateMaterialRequest request,
        CancellationToken ct
    )
    {
        CreateMaterialCommand command = commandMapper.Map(request);

        Material material = await createMaterialCommandHandler.ExecuteAsync(command, ct);

        MaterialResponse response = materialResponseMapper.Map(material);
        return response;
    }
}
