using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;

public class GetModel(PhysicalModelDbContext dbContext, ModelResponseMapper modelResponseMapper)
    : BaseEndpoint,
        IGetEndpoint<string, ModelResponse>
{
    public override string Route => "models/{id}";

    public async Task<ModelResponse> GetAsync(string id, CancellationToken ct)
    {
        ModelId expectedId = new(Guid.Parse(id));
        Model? element = await dbContext
            .Models
            .FirstAsync(m => m.Id == expectedId, cancellationToken: ct);

        if (element is null)
        {
            return null;
        }

        ModelResponse response = modelResponseMapper.Map(element);
        return response;
    }
}
