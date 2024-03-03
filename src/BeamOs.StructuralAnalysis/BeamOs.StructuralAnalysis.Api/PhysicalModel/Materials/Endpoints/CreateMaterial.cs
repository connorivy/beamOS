using BeamOs.Api.PhysicalModel.Materials.Mappers;
using BeamOS.Common.Api;
using BeamOS.PhysicalModel.Application.Materials;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;

namespace BeamOs.Api.PhysicalModel.Materials.Endpoints;

public class CreateMaterial(
    CreateMaterialRequestMapper commandMapper,
    CreateMaterialCommandHandler createMaterialCommandHandler,
    MaterialResponseMapper materialResponseMapper
) : BeamOsFastEndpoint<CreateMaterialRequest, MaterialResponse>
{
    public override void Configure()
    {
        this.Post("/materials");
        this.AllowAnonymous();
    }

    //public override string Route => "materials";

    //public override EndpointType EndpointType => EndpointType.Post;

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
