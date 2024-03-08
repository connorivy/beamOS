using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class GetModel(
    BeamOsFastEndpointOptions options,
    BeamOsStructuralDbContext dbContext,
    ModelResponseMapper modelResponseMapper
) : BeamOsFastEndpoint<string, ModelResponse>(options)
{
    public override string Route => "models/{id}";

    public override Http EndpointType => Http.GET;

    public override async Task<ModelResponse?> ExecuteAsync(string id, CancellationToken ct)
    {
        ModelId expectedId = new(Guid.Parse(id));
        Model? element = await dbContext
            .Models
            .FirstAsync(m => m.Id == expectedId, cancellationToken: ct);

        if (element is null)
        {
            return null;
        }

        var response = modelResponseMapper.Map(element);
        return response;
    }
}
