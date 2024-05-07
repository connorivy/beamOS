using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Contracts.Common;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class DeleteModel(BeamOsFastEndpointOptions options, IModelRepository modelRepository)
    : BeamOsFastEndpoint<IdRequest, bool>(options)
{
    public override Http EndpointType => Http.DELETE;
    public override string Route => "/models/{id}";

    public override async Task<bool> ExecuteAsync(IdRequest req, CancellationToken ct)
    {
        ModelId modelId = new(Guid.Parse(req.Id));
        await modelRepository.RemoveById(modelId, ct);
        //await unitOfWork.SaveChangesAsync(); // I don't think this is necessary right here

        return true;
    }
}
